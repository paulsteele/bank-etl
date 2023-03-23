using Microsoft.Extensions.Logging;

namespace core;

public class ErrorHandler
{
	public void ExecuteWithErrorCatching<T>(ILogger<T> logger, Action action)
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
	
	public async Task ExecuteWithErrorCatching<T>(ILogger<T> logger, Func<Task> action)
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

	private void LogError<T>(Exception e, ILogger<T> logger)
	{
		logger.LogError(e.Message);
		logger.LogError(e.StackTrace);
		if (e.InnerException != null)
		{
			LogError(e.InnerException, logger);
		}
	}
}