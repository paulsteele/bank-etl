using System.ComponentModel;
using Autofac;
using core.Db;
using core.models;
using sqs;

namespace core;

public class EventLoop
{
	private readonly IEnumerable<ISource> _sources;
	private readonly IEnumerable<ITransformer> _transformers;
	private readonly ILifetimeScope _lifetimeScope;

	public EventLoop(
		IEnumerable<ISource> sources, 
		IEnumerable<ITransformer> transformers,
		ILifetimeScope lifetimeScope)
	{
		_sources = sources;
		_transformers = transformers;
		_lifetimeScope = lifetimeScope;
	}

	public async Task Start()
	{
		while (true)
		{
			await using var scope = _lifetimeScope.BeginLifetimeScope();
			var db = scope.Resolve<IDb>();
			foreach (var source in _sources)
			{
				await source.Poll(db);
			}

			foreach (var transformer in _transformers)
			{
				foreach (var bankItem in db.GetItemsFromState(transformer.SourceState))
				{
					await transformer.Transform(bankItem);
				}
			}
			
			Thread.Sleep(10000);
		}

		// ReSharper disable once FunctionNeverReturns
	}
}