using core.Db;

namespace core.models;

public interface ISource<T> 
{
	Task<TimeSpan> Poll(IDb database, string successState);
}