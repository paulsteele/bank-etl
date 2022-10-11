// See https://aka.ms/new-console-template for more information
using Autofac;
using core;
using core.Db;
using Microsoft.Extensions.Logging;

EtlDependencyContainerBuilder.RegisterContainer();

DependencyContainer.Instance.Resolve<IDb>().Init();

DependencyContainer.Instance.Resolve<EventLoop>().Start();

DependencyContainer.Instance.Resolve<ILogger<Program>>().LogInformation("Started ETL");
new CancellationToken().WaitHandle.WaitOne();