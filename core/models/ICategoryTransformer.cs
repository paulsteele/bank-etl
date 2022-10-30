using core.Db;

namespace core.models;

public interface ICategoryTransformer
{
	string SourceState { get; } 
	Task Transform(Category item, IDb db);
}