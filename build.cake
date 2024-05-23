#addin "nuget:?package=Cake.MinVer&version=3.0.0"
#load "CakeScripts/VersionParsers.cs"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// SET ERROR LEVELS
//////////////////////////////////////////////////////////////////////

var ErrorDetail = new List<string>();

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var version = MinVer(settings=> settings
    .WithAutoIncrement(MinVerAutoIncrement.Minor)
);

var packageVersion = version;

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = Argument("artifact-dir", PROJECT_DIR + "package") + "/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";
var IMAGE_DIR = PROJECT_DIR + "images/";
var NUNITFRAMEWORKTESTSBIN = PROJECT_DIR + "src/NUnitFramework/tests/bin/" + configuration + "/";
var NUNITFRAMEWORKLEGACYTESTSBIN = PROJECT_DIR + "src/NUnitFramework/nunit.framework.legacy.tests/bin/" + configuration + "/";
var NUNITLITETESTSBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite.tests/bin/" + configuration + "/";
var NUNITFRAMEWORKBIN = PROJECT_DIR + "src/NUnitFramework/framework/bin/" + configuration + "/";
var NUNITFRAMEWORKLEGACYBIN = PROJECT_DIR + "src/NUnitFramework/nunit.framework.legacy/bin/" + configuration + "/";
var NUNITLITEBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite/bin/" + configuration + "/";
var NUNITLITERUNNERBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite-runner/bin/" + configuration + "/";

var SOLUTION_FILE = "./nunit.sln";

var DIRECTORY_BUILD_PROPS = PROJECT_DIR + "src/NUnitFramework/Directory.Build.props";

// Test Runners
var NUNITLITE_RUNNER_DLL = "nunitlite-runner.dll";

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var FRAMEWORKLEGACY_TESTS = "nunit.framework.legacy.tests.dll";
var EXECUTABLE_NUNITLITE_TEST_RUNNER_EXE = "nunitlite-runner.exe";
var EXECUTABLE_NUNITLITE_TESTS_EXE = "nunitlite.tests.exe";
var EXECUTABLE_NUNITLITE_TESTS_DLL = "nunitlite.tests.dll";

// Packages
var ZIP_PACKAGE = PACKAGE_DIR + "NUnit.Framework-" + packageVersion + ".zip";

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

var LibraryFrameworks = XmlPeek(DIRECTORY_BUILD_PROPS, "/Project/PropertyGroup/NUnitLibraryFrameworks").Split(';');
var RuntimeFrameworks = XmlPeek(DIRECTORY_BUILD_PROPS, "/Project/PropertyGroup/NUnitRuntimeFrameworks").Split(';');

var NetCoreTestRuntimes = RuntimeFrameworks.Where(s => !s.StartsWith("net4")).ToArray();
var NetFrameworkTestRuntime = RuntimeFrameworks.Except(NetCoreTestRuntimes).Single();

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Building {0} version {1} of NUnit.", configuration, packageVersion);
});

//////////////////////////////////////////////////////////////////////
// VERSIONING
//////////////////////////////////////////////////////////////////////
Task("Version")
    .Does(context =>
{
    context.Information($"Version: {version.Version}");
    context.Information($"Major: {version.Major}");
    context.Information($"Minor: {version.Minor}");
    context.Information($"Patch: {version.Patch}");
    context.Information($"PreRelease: {version.PreRelease}");
    context.Information($"BuildMetadata: {version.BuildMetadata}");
});


//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Deletes all files in the BIN directories")
    .Does(() =>
    {
        CleanDirectory(NUNITFRAMEWORKBIN);
        CleanDirectory(NUNITFRAMEWORKLEGACYBIN);
        CleanDirectory(NUNITLITEBIN);
        CleanDirectory(NUNITLITERUNNERBIN);
    });

//////////////////////////////////////////////////////////////////////
// NUGET RESTORE
//////////////////////////////////////////////////////////////////////

Task("NuGetRestore")
    .Description("Restores NuGet Packages")
    .IsDependentOn("Version")
    .Does(() =>
    {
        DotNetRestore(SOLUTION_FILE);
    });

//////////////////////////////////////////////////////////////////////
// BUILD FRAMEWORKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Description("Builds the Solution")
    .IsDependentOn("NuGetRestore")
    .Does(() =>
    {
        DotNetBuild(SOLUTION_FILE, CreateDotNetBuildSettings());
    });

DotNetBuildSettings CreateDotNetBuildSettings() 
{
    var version = packageVersion.ToString(); 
    var assemblyVersion = version.Substring(0, version.LastIndexOf('-')) + ".0";
    var msBuildSettings = new DotNetMSBuildSettings {
        ContinuousIntegrationBuild = BuildSystem.GitHubActions.IsRunningOnGitHubActions,
        AssemblyVersion = VersionParsers.ParseAssemblyVersion(version),
        FileVersion = VersionParsers.ParseAssemblyVersion(version),
        InformationalVersion = version
    };
    Information("AssemblyVersion: {0}", msBuildSettings.AssemblyVersion);
    Information("FileVersion: {0}", msBuildSettings.FileVersion);
    Information("InformationalVersion: {0}", msBuildSettings.InformationalVersion);

    var settings =  new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        Verbosity = DotNetVerbosity.Minimal,
        MSBuildSettings = msBuildSettings
     };
    return settings;
}

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("CheckForError")
    .Description("Checks for errors running the test suites")
    .Does(() => CheckForError(ref ErrorDetail));

Task("TestNetFramework")
    .Description("Tests the .NET Framework version of nunit framework")
    .IsDependentOn("Build")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = NetFrameworkTestRuntime;
        var dir = NUNITFRAMEWORKTESTSBIN + runtime + "/";
        Information("Run tests for " + runtime + " in " + dir + "using runner");
        RunTest(dir + EXECUTABLE_NUNITLITE_TEST_RUNNER_EXE, dir, FRAMEWORK_TESTS, dir + "nunit.framework.tests.xml", runtime, ref ErrorDetail);
        dir = NUNITFRAMEWORKLEGACYTESTSBIN + runtime + "/";
        Information("Run legacy tests for " + runtime + " in " + dir + "using runner");
        RunTest(dir + EXECUTABLE_NUNITLITE_TEST_RUNNER_EXE, dir, FRAMEWORKLEGACY_TESTS, dir + "nunit.framework.legacy.tests.xml", runtime, ref ErrorDetail);
        dir = NUNITLITETESTSBIN + runtime + "/";
        Information("Run tests for " + runtime + " in " + dir + " for nunitlite.tests");
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS_EXE, dir, runtime, ref ErrorDetail);
        PublishTestResults(runtime);
    });

var testCore = Task("TestNetCore")
    .Description("Tests the .NET (6.0+) version of the framework");

foreach (var runtime in NetCoreTestRuntimes)
{
    var task = Task("TestNetCore on " + runtime)
        .Description("Tests the .NET (6.0+) version of the framework on " + runtime)
        .WithCriteria(IsRunningOnWindows() || !runtime.EndsWith("windows"))
        .IsDependentOn("Build")
        .OnError(exception => { ErrorDetail.Add(exception.Message); })
        .Does(() =>
        {
            var dir = NUNITFRAMEWORKTESTSBIN + runtime + "/";
            Information("Run tests for " + runtime + " in " + dir);
            RunDotnetCoreTests(dir + NUNITLITE_RUNNER_DLL, dir, FRAMEWORK_TESTS, runtime, GetResultXmlPath(FRAMEWORK_TESTS, runtime), ref ErrorDetail);
            dir = NUNITFRAMEWORKLEGACYTESTSBIN + runtime + "/";
            Information("Run legacy tests for " + runtime + " in " + dir);
            RunDotnetCoreTests(dir + NUNITLITE_RUNNER_DLL, dir, FRAMEWORKLEGACY_TESTS, runtime, GetResultXmlPath(FRAMEWORKLEGACY_TESTS, runtime), ref ErrorDetail);
            dir = NUNITLITETESTSBIN + runtime + "/";
            Information("Run tests for " + runtime + " in " + dir + " for nunitlite.tests");
            RunDotnetCoreTests(dir + EXECUTABLE_NUNITLITE_TESTS_DLL, dir, runtime, ref ErrorDetail);
            PublishTestResults(runtime);
        });

    testCore.IsDependentOn(task);
}

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

var RootFiles = new FilePath[]
{
    "LICENSE.txt",
    "NOTICES.md",
    "CHANGES.md",
    "README.md",
    "THIRD_PARTY_NOTICES.md"
};

// Not all of these are present in every framework
// The Microsoft and System assemblies are part of the BCL
// used by the .NET 4.0 framework. 4.0 tests will not run without them.
var FrameworkFiles = new FilePath[]
{
    "mock-assembly.dll",
    "mock-assembly.exe",
    "nunit.framework.dll",
    "nunit.framework.legacy.dll",
    "nunit.framework.pdb",
    "nunit.framework.legacy.pdb",
    "nunit.framework.xml",
    "nunit.framework.legacy.xml",
    "nunit.framework.tests.dll",
    "nunit.testdata.dll",
    "nunitlite.dll",
    "nunitlite.pdb",
    "nunitlite.tests.exe",
    "nunitlite.tests.dll",
    "slow-nunit-tests.dll",
    "nunitlite-runner.exe",
    "nunitlite-runner.pdb",
    "nunitlite-runner.dll",
    "Microsoft.Threading.Tasks.dll",
    "Microsoft.Threading.Tasks.Extensions.Desktop.dll",
    "Microsoft.Threading.Tasks.Extensions.dll",
    "System.IO.dll",
    "System.Runtime.dll",
    "System.Threading.Tasks.dll",
    "System.ValueTuple.dll"
};

string CurrentImageDir => $"{IMAGE_DIR}NUnit-{packageVersion}/";

Task("CreateImage")
    .Description("Copies all files into the image directory")
    .Does(() =>
    {
        CleanDirectory(CurrentImageDir);
        CopyFiles(RootFiles, CurrentImageDir);

        var imageBinDir = Directory(CurrentImageDir) + Directory("bin");

        CreateDirectory(imageBinDir);
        Information("Created imagedirectory at:" + imageBinDir.ToString());
        var directories = new String[]
        {
            NUNITFRAMEWORKBIN,
            NUNITFRAMEWORKLEGACYBIN,
            NUNITLITEBIN
        };
        foreach (var dir in directories)
        {
            foreach (var runtime in LibraryFrameworks)
            {
                var targetDir = imageBinDir + Directory(runtime);
                var sourceDir = Directory(dir) + Directory(runtime);
                CreateDirectory(targetDir);
                Information("Created directory " + targetDir.ToString());
                foreach (FilePath file in FrameworkFiles)
                {
                    var sourcePath = sourceDir + File(file.FullPath);
                    if (FileExists(sourcePath))
                        CopyFileToDirectory(sourcePath, targetDir);
                }
                Information("Files copied from " + sourceDir.ToString() + " to " + targetDir.ToString());
                var schemaPath = sourceDir + Directory("Schemas");
                if (DirectoryExists(schemaPath))
                {
                    CopyDirectory(sourceDir, targetDir);
                }
            }
        }    
        Information("Finished copying framework files");
        foreach (var dir in RuntimeFrameworks)
        {
            var targetDir = imageBinDir + Directory(dir);
            var sourceDir = NUNITLITERUNNERBIN + Directory(dir);
            Information("Copying " + sourceDir.ToString() + " to " + targetDir.ToString());
            CopyDirectory(sourceDir, targetDir);
        }
    });

Task("PackageFramework")
    .Description("Creates NuGet packages of the framework")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);

        var settings = new NuGetPackSettings
        {
            Version = packageVersion,
            BasePath = CurrentImageDir,
            OutputDirectory = PACKAGE_DIR,
            Symbols = true,
            // snupkg is not yet supported by Cake, https://github.com/cake-build/cake/issues/2362
            ArgumentCustomization = args => args.Append("-SymbolPackageFormat snupkg")
        };

        NuGetPack("nuget/framework/nunit.nuspec", settings);
        NuGetPack("nuget/nunitlite/nunitlite.nuspec", settings);
    });

Task("PackageZip")
    .Description("Creates a ZIP file of the framework")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);

        var zipFiles = GetFiles(CurrentImageDir + "*.*");
        foreach (var framework in LibraryFrameworks)
            zipFiles += GetFiles(CurrentImageDir + "bin/"+ framework + "/**/*.*");
        Zip(CurrentImageDir, File(ZIP_PACKAGE), zipFiles);
    });

Task("SignPackages")
    .Description("Signs the NuGet packages")
    .IsDependentOn("PackageFramework")
    .Does(() =>
    {
        // Get the secret.
        var secret = EnvironmentVariable("SIGNING_SECRET");
        if(string.IsNullOrWhiteSpace(secret)) {
            throw new InvalidOperationException("Could not resolve signing secret.");
        }

        // Get the user.
        var user = EnvironmentVariable("SIGNING_USER");
        if(string.IsNullOrWhiteSpace(user)) {
            throw new InvalidOperationException("Could not resolve signing user.");
        }

        var signClientPath = Context.Tools.Resolve("SignClient.exe") ?? Context.Tools.Resolve("SignClient") ?? throw new Exception("Failed to locate sign tool");

        var settings = File("./signclient.json");

        // Get the files to sign.
        var files = GetFiles(string.Concat(PACKAGE_DIR, "*.*nupkg"));

        foreach(var file in files)
        {
            Information("Signing {0}...", file.FullPath);

            // Build the argument list.
            var arguments = new ProcessArgumentBuilder()
                .Append("sign")
                .AppendSwitchQuoted("-c", MakeAbsolute(settings.Path).FullPath)
                .AppendSwitchQuoted("-i", MakeAbsolute(file).FullPath)
                .AppendSwitchQuotedSecret("-s", secret)
                .AppendSwitchQuotedSecret("-r", user)
                .AppendSwitchQuoted("-n", "NUnit.org")
                .AppendSwitchQuoted("-d", "NUnit is a unit-testing framework for all .NET languages.")
                .AppendSwitchQuoted("-u", "https://nunit.org/");

            // Sign the binary.
            var result = StartProcess(signClientPath.FullPath, new ProcessSettings {  Arguments = arguments });
            if(result != 0)
            {
                // We should not recover from this.
                throw new InvalidOperationException("Signing failed!");
            }
        }
    });

//////////////////////////////////////////////////////////////////////
// SETUP AND TEARDOWN TASKS
//////////////////////////////////////////////////////////////////////

Teardown(context => CheckForError(ref ErrorDetail));

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - GENERAL
//////////////////////////////////////////////////////////////////////

void CheckForError(ref List<string> errorDetail)
{
    if(errorDetail.Count != 0)
    {
        var copyError = new List<string>();
        copyError = errorDetail.Select(s => s).ToList();
        errorDetail.Clear();
        throw new Exception("One or more unit tests failed, breaking the build.\n"
                              + copyError.Aggregate((x,y) => x + "\n" + y));
    }
}

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - TEST
//////////////////////////////////////////////////////////////////////

FilePath GetResultXmlPath(string testAssembly, string framework)
{
    var assemblyName = System.IO.Path.GetFileNameWithoutExtension(testAssembly);

    CreateDirectory($@"test-results\{framework}");

    return MakeAbsolute(new FilePath($@"test-results\{framework}\{assemblyName}.xml"));
}

void RunNUnitTests(DirectoryPath workingDir, string testAssembly, string framework, ref List<string> errorDetail)
{
    try
    {
        var path = workingDir.CombineWithFilePath(testAssembly);

        var settings = new NUnit3Settings();
        settings.Results = new[] { new NUnit3Result { FileName = GetResultXmlPath(testAssembly, framework) } };

        if (!IsRunningOnWindows())
            settings.Process = NUnit3ProcessOption.InProcess;

        NUnit3(path.ToString(), settings);
    }
    catch(CakeException ce)
    {
        errorDetail.Add(string.Format("{0}: {1}", framework, ce.Message));
    }
}

void RunTest(FilePath exePath, DirectoryPath workingDir, string framework, ref List<string> errorDetail)
{
    RunTest(exePath, workingDir, null, GetResultXmlPath(exePath.FullPath, framework), framework, ref errorDetail);
}

void RunTest(FilePath exePath, DirectoryPath workingDir, string arguments, FilePath resultFile, string framework, ref List<string> errorDetail)
{
    int rc = StartProcess(
        MakeAbsolute(exePath),
        new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(arguments)
                .AppendSwitchQuoted("--result", ":", resultFile.FullPath)
                .Render(),
            WorkingDirectory = workingDir
        });

    if (rc > 0)
        errorDetail.Add(string.Format("{0}: {1} tests failed", framework, rc));
    else if (rc < 0)
        errorDetail.Add(string.Format("{0} returned rc = {1}", exePath, rc));
}

void RunDotnetCoreTests(FilePath exePath, DirectoryPath workingDir, string framework, ref List<string> errorDetail)
{
    RunDotnetCoreTests(exePath, workingDir, null, framework, GetResultXmlPath(exePath.FullPath, framework), ref errorDetail);
}

void RunDotnetCoreTests(FilePath exePath, DirectoryPath workingDir, string arguments, string framework, FilePath resultFile, ref List<string> errorDetail)
{
    if (!FileExists(exePath))
    {
        Information(string.Format("{0}: {1} not found", framework, exePath));
        return;
    }

    int rc = StartProcess(
        "dotnet",
        new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .AppendQuoted(exePath.FullPath)
                .Append(arguments)
                .AppendSwitchQuoted("--result", ":", resultFile.FullPath)
                .Render(),
            WorkingDirectory = workingDir
        });

    if (rc > 0)
        errorDetail.Add(string.Format("{0}: {1} tests failed", framework, rc));
    else if (rc < 0)
        errorDetail.Add(string.Format("{0} returned rc = {1}", exePath, rc));
}

void PublishTestResults(string framework)
{
    if (EnvironmentVariable("TF_BUILD", false))
    {
        Information("Publishing test results to Azure Pipelines");
        var fullTestRunTitle = framework;
        var ciRunName = Argument<string>("test-run-name");
        if (!string.IsNullOrEmpty(ciRunName))
            fullTestRunTitle += '/' + ciRunName;

        AzurePipelines.Commands.PublishTestResults(new AzurePipelinesPublishTestResultsData
        {
            TestResultsFiles = GetFiles($@"test-results\{framework}\*.xml").ToList(),
            TestRunTitle = fullTestRunTitle,
            TestRunner = AzurePipelinesTestRunnerType.NUnit,
            MergeTestResults = true,
            PublishRunAttachments = true,
            Configuration = configuration
        });
    }
}

public static T WithRawArgument<T>(this T settings, string rawArgument) where T : Cake.Core.Tooling.ToolSettings
{
    if (settings == null) throw new ArgumentNullException(nameof(settings));

    if (!string.IsNullOrEmpty(rawArgument))
    {
        var previousCustomizer = settings.ArgumentCustomization;
        if (previousCustomizer != null)
            settings.ArgumentCustomization = builder => previousCustomizer.Invoke(builder).Append(rawArgument);
        else
            settings.ArgumentCustomization = builder => builder.Append(rawArgument);
    }

    return settings;
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
    .Description("Rebuilds all versions of the framework")
    .IsDependentOn("Clean")
    .IsDependentOn("Build");

Task("Test")
    .Description("Builds and tests all versions of the framework")
    .IsDependentOn("Build")
    .IsDependentOn("TestNetFramework")
    .IsDependentOn("TestNetCore");

Task("Package")
    .Description("Packages all versions of the framework")
    .IsDependentOn("CheckForError")
    .IsDependentOn("PackageFramework")
    .IsDependentOn("PackageZip");



Task("Default")
    .Description("Builds all versions of the framework")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
