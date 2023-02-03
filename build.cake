#tool NUnit.ConsoleRunner&version=3.12.0
#addin nuget:?package=Cake.Coverlet

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

var version = "4.0.0";
var modifier = "-alpha-1";

var dbgSuffix = configuration == "Debug" ? "-dbg" : "";
var packageVersion = version + modifier + dbgSuffix;

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

// Equivalent of NUnitLibraryFrameworks in Directory.Build.props
var LibraryFrameworks = new string[]
{
    "net462",
    "netstandard2.0"
};

// Subset of NUnitRuntimeFrameworks in Directory.Build.props
var NetCoreTests = new String[]
{
    "netcoreapp3.1",
    "net5.0",
    "net6.0",
    "net7.0"
};

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = Argument("artifact-dir", PROJECT_DIR + "package") + "/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";
var IMAGE_DIR = PROJECT_DIR + "images/";

var SOLUTION_FILE = "./nunit.sln";

// Test Runners
var NUNITLITE_RUNNER_DLL = "nunitlite-runner.dll";

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var EXECUTABLE_NUNITLITE_TEST_RUNNER_EXE = "nunitlite-runner.exe";
var EXECUTABLE_NUNITLITE_TESTS_EXE = "nunitlite.tests.exe";
var EXECUTABLE_NUNITLITE_TESTS_DLL = "nunitlite.tests.dll";

// Packages
var ZIP_PACKAGE = PACKAGE_DIR + "NUnit.Framework-" + packageVersion + ".zip";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    if (BuildSystem.IsRunningOnAppVeyor)
    {
        var tag = AppVeyor.Environment.Repository.Tag;

        if (tag.IsTag)
        {
            packageVersion = tag.Name;
        }
        else
        {
            var buildNumber = AppVeyor.Environment.Build.Number.ToString("00000");
            var branch = AppVeyor.Environment.Repository.Branch;
            var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

            if (branch == "master" && !isPullRequest)
            {
                packageVersion = version + "-dev-" + buildNumber + dbgSuffix;
            }
            else
            {
                var suffix = "-ci-" + buildNumber + dbgSuffix;

                if (isPullRequest)
                    suffix += "-pr-" + AppVeyor.Environment.PullRequest.Number;
                else if (AppVeyor.Environment.Repository.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase))
                    suffix += "-pre-" + buildNumber;
                else
                    suffix += "-" + System.Text.RegularExpressions.Regex.Replace(branch, "[^0-9A-Za-z-]+", "-");

                // Nuget limits "special version part" to 20 chars. Add one for the hyphen.
                if (suffix.Length > 21)
                    suffix = suffix.Substring(0, 21);

                packageVersion = version + suffix;
            }
        }

        AppVeyor.UpdateBuildVersion(packageVersion);
    }

    Information("Building {0} version {1} of NUnit.", configuration, packageVersion);
});

//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Deletes all files in the BIN directory")
    .Does(() =>
    {
        CleanDirectory(BIN_DIR);
    });

//////////////////////////////////////////////////////////////////////
// NUGET RESTORE
//////////////////////////////////////////////////////////////////////

Task("NuGetRestore")
    .Description("Restores NuGet Packages")
    .Does(() =>
    {
        DotNetCoreRestore(SOLUTION_FILE);
    });

//////////////////////////////////////////////////////////////////////
// BUILD FRAMEWORKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Description("Builds the Solution")
    .IsDependentOn("NuGetRestore")
    .Does(() =>
    {
        DotNetCoreBuild(SOLUTION_FILE, CreateDotNetCoreBuildSettings());
    });

DotNetCoreBuildSettings CreateDotNetCoreBuildSettings() =>
    new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        Verbosity = DotNetCoreVerbosity.Minimal
    };

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
        var runtime = "net462";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + EXECUTABLE_NUNITLITE_TEST_RUNNER_EXE, dir, FRAMEWORK_TESTS, dir + "nunit.framework.tests.xml", runtime, ref ErrorDetail);
        //RunNUnitTests(dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS_EXE, dir, runtime, ref ErrorDetail);

        // TODO: Extract
        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.opencover,
            CoverletOutputDirectory = Directory(@".\coverage-results\"),
            CoverletOutputName = $"results-{runtime}_{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
        };
        
        Coverlet(dir);

        PublishTestResults(runtime);
    });

var testNetStandard20 = Task("TestNetStandard20")
    .Description("Tests the .NET Standard 2.0 version of the framework");

foreach (var runtime in NetCoreTests)
{
    var task = Task("TestNetStandard20 on " + runtime)
        .Description("Tests the .NET Standard 2.0 version of the framework on " + runtime)
        .WithCriteria(IsRunningOnWindows() || !runtime.EndsWith("windows"))
        .IsDependentOn("Build")
        .OnError(exception => { ErrorDetail.Add(exception.Message); })
        .Does(() =>
        {
            var dir = BIN_DIR + runtime + "/";
            RunDotnetCoreTests(dir + NUNITLITE_RUNNER_DLL, dir, FRAMEWORK_TESTS, runtime, GetResultXmlPath(FRAMEWORK_TESTS, runtime), ref ErrorDetail);
            RunDotnetCoreTests(dir + EXECUTABLE_NUNITLITE_TESTS_DLL, dir, runtime, ref ErrorDetail);

             // TODO: Extract
            var coverletSettings = new CoverletSettings {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputDirectory = Directory(@".\coverage-results\"),
                CoverletOutputName = $"results-netstandard20_{runtime}_{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
            };
        
            Coverlet(dir);

            PublishTestResults(runtime);
        });

    testNetStandard20.IsDependentOn(task);
}

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

var RootFiles = new FilePath[]
{
    "LICENSE.txt",
    "NOTICES.txt",
    "CHANGES.md"
};

// Not all of these are present in every framework
// The Microsoft and System assemblies are part of the BCL
// used by the .NET 4.0 framework. 4.0 tests will not run without them.
var FrameworkFiles = new FilePath[]
{
    "mock-assembly.dll",
    "mock-assembly.exe",
    "nunit.framework.dll",
    "nunit.framework.pdb",
    "nunit.framework.xml",
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

        var imageBinDir = CurrentImageDir + "bin/";

        CreateDirectory(imageBinDir);
        Information("Created directory " + imageBinDir);

        foreach (var runtime in LibraryFrameworks)
        {
            var targetDir = imageBinDir + Directory(runtime);
            var sourceDir = BIN_DIR + Directory(runtime);
            CreateDirectory(targetDir);
            foreach (FilePath file in FrameworkFiles)
            {
                var sourcePath = sourceDir + "/" + file;
                if (FileExists(sourcePath))
                    CopyFileToDirectory(sourcePath, targetDir);
            }
            var schemaPath = sourceDir + "/Schemas";
            if (DirectoryExists(schemaPath))
                CopyDirectory(sourceDir, targetDir);
        }

        foreach (var dir in NetCoreTests)
        {
            var targetDir = imageBinDir + Directory(dir);
            var sourceDir = BIN_DIR + Directory(dir);
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
// UPLOAD ARTIFACTS
//////////////////////////////////////////////////////////////////////

Task("UploadArtifacts")
    .Description("Uploads artifacts to AppVeyor")
    .IsDependentOn("Package")
    .Does(() =>
    {
        UploadArtifacts(PACKAGE_DIR, "*.nupkg");
        UploadArtifacts(PACKAGE_DIR, "*.snupkg");
        UploadArtifacts(PACKAGE_DIR, "*.zip");
    });

//////////////////////////////////////////////////////////////////////
// SETUP AND TEARDOWN TASKS
//////////////////////////////////////////////////////////////////////

Teardown(context => CheckForError(ref ErrorDetail));

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - GENERAL
//////////////////////////////////////////////////////////////////////

void UploadArtifacts(string packageDir, string searchPattern)
{
    foreach(var zip in System.IO.Directory.GetFiles(packageDir, searchPattern))
        AppVeyor.UploadArtifact(zip);
}

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
        return;

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
    .IsDependentOn("TestNetStandard20");

Task("Package")
    .Description("Packages all versions of the framework")
    .IsDependentOn("CheckForError")
    .IsDependentOn("PackageFramework")
    .IsDependentOn("PackageZip");

Task("Appveyor")
    .Description("Builds, tests and packages on AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("UploadArtifacts");

Task("Default")
    .Description("Builds all versions of the framework")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
