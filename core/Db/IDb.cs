using core.models;

namespace core.Db;

public interface IDb {
	void Init();
	BankItem UpsertBankItem(BankItem item);
	void SaveChanges();
}