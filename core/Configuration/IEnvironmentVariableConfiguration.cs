namespace core.Configuration;

public interface IEnvironmentVariableConfiguration {
	string DatabaseUrl { get; }
	string DatabasePort { get; }
	string DatabaseUser { get; }
	string DatabasePassword { get; }
	string DatabaseName { get; }
	string AwsAccessKey { get; }
	string AwsSecretAccessKey { get; }
}