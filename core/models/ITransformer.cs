using core.Db;

namespace core.models;

public interface ITransformer<in T>
{
	string SourceState { get; } 
	Task Transform(T item, IDb db);
}