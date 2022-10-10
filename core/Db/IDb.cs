using core.models;

namespace core.Db;

public interface IDb {
	void Init();
	BankItem? AddItem(BankItem item);
	void SaveChanges();
}