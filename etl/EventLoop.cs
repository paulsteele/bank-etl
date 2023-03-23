using Autofac;
using core.models;
using Microsoft.Extensions.Logging;

namespace core;

public class EventLoop
{
	private readonly ILogger<EventLoop> _logger;
	private readonly ILifetimeScope _lifetimeScope;

	public EventLoop(
		ILogger<EventLoop> logger,
		ILifetimeScope lifetimeScope
	)
	{
		_logger = logger;
		_lifetimeScope = lifetimeScope;
	}

	public void Start(IEnumerable<IFlow> flows)
	{
		
	}
}