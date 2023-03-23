using core.Db;

namespace core.models;

public interface IFlow
{
	Task Execute(IDb database);
}

public class Flow<T> : IFlow
{
	private ISource<T> Source { get; }
	private IList<ITransformer<T>> Transformers { get; }
	private Func<IDb, string, T[]> GetItemsFromStateFunction { get; }

	public Flow(
		ISource<T> source,
		IList<ITransformer<T>> transformers,
		Func<IDb, string, T[]> getItemsFromStateFunction
	)
	{
		Source = source;
		Transformers = transformers;
		GetItemsFromStateFunction = getItemsFromStateFunction;
	}

	public async Task Execute(IDb database)
	{
		await Source.Poll(database);
		foreach (var transformer in Transformers)
		{
			foreach (var item in GetItemsFromStateFunction(database, transformer.SourceState))
			{
				await transformer.Transform(item, database);
			}
		}
	}
}