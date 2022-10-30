using core.Db;

namespace core.models;

public interface IBankItemTransformer
{
	string SourceState { get; } 
	Task Transform(BankItem item, IDb db);
}