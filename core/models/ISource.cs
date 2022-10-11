using core.Db;

namespace core.models;

public interface ISource
{
	Task Poll(IDb database);
}