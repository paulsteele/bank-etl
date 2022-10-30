using System.ComponentModel;
using Autofac;
using core.Db;
using core.models;
using firefly_iii;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using sqs;

namespace core;

public class EventLoop
{
	private readonly IEnumerable<IBankItemSource> _bankItemSources;
	private readonly IEnumerable<IBankItemTransformer> _bankItemTransformers;
	private readonly IEnumerable<ICategorySource> _categorySources;
	private readonly IEnumerable<ICategoryTransformer> _categoryTransformers;
	private readonly ILogger<EventLoop> _logger;
	private readonly ILifetimeScope _lifetimeScope;

	public EventLoop(
		IEnumerable<IBankItemSource> bankItemSources, 
		IEnumerable<IBankItemTransformer> bankItemTransformers,
		IEnumerable<ICategorySource> categorySources,
		IEnumerable<ICategoryTransformer> categoryTransformers,
		ILogger<EventLoop> logger,
		ILifetimeScope lifetimeScope)
	{
		_bankItemSources = bankItemSources;
		_bankItemTransformers = bankItemTransformers;
		_categorySources = categorySources;
		_categoryTransformers = categoryTransformers;
		_logger = logger;
		_lifetimeScope = lifetimeScope;
	}

	const int TenSeconds = 10000;
	const int OneDay = 1000 * 60 * 60 * 24;

	public void Start()
	{
		Task.Run(RunWithErrorLogging(SyncBankItems));
		Task.Run(SyncCategories);
	}

	private Action RunWithErrorLogging(Func<Task> action)
	{
		// ReSharper disable once AsyncVoidLambda
		return async () =>
		{
			try
			{
				await action();
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				_logger.LogError(e.StackTrace);
			}
		};
	}

	private async Task SyncCategories()
	{
		while (true)
		{
			var fastPoll = false;
			
			await using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var db = scope.Resolve<IDb>();
				foreach (var source in _categorySources)
				{
					await source.Poll(db);
					fastPoll = true;
				}

				foreach (var transformer in _categoryTransformers)
				{
					foreach (var category in db.GetCategoriesFromState(transformer.SourceState))
					{
						await transformer.Transform(category, db);
						fastPoll = true;
					}
				}

				db.SaveChanges();
			}

			if (!fastPoll)
			{
				_logger.LogInformation("No categories to sync. Slow polling.");
			}
			Thread.Sleep(fastPoll ? TenSeconds : OneDay);
		}
		
		// ReSharper disable once FunctionNeverReturns
	}
	
	private async Task SyncBankItems()
	{
		while (true)
		{
			await using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var db = scope.Resolve<IDb>();
				foreach (var source in _bankItemSources)
				{
					await source.Poll(db);
				}

				foreach (var transformer in _bankItemTransformers)
				{
					foreach (var bankItem in db.GetItemsFromState(transformer.SourceState))
					{
						await transformer.Transform(bankItem, db);
					}
				}
				db.SaveChanges();
			}
			
			Thread.Sleep(TenSeconds);
		}

		// ReSharper disable once FunctionNeverReturns
	}
}