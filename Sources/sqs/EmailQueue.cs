using System.Net;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using core.Configuration;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace sqs;

internal class EmailQueue : ISource
{
	private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;
	private readonly IDb _database;
	private readonly ILogger<EmailQueue> _logger;
	private const string ReceivedFromSqs = nameof(ReceivedFromSqs);

	public EmailQueue(
		IEnvironmentVariableConfiguration environmentVariableConfiguration,
		IDb database,
		ILogger<EmailQueue> logger
	)
	{
		_environmentVariableConfiguration = environmentVariableConfiguration;
		_database = database;
		_logger = logger;
	}

	private AmazonSQSClient CreateClient()
	{
		return new AmazonSQSClient(_environmentVariableConfiguration.AwsAccessKey, _environmentVariableConfiguration.AwsSecretAccessKey);
	}

	public void StartListening()
	{
		var client = CreateClient();

		var timer = new Timer(30000);
		timer.Elapsed += async (sender, args) =>
		{
			var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(_environmentVariableConfiguration.SqsQueueUrl){WaitTimeSeconds = 10});
			if (!AssertSuccess(response))
			{
				return;
			}
			_logger.LogInformation($"Found {response.Messages.Count} messages");
			// ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
			foreach (var message in response.Messages)
			{
				var item = new BankItem
				{
					RawPayload = message.Body,
					State = ReceivedFromSqs
				};
				
				_logger.LogInformation($"Saved {message.MessageId}");
				_database.UpsertBankItem(item);
				_database.SaveChanges();

				var deleteResponse = await client.DeleteMessageAsync(new DeleteMessageRequest(_environmentVariableConfiguration.SqsQueueUrl, message.ReceiptHandle));
				if (AssertSuccess(deleteResponse))
				{
					_logger.LogInformation($"Removed {message.MessageId} from the queue");
				}
			}
		};
	}

	private bool AssertSuccess(AmazonWebServiceResponse response)
	{
		if (response.HttpStatusCode == HttpStatusCode.OK)
		{
			return true;
		}

		foreach (var metadata in response.ResponseMetadata.Metadata)
		{
			_logger.LogError($"{metadata.Key} : {metadata.Value}");
		}

		return false;
	}
}
