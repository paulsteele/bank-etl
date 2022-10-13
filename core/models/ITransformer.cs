namespace core.models;

public interface ITransformer
{
	string SourceState { get; } 
	Task Transform(IEnumerable<BankItem> items);
}