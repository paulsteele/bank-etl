// See https://aka.ms/new-console-template for more information
using Autofac;
using chase;
using core;
using core.Db;
using core.models;
using discord;
using firefly_iii;
using Microsoft.Extensions.Logging;
using ses;
using sqs;

EtlDependencyContainerBuilder.RegisterContainer();

DependencyContainer.Instance.Resolve<IDb>().Init();

var bankItemFlow = new Flow<BankItem>(
	DependencyContainer.Instance.Resolve<EmailQueue>(),
	new List<ITransformer<BankItem>>
	{
		DependencyContainer.Instance.Resolve<SesBankItemTransformer>(),
		DependencyContainer.Instance.Resolve<ChaseBankItemTransformer>(),
		DependencyContainer.Instance.Resolve<ItemRequestTransformer>(),
		DependencyContainer.Instance.Resolve<ItemResponseTransformer>(),
		DependencyContainer.Instance.Resolve<TransactionTransformer>(),
		DependencyContainer.Instance.Resolve<ItemRemainingTransformer>()
	},
	(db, s) => db.GetItemsFromState(s)
);

var categoryFlow = new Flow<Category>(
	DependencyContainer.Instance.Resolve<FireflyCategorySource>(),
	new List<ITransformer<Category>>
	{
		DependencyContainer.Instance.Resolve<CategoryEmojiRequestTransformer>(),
		DependencyContainer.Instance.Resolve<CategoryEmojiResponseTransformer>()
	},
	(db, s) => db.GetCategoriesFromState(s)
);

DependencyContainer.Instance.Resolve<EventLoop>().Start(new IFlow[]{bankItemFlow, categoryFlow});

DependencyContainer.Instance.Resolve<ILogger<Program>>().LogInformation("Started ETL");
new CancellationToken().WaitHandle.WaitOne();