// See https://aka.ms/new-console-template for more information

using Autofac;
using core;
using core.Database;

EtlDependencyContainerBuilder.RegisterContainer();
DependencyContainer.Instance.Resolve<IDb>().Init();