using core.Db;

namespace core.models;

public interface ISource<T> 
{
	Task Poll(IDb database, string successState);
}