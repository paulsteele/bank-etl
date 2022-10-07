using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] 
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
	[Parameter("Name of database migration")]
	readonly string MigrationName;
	[PathExecutable("podman")] 
	readonly Tool Podman;

	[Solution] readonly Solution Solution;
	string RootProjectName = "etl";

	Target Clean => _ => _
		.Before(Restore).Executes(() =>
			{
				RootDirectory
					.GlobDirectories("**/bin", "**/obj")
					.Where(d => !d.ToString().Contains("Build"))
					.ForEach(DeleteDirectory);
			}
		);

	Target Restore => _ => _
		.Executes(() =>
			{
				DotNetRestore(s => s
					.SetProjectFile(Solution)
				);
			}
		);

	Target Compile => _ => _
		.DependsOn(Restore).Executes(() =>
			{
				DotNetBuild(s => s
					.SetProjectFile(Solution)
					.SetConfiguration(Configuration)
					.EnableNoRestore()
				);
			}
		);

	Target Run => _ => _
		.Executes(() =>
			{
				RootProjectName = "etl";
				DotNetRun(s => s
					.SetProjectFile(Solution.GetProject(RootProjectName))
					.SetConfiguration(Configuration)
					.EnableNoRestore()
				);
			}
		);

	Target ListMigrations => _ => _
		.Executes(() =>
			{
				EntityFrameworkTasks.EntityFrameworkMigrationsList(s => s
					.SetProject(Solution.GetProject(RootProjectName)));
			}
		);

	Target AddMigration => _ => _
		.Requires(() => MigrationName).Executes(() =>
			{
				EntityFrameworkTasks.EntityFrameworkMigrationsAdd(s => s
					.SetProject(Solution.GetProject(RootProjectName))
					.SetName(MigrationName)
				);
			}
		);

	Target CreateDevContainers => _ => _
		.Executes(() =>
			{
				Podman($"run -d --name {RootProjectName}-db -p 3306:3306 -e MYSQL_ROOT_PASSWORD=pass mysql --default-authentication-plugin=mysql_native_password");
			}
		);

	Target StartDevContainers => _ => _
		.Executes(() =>
			{
				Podman($"start {RootProjectName}-db");
			}
		);

	Target StopDevContainers => _ => _
		.Before(RemoveDevContainers).Executes(() =>
			{
				Podman($"stop {RootProjectName}-db");
			}
		);

	Target RemoveDevContainers => _ => _
		.Executes(() =>
			{
				Podman($"rm {RootProjectName}-db");
			}
		);

	public static int Main() => Execute<Build>(x => x.Compile);
}