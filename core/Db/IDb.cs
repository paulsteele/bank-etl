using core.models;

namespace core.Db;

public interface IDb {
	void Init();
	BankItem? AddItem(BankItem item);
	IEnumerable<BankItem> GetItemsFromState(string state);
	void SaveChanges();
}