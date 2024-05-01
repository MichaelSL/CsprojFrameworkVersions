using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(PublishBinaries) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Empty);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Empty => _ => _
        .Executes(() =>
        {
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

    Target PublishBinaries => _ => _
        .Produces(RootDirectory / "artifacts/CsprojFrameworkVersions*")
        .Executes(() =>
        {
            AbsolutePath OutputDirectory = RootDirectory / "artifacts";
            OutputDirectory.CreateOrCleanDirectory();

            var platforms = new[] { "win-x64", "linux-x64", "osx-x64" };
            foreach (var platform in platforms)
            {

                DotNetTasks.DotNetPublish(s => s
                    .SetProject("CsprojFrameworkVersions/CsprojFrameworkVersions.csproj")
                    .SetConfiguration(Configuration)
                    .SetRuntime(platform)
                    .SetSelfContained(true)
                    .SetPublishSingleFile(true)
                    .SetOutput($"artifacts/{platform}"));
            }
        });
}
