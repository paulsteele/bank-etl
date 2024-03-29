﻿using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using core;
using core.Configuration;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace sqs.sources;

public class EmailQueue : ISource<BankItem>
{
	private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;
	private readonly ILogger<EmailQueue> _logger;
	private readonly IErrorHandler _errorHandler;

	public EmailQueue(
		IEnvironmentVariableConfiguration environmentVariableConfiguration,
		ILogger<EmailQueue> logger,
		IErrorHandler errorHandler
	)
	{
		_environmentVariableConfiguration = environmentVariableConfiguration;
		_logger = logger;
		_errorHandler = errorHandler;
	}

	private AmazonSQSClient CreateClient()
	{
		return new AmazonSQSClient(
			_environmentVariableConfiguration.AwsAccessKey,
			_environmentVariableConfiguration.AwsSecretAccessKey,
			new AmazonSQSConfig {RegionEndpoint = RegionEndpoint.USEast1}
		);
	}

	public Task<TimeSpan> Poll(IDb database, string successState)
	{
		return _errorHandler.ExecuteWithErrorCatching(
			_logger, async () =>
			{
				var client = CreateClient();
				var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(_environmentVariableConfiguration.SqsQueueUrl) {WaitTimeSeconds = 0, MaxNumberOfMessages = 5});
				if (!AssertSuccess(response))
				{
					return TimeSpan.FromSeconds(30);
				}

				_logger.LogInformation($"Found {response.Messages.Count} messages");

				// ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
				foreach (var message in response.Messages)
				{
					var item = new BankItem
					{
						RawPayload = message.Body,
						State = successState
					};

					database.AddItem(item);
					database.SaveChanges();
					_logger.LogInformation($"Added {message.MessageId}");

					var deleteResponse = await client.DeleteMessageAsync(new DeleteMessageRequest(_environmentVariableConfiguration.SqsQueueUrl, message.ReceiptHandle));
					if (AssertSuccess(deleteResponse))
					{
						_logger.LogInformation($"Removed {message.MessageId} from the queue");
					}
				}

				return response.Messages.Count > 0 ? TimeSpan.FromSeconds(30) : TimeSpan.FromMinutes(1);
			},
			TimeSpan.FromSeconds(30)
		);
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
