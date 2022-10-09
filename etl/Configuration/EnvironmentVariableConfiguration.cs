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
		}

		private static T GetVar<T>(string name, T defaultValue, Func<string, T> converter) {
			var envVar = Environment.GetEnvironmentVariable(name) ?? Environment.GetEnvironmentVariable(name.ToUpper());
			return envVar != null ? converter(envVar) : defaultValue;
		}

		private static string ConvertString(string s) {
			return s;
		}

		private static int ConvertInt(string s) {
			return int.Parse(s);
		}

		public string DatabaseUrl { get; }
		public string DatabasePort { get; }
		public string DatabaseUser { get; }
		public string DatabasePassword { get; }
		public string DatabaseName { get; }
		public string AwsAccessKey { get; }
		public string AwsSecretAccessKey { get; }
		public string SqsQueueUrl { get; }
	}
}
