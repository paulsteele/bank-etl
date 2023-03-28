using chase.transformers;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;
using Moq;

namespace tests.Services.Chase;

public class ChaseTransformerTests
{
	private ChaseBankItemTransformer _transformer;
	private Mock<ILogger<ChaseBankItemTransformer>> _mockLogger;

	[SetUp]
	public void Setup()
	{
		_mockLogger = new Mock<ILogger<ChaseBankItemTransformer>>();

		_transformer = new ChaseBankItemTransformer(_mockLogger.Object, new ErrorHandler());
	}

	[Test]
	public async Task ShouldParseTransactionEmail()
	{
		var testItem = new BankItem
		{
			RawEmail = await File.ReadAllTextAsync("./Services/Chase/TestTransactionEmail.html")
		};

		var res = await _transformer.Transform(testItem, new Mock<IDb>().Object);
		Assert.Multiple(() =>
		{
			Assert.That(res.Is(TransformStatus.Success), Is.True);
			Assert.That(res.Result.Amount, Is.EqualTo(1234.56m));
			Assert.That(res.Result.Timestamp, Is.Not.Null);
			Assert.That(res.Result.Vendor, Is.EqualTo("SOMEBODY"));
		});
	}
}