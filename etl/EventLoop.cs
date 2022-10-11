using System.ComponentModel;
using Autofac;
using core.Db;
using core.models;
using sqs;

namespace core;

public class EventLoop
{
	private readonly IEnumerable<ISource> _sources;
	private readonly ILifetimeScope _lifetimeScope;

	public EventLoop(IEnumerable<ISource> sources, ILifetimeScope lifetimeScope)
	{
		_sources = sources;
		_lifetimeScope = lifetimeScope;
	}

	public void Start()
	{
			using var scope = _lifetimeScope.BeginLifetimeScope();
			var db = scope.Resolve<IDb>();
		while (true)
		{
			foreach (var source in _sources)
			{
				source.Poll(db);
			}
			
			Thread.Sleep(10000);
		}

		// ReSharper disable once FunctionNeverReturns
	}
}