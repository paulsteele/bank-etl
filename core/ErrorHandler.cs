using Microsoft.Extensions.Logging;

namespace core;

public class ErrorHandler
{
	public T ExecuteWithErrorCatching<T, TY>(ILogger<TY> logger, Func<T> action)
	{
		try
		{
			return action.Invoke();
		}
		catch (Exception e)
		{
			LogError(e, logger);
			return default!;
		}
	}
	
	public Task<T> ExecuteWithErrorCatching<T, TL>(ILogger<TL> logger, Func<Task<T>> action)
	{
		try
		{
			return action.Invoke();
		}
		catch (Exception e)
		{
			LogError(e, logger);
			return Task.FromException<T>(e);
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