using Amazon.SQS;
using core.Configuration;

namespace sqs;
internal class EmailQueue
{
	 private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;

	 public EmailQueue(IEnvironmentVariableConfiguration environmentVariableConfiguration)
	 {
		 _environmentVariableConfiguration = environmentVariableConfiguration;
	 }

	 private AmazonSQSClient CreateClient()
	 {
		 return new AmazonSQSClient(_environmentVariableConfiguration.AwsAccessKey, _environmentVariableConfiguration.AwsSecretAccessKey);
	 }
}
