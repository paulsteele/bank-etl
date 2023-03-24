using Autofac;
using chase.transformers;
using core;
using core.Db;
using core.models;
using discord.transformers;
using firefly_iii.sources;
using firefly_iii.transformers;
using Microsoft.Extensions.Logging;
using ses.transformers;
using sqs.sources;

EtlDependencyContainerBuilder.RegisterContainer();

DependencyContainer.Instance.Resolve<IDb>().Init();

var bankItemFlow = new Flow<BankItem>(
	"BankItem",
	new SourceStep<BankItem>(DependencyContainer.Instance.Resolve<EmailQueue>(), "ReceivedFromSqs"),
	new List<FlowStep<BankItem>>
	{
		new(DependencyContainer.Instance.Resolve<SesBankItemTransformer>(), "ParsedFromSes"),
		new(DependencyContainer.Instance.Resolve<ChaseBankItemTransformer>(), "ParsedFromChase"),
		new(DependencyContainer.Instance.Resolve<ItemRequestTransformer>(), "WaitingForEmoji"),
		new(DependencyContainer.Instance.Resolve<ItemResponseTransformer>(), "ReceivedEmoji"),
		new(DependencyContainer.Instance.Resolve<TransactionTransformer>(), "SentToFirefly"),
		new(DependencyContainer.Instance.Resolve<ItemRemainingTransformer>(), "Done")
	},
	(db, s) => db.GetItemsFromState(s),
	DependencyContainer.Instance.Resolve<ILogger<Flow<BankItem>>>()
);

var categoryFlow = new Flow<Category>(
	"Category",
	new SourceStep<Category>(DependencyContainer.Instance.Resolve<FireflyCategorySource>(), "ReceivedFromFirefly"),
	new List<FlowStep<Category>>
	{
		new(DependencyContainer.Instance.Resolve<CategoryEmojiRequestTransformer>(), "WaitingForEmoji"),
		new(DependencyContainer.Instance.Resolve<CategoryEmojiResponseTransformer>(), "Setup")
	},
	(db, s) => db.GetCategoriesFromState(s),
	DependencyContainer.Instance.Resolve<ILogger<Flow<Category>>>()
);

var eventLoop = DependencyContainer.Instance.Resolve<EventLoop>().Start(new IFlow[]{bankItemFlow, categoryFlow});

DependencyContainer.Instance.Resolve<ILogger<Program>>().LogInformation("Started ETL");

await eventLoop;