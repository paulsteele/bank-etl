using core.Db;
using core.models;

namespace firefly_iii;

public class TransactionTransformer : IBankItemTransformer
{
	private readonly FireflyClient _client;

	public TransactionTransformer(
		FireflyClient client
	)
	{
		_client = client;
	}

	public string SourceState => "ReadyForUpload";
	public async Task Transform(BankItem item, IDb db)
	{
		db.LoadForeignKeys(item);
		await _client.SendTransaction(item);
	}
}