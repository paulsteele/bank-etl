using core.Db;
using Microsoft.Extensions.Logging;

namespace core.models;

public interface IFlow
{
	Task<TimeSpan> Execute(IDb database);
	string Name { get; }
}

public class Flow<T> : IFlow where T : class, IStateful
{
	private readonly ILogger<Flow<T>> _logger;
	public string Name { get; }
	private SourceStep<T> SourceStep { get; }
	private LinkedList<FlowStep<T>> FlowSteps { get; }
	private Func<IDb, string, T[]> GetItemsFromStateFunction { get; }

	public Flow(
		string name,
		SourceStep<T> sourceStep,
		IEnumerable<FlowStep<T>> flowSteps,
		Func<IDb, string, T[]> getItemsFromStateFunction,
		ILogger<Flow<T>> logger
	)
	{
		Name = name;
		SourceStep = sourceStep;
		FlowSteps = new LinkedList<FlowStep<T>>(flowSteps);
		GetItemsFromStateFunction = getItemsFromStateFunction;
		_logger = logger;
	}

	public async Task<TimeSpan> Execute(IDb database)
	{
		var nextRequestedPollTime = await SourceStep.Source.Poll(database, SourceStep.CompleteState);

		for (var node = FlowSteps.First; node != null; node = node.Next)
		{
			var state = node.Previous?.Value.CompleteState ?? SourceStep.CompleteState;

			var shouldSave = false;
			foreach (var item in GetItemsFromStateFunction(database, state))
			{
				var res = await node.Value.Transformer.Transform(item, database);
				
				nextRequestedPollTime = nextRequestedPollTime < res.NextRequestedPoll
					? nextRequestedPollTime
					: res.NextRequestedPoll;

				if (res.Is(TransformStatus.Success))
				{
					shouldSave = true;
					res.Result.State = node.Value.CompleteState;
				}
			}

			if (shouldSave)
			{
				_logger.LogInformation($"Flow {Name} has changes. Saving");
				database.SaveChanges();
			}
		}

		return nextRequestedPollTime;
	}
}

public class FlowStep<T> where T : class, IStateful {
	public ITransformer<T> Transformer { get; }
	public string CompleteState { get; }

	public FlowStep(ITransformer<T> transformer, string completeState)
	{
		Transformer= transformer;
		CompleteState = completeState;
	}
}

public class SourceStep<T> where T : class, IStateful {
	public ISource<T> Source { get; }
	public string CompleteState { get; }

	public SourceStep(ISource<T> source, string completeState)
	{
		Source = source;
		CompleteState = completeState;
	}
}
