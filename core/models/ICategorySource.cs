using core.Db;

namespace core.models;

public interface ICategorySource
{
	Task Poll(IDb database);
}