// See https://aka.ms/new-console-template for more information

using Autofac;
using core;
using core.Database;

DependencyContainer.Instance.Resolve<IDb>().Init();