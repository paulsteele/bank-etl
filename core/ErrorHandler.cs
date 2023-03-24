using Microsoft.Extensions.Logging;

namespace core;

public class ErrorHandler
{
	public async Task ExecuteWithErrorCatching<TY>(ILogger<TY> logger, Func<Task> action)
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
	
	public async Task<T> ExecuteWithErrorCatching<T, TL>(ILogger<TL> logger, Func<Task<T>> action, T errorItem)
	{
		try
		{
			return await action.Invoke();
		}
		catch (Exception e)
		{
			LogError(e, logger);
			return errorItem;
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