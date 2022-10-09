using Microsoft.Extensions.Logging;

namespace core;

public static class ErrorCatching
{
	public static void ExecuteWithErrorCatching<T>(ILogger<T> logger, Action action)
	{
		try
		{
			action.Invoke();
		}
		catch (Exception e)
		{
			LogError(e, logger);
		}
	}
	
	public static async Task ExecuteWithErrorCatching<T>(ILogger<T> logger, Func<Task> action)
	{
		try
		{
			await action.Invoke();
		}
		catch (Exception e)
		{
			LogError(e, logger);
		}
	}

	private static void LogError<T>(Exception e, ILogger<T> logger)
	{
		logger.LogError(e.Message);
		logger.LogError(e.StackTrace);
		if (e.InnerException != null)
		{
			LogError(e.InnerException, logger);
		}
	}
}