using core.Db;

namespace core.models;

public interface ITransformer<T> where T : class, IStateful
{
	Task<TransformResult<T>> Transform(T item, IDb db);
}

public class TransformResult<T> where T : class, IStateful
{
	public TransformStatus Status { get; }
	public TimeSpan NextRequestedPoll { get; }
	public T Result { get; }

	public TransformResult(T result, TransformStatus status, TimeSpan nextRequestedPoll)
	{
		Result = result;
		Status = status;
		NextRequestedPoll = nextRequestedPoll;
	}
}

public enum TransformStatus {
	Success,
	Failure
}

public static class TransformStatusExtension
{
	public static bool Is(this TransformStatus status, TransformStatus expectedStatus) => status == expectedStatus;
	public static bool Is<T>(this TransformResult<T> result, TransformStatus expectedStatus) where T : class, IStateful => result.Status == expectedStatus;

	public static TransformResult<T> ToSuccessResult<T>(this T item, TimeSpan nextRequestedPollTime)
		where T : class, IStateful => new(item, TransformStatus.Success, nextRequestedPollTime);
	
	public static TransformResult<T> DefaultFailureResult<T>(this T item)
		where T : class, IStateful => new(item, TransformStatus.Failure, TimeSpan.FromSeconds(30));
}