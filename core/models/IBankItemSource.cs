using core.Db;

namespace core.models;

public interface IBankItemSource
{
	Task Poll(IDb database);
}