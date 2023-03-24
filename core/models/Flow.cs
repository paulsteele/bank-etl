using core.Db;

namespace core.models;

public interface IFlow
{
	Task<TimeSpan> Execute(IDb database);
	string Name { get; }
}

public class Flow<T> : IFlow where T : class, IStateful
{
	public string Name { get; }
	private SourceStep<T> SourceStep { get; }
	private LinkedList<FlowStep<T>> FlowSteps { get; }
	private Func<IDb, string, T[]> GetItemsFromStateFunction { get; }

	public Flow(
		string name,
		SourceStep<T> sourceStep,
		IEnumerable<FlowStep<T>> flowSteps,
		Func<IDb, string, T[]> getItemsFromStateFunction
	)
	{
		Name = name;
		SourceStep = sourceStep;
		FlowSteps = new LinkedList<FlowStep<T>>(flowSteps);
		GetItemsFromStateFunction = getItemsFromStateFunction;
	}

	public async Task<TimeSpan> Execute(IDb database)
	{
		var nextRequestedPollTime = await SourceStep.Source.Poll(database, SourceStep.CompleteState);

		for (var node = FlowSteps.First; node != null; node = node.Next)
		{
			var state = node.Previous?.Value.CompleteState ?? SourceStep.CompleteState;
			
			foreach (var item in GetItemsFromStateFunction(database, state))
			{
				var res = await node.Value.Transformer.Transform(item, database);
				
				nextRequestedPollTime = nextRequestedPollTime < res.NextRequestedPoll
					? nextRequestedPollTime
					: res.NextRequestedPoll;

				if (res.Is(TransformStatus.Success))
				{
					res.Result.State = node.Value.CompleteState;
				}
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
