// See https://aka.ms/new-console-template for more information

using Autofac;
using core;
using core.Db;
using Microsoft.Extensions.Logging;
using sqs;

EtlDependencyContainerBuilder.RegisterContainer();
DependencyContainer.Instance.Resolve<IDb>().Init();
var logger = DependencyContainer.Instance.Resolve<ILogger<Program>>();

var sources = DependencyContainer.Instance.Resolve<IEnumerable<ISource>>();
foreach (var source in sources)
{
	source.StartListening();
}

logger.LogInformation("Started ETL");
new CancellationToken().WaitHandle.WaitOne();