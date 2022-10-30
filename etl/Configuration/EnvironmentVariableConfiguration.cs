namespace core.Configuration {
	public class EnvironmentVariableConfiguration : IEnvironmentVariableConfiguration {

		public EnvironmentVariableConfiguration() {

			DatabaseUrl = GetVar(nameof(DatabaseUrl), "localhost", ConvertString);
			DatabasePort = GetVar(nameof(DatabasePort), "3306", ConvertString);
			DatabaseUser = GetVar(nameof(DatabaseUser), "root", ConvertString);
			DatabasePassword = GetVar(nameof(DatabasePassword), "pass", ConvertString);
			DatabaseName = GetVar(nameof(DatabaseName), "hub", ConvertString);
			AwsAccessKey = GetVar(nameof(AwsAccessKey), "access", ConvertString);
			AwsSecretAccessKey = GetVar(nameof(AwsSecretAccessKey), "secret", ConvertString);
			SqsQueueUrl = GetVar(nameof(SqsQueueUrl), "example", ConvertString);
			DiscordBotKey = GetVar(nameof(DiscordBotKey), "key", ConvertString);
			DiscordChannelName = GetVar(nameof(DiscordChannelName), "bank-transactions", ConvertString);
			FireflyHost = GetVar(nameof(FireflyHost), "https://example.com", ConvertString);
			FireflyToken = GetVar(nameof(FireflyToken), "token", ConvertString);
		}

		private static T GetVar<T>(string name, T defaultValue, Func<string, T> converter) {
			var envVar = Environment.GetEnvironmentVariable(name) ?? Environment.GetEnvironmentVariable(name.ToUpper());
			return envVar != null ? converter(envVar) : defaultValue;
		}

		private static string ConvertString(string s) {
			return s;
		}

		public string DatabaseUrl { get; }
		public string DatabasePort { get; }
		public string DatabaseUser { get; }
		public string DatabasePassword { get; }
		public string DatabaseName { get; }
		public string AwsAccessKey { get; }
		public string AwsSecretAccessKey { get; }
		public string SqsQueueUrl { get; }
		public string DiscordBotKey { get; }
		public string DiscordChannelName { get; }
		public string FireflyToken { get; }
		public string FireflyHost { get; }
	}
}
