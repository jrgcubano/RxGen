#addin "Cake.FileHelpers"
// #addin "nuget:?package=Cake.Coveralls"
// #addin "nuget:?package=Cake.PinNuGetDependency"
// #tool "nuget:?package=OpenCover"
// #tool "nuget:?package=ReportGenerator"
#tool "nuget:?package=xunit.runner.console"
// #tool "nuget:?package=coveralls.io"
#tool "GitReleaseManager"
#tool "GitVersion.CommandLine"

var target = Argument<string>("target", "Default");

var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isLocalBuild = BuildSystem.IsLocalBuild;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

var isRepository = StringComparer.OrdinalIgnoreCase.Equals("jrgcubano/rxgen", AppVeyor.Environment.Repository.Name);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

var githubOwner = "jrgcubano";
var githubRepository = "rxgen";
var githubUrl = string.Format("https://github.com/{0}/{1}", githubOwner, githubRepository);

var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") :
    "Release";

// var coverallsApiKey = EnvironmentVariable("COVERALLS_API_KEY");

var artifactsDir = "./artifacts";
var artifactsDirectory = Directory(artifactsDir);
var nugetDir = artifactsDir + "/nuget";
var srcProjects = new [] { "RxGen" };
var testProjects = new [] { "RxGen.Tests", "RxGen.AcceptanceTests" };
var packageWhitelist = srcProjects;

var gitVersion = GitVersion();
var majorMinorPatch = gitVersion.MajorMinorPatch;
var informationalVersion = gitVersion.InformationalVersion;
var nugetVersion = gitVersion.NuGetVersion;
var buildVersion = gitVersion.FullBuildMetaData;

var testResultsDir = artifactsDir + "/test-results";
var testCoverageFilePath = testResultsDir + "/coverage.xml";
var testCoverageFilter = "+[RxGen*]* -[xunit.*]* -[FluentAssertions*]* -[*Tests]* ";
var testCoverageExcludeByAttribute = "*.ExcludeFromCodeCoverage*";
var testCoverageExcludeByFile = "*/*Designer.cs;*/*AssemblyInfo.cs;*/*.g.cs;*/*.g.i.cs";

var msBuildSettings = new DotNetCoreMSBuildSettings()
    .WithProperty("Version", nugetVersion);
    // .WithProperty("AssemblyVersion", semVersion)
    // .WithProperty("FileVersion", semVersion);


Setup((context) =>
{
    Information("Building RxGen");
    Information("IsTagged: {0}", isTagged);
    Information("IsPullRequest: {0}", isPullRequest);
    Information("IsLocalBuild: {0}", isLocalBuild);
    Information("IsRepository: {0}", isRepository);
    Information("IsReleaseBranch: {0}", isReleaseBranch);
    Information("InformationalVersion: {0}", informationalVersion);
    Information("NugetVersion: {0}", nugetVersion);
});

Teardown((context) =>
{
});

Task("UpdateAppVeyorBuildNumber")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
    {
        AppVeyor.UpdateBuildVersion(buildVersion);
    })
    .ReportError(exception =>
    {
        // Via: See https://github.com/reactiveui/ReactiveUI/issues/1262
        Warning("Build with version {0} already exists.", buildVersion);
    });


Task("Clean")
	.Does(() =>
    {
        CleanDirectories(artifactsDir);
        CleanProjects("src", srcProjects);
        CleanProjects("test", testProjects);
        EnsureDirectoryExists(artifactsDir);
        EnsureDirectoryExists(testResultsDir);
        EnsureDirectoryExists(nugetDir);
	});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore("./RxGen.sln", new DotNetCoreRestoreSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal,
            Sources = new [] {
                "https://www.myget.org/F/xunit/api/v3/index.json",
                "https://dotnet.myget.org/F/dotnet-core/api/v3/index.json",
                "https://dotnet.myget.org/F/cli-deps/api/v3/index.json",
                "https://api.nuget.org/v3/index.json",
            }
        });
    });

 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        BuildProjects("src", srcProjects, configuration, msBuildSettings);
        BuildProjects("test", testProjects, configuration, msBuildSettings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var artifactsAsDir = Directory(artifactsDir);
        foreach(var project in GetFiles("./test/**/*.csproj"))
        {
            var outputFilePath = MakeAbsolute(artifactsAsDir.Path)
                .CombineWithFilePath(project.GetFilenameWithoutExtension());
            var arguments = new ProcessArgumentBuilder()
                .AppendSwitch("-configuration", configuration)
                .AppendSwitchQuoted("-xml", outputFilePath.AppendExtension(".xml").ToString())
                .AppendSwitchQuoted("-html", outputFilePath.AppendExtension(".html").ToString());

            DotNetCoreTool(project, "xunit", arguments);

            // Action<ICakeContext> testAction = tool =>
            // {
            //     tool.DotNetCoreTool(project, "xunit", arguments);
            // };

            // OpenCover(testAction, testCoverageFilePath, new OpenCoverSettings
            // {
            //     MergeOutput = true,
            //     Register = "user",
            //     SkipAutoProps = true,
            //     OldStyle = true,
            //     ReturnTargetCodeOffset = 0
            //     ArgumentCustomization = args => args.Append("-hideskipped:all")
            // }
            // .WithFilter(testCoverageFilter)
            // .ExcludeByAttribute(testCoverageExcludeByAttribute)
            // .ExcludeByFile(testCoverageExcludeByFile));

            // if (FileExists(testCoverageFilePath))
            // {
            //     ReportGenerator(testCoverageFilePath, testResultsDir);
            // }
        }
    });

// Task("Upload-Coverage-Report")
//     .WithCriteria(() => FileExists(coverageFilePath))
//     .WithCriteria(() => !isLocalBuild)
//     .WithCriteria(() => !isPullRequest)
//     .IsDependentOn("Test-Coverage")
//     .Does(() =>
// {
//     CoverallsIo(coverageFilePath, new CoverallsIoSettings()
//     {
//         RepoToken = coverallsApiKey
//     });
// });


Task("PackageNuget")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
    {
        foreach (var project in srcProjects)
        {
            var projectPath = File(string.Format("./src/{0}/{0}.csproj", project));
            DotNetCorePack(projectPath, new DotNetCorePackSettings()
            {
                Configuration = configuration,
                OutputDirectory = nugetDir,
                MSBuildSettings = msBuildSettings
            });
        }
    });

// Task("PinNugetDependencies")
//     .Does (() =>
//     {
//         // only pin whitelisted packages.
//         foreach(var package in packageWhitelist)
//         {
//             // only pin the package which was created during this build run.
//             var packagePath = $"{nugetDir}/" + File($"{package}.{nugetVersion}.nupkg");
//             PinNuGetDependency(packagePath, "RxGen");
//         }
//     });

Task("Package")
    .IsDependentOn("PackageNuget")
    //.IsDependentOn("PinNugetDependencies")
    .Does(() =>
    {

    });

Task("CreateRelease")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isReleaseBranch)
    .WithCriteria(() => !isTagged)
    .Does (() =>
    {
        var username = EnvironmentVariable("GITHUB_USERNAME");
        if (string.IsNullOrEmpty(username))
            throw new Exception("The GITHUB_USERNAME environment variable is not defined.");

        var token = EnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(token))
            throw new Exception("The GITHUB_TOKEN environment variable is not defined.");

        GitReleaseManagerCreate(username, token, githubOwner, githubRepository, new GitReleaseManagerCreateSettings {
            Milestone         = majorMinorPatch,
            Name              = majorMinorPatch,
            Prerelease        = true,
            TargetCommitish   = "master"
        });
    });

Task("PublishPackages")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isReleaseBranch)
    .Does(() =>
    {

        if (isReleaseBranch && !isTagged)
        {
            Information("Packages will not be published as this release has not been tagged.");
            return;
        }

        var apiKey = EnvironmentVariable("NUGET_APIKEY");
        if (string.IsNullOrEmpty(apiKey))
            throw new Exception("The NUGET_APIKEY environment variable is not defined.");

        var source = EnvironmentVariable("NUGET_SOURCE");
        if (string.IsNullOrEmpty(source))
            throw new Exception("The NUGET_SOURCE environment variable is not defined.");

        // only push whitelisted packages.
        foreach(var package in packageWhitelist)
        {
            // only push the package which was created during this build run.
            var packagePath = nugetDir + "/" + File(string.Format("{0}.{1}.nupkg", package, nugetVersion));

            NuGetPush(packagePath, new NuGetPushSettings {
                Source = source,
                ApiKey = apiKey
            });
        }
   });

Task("PublishRelease")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isRepository)
    .WithCriteria(() => isReleaseBranch)
    .WithCriteria(() => isTagged)
    .Does (() =>
    {
        var username = EnvironmentVariable("GITHUB_USERNAME");
        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
        }

        var token = EnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
        }

        // only push whitelisted packages.
        foreach(var package in packageWhitelist)
        {
            // only push the package which was created during this build run.
            var packagePath = nugetDir + "/" + File(string.Format("{0}.{1}.nupkg", package, nugetVersion));
            GitReleaseManagerAddAssets(username, token, githubOwner, githubRepository, majorMinorPatch, packagePath);
        }

        GitReleaseManagerClose(username, token, githubOwner, githubRepository, majorMinorPatch);
    });

Task("Default")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .IsDependentOn("CreateRelease")
    .IsDependentOn("PublishPackages")
    .IsDependentOn("PublishRelease")
    .Does (() =>
    {
    });

Task("Local")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

RunTarget(target);

private void CleanProjects(string projectKind, IEnumerable<string> projectNames)
{
    foreach(var project in projectNames)
    {
        CleanDirectories(string.Format("./{0}/{1}/bin/**", projectKind, project));
        CleanDirectories(string.Format("./{0}/{1}/obj/**", projectKind, project));
    }
}

private void BuildProjects(
    string projectKind, IEnumerable<string> projectNames,
    string configuration, DotNetCoreMSBuildSettings msBuildSettings)
{
    foreach(var project in projectNames)
    {
        var projectPath = File(string.Format("./{0}/{1}/{1}.csproj", projectKind, project));
        DotNetCoreBuild(projectPath, new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            MSBuildSettings = msBuildSettings
        });
    }
}