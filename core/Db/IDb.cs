using core.models;

namespace core.Db;

public interface IDb {
	void Init();
	BankItem? AddItem(BankItem item);
	BankItem[] GetItemsFromState(string state);
	Category? AddCategory(Category category);
	Category[] GetCategoriesFromState(string state);
	Category[] GetAllCategories();
	void LoadForeignKeys(BankItem item);
	void SaveChanges();
}