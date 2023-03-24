using Autofac;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace core;

public class EventLoop
{
	private readonly ILogger<EventLoop> _logger;
	private bool _keepRunning = true;
	private readonly ILifetimeScope _lifetimeScope;

	public EventLoop(
		ILogger<EventLoop> logger,
		ILifetimeScope lifetimeScope
	)
	{
		_logger = logger;
		_lifetimeScope = lifetimeScope;
	}

	public void Stop()
	{
		_keepRunning = false;
	}

	public Task Start(IEnumerable<IFlow> flows)
	{
		var taskCompletionSources = new List<TaskCompletionSource>();
		foreach (var flow in flows)
		{
			var completionSource = new TaskCompletionSource();
			taskCompletionSources.Add(completionSource);
			Task.Run(async () =>
			{
				while (_keepRunning)
				{
					await using var scope = _lifetimeScope.BeginLifetimeScope();
					var db = scope.Resolve<IDb>();
					var delayTime = await flow.Execute(db);
					_logger.LogInformation($"Flow {flow.Name} completed. Waiting {delayTime}");
					await Task.Delay(delayTime);
				}

				completionSource.TrySetResult();
			});
		}

		return Task.WhenAll(taskCompletionSources.Select(tsc => tsc.Task));
	}
}