//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// SET ERROR LEVELS
//////////////////////////////////////////////////////////////////////

var ErrorDetail = new List<string>();

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var version = "3.6.0";
var modifier = "";

var isAppveyor = BuildSystem.IsRunningOnAppVeyor;
var dbgSuffix = configuration == "Debug" ? "-dbg" : "";
var packageVersion = version + modifier + dbgSuffix;

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

var WindowsFrameworks = new string[] {
    "net-4.5", "net-4.0", "net-3.5", "net-2.0", "portable" };

var LinuxFrameworks = new string[] {
    "net-4.5", "net-4.0", "net-3.5", "net-2.0" };

var AllFrameworks = IsRunningOnWindows() ? WindowsFrameworks : LinuxFrameworks;

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = PROJECT_DIR + "package/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";
var IMAGE_DIR = PROJECT_DIR + "images/";

var SOLUTION_FILE = IsRunningOnWindows()
    ? "./nunit.sln"
    : "./nunit.linux.sln";

// Package sources for nuget restore
var PACKAGE_SOURCE = new string[]
    {
        "https://www.nuget.org/api/v2",
        "https://www.myget.org/F/nunit/api/v2"
    };

// Test Runners
var NUNITLITE_RUNNER = "nunitlite-runner.exe";

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var EXECUTABLE_FRAMEWORK_TESTS = "nunit.framework.tests.exe";
var NUNITLITE_TESTS = "nunitlite.tests.dll";
var EXECUTABLE_NUNITLITE_TESTS = "nunitlite.tests.exe";

// Packages
var SRC_PACKAGE = PACKAGE_DIR + "NUnit-" + version + modifier + "-src.zip";
var ZIP_PACKAGE = PACKAGE_DIR + "NUnit-" + packageVersion + ".zip";

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
// INITIALIZE FOR BUILD
//////////////////////////////////////////////////////////////////////

Task("InitializeBuild")
    .Description("Initializes the build")
    .Does(() =>
    {
        NuGetRestore(SOLUTION_FILE, new NuGetRestoreSettings()
        {
            Source = PACKAGE_SOURCE
        });

        if (isAppveyor)
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
                        suffix += "-" + branch;

                    // Nuget limits "special version part" to 20 chars. Add one for the hyphen.
                    if (suffix.Length > 21)
                        suffix = suffix.Substring(0, 21);

                    packageVersion = version + suffix;
                }
            }

            AppVeyor.UpdateBuildVersion(packageVersion);
        }
    });

//////////////////////////////////////////////////////////////////////
// BUILD FRAMEWORKS
//////////////////////////////////////////////////////////////////////

Task("Build45")
    .Description("Builds the .NET 4.5 version of the framework")
    .Does(() =>
    {
        BuildProject("src/NUnitFramework/framework/nunit.framework-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite/nunitlite-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/testdata/nunit.testdata-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/slow-tests/slow-nunit-tests-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/tests/nunit.framework.tests-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-4.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-4.5.csproj", configuration);
    });

Task("Build40")
    .Description("Builds the .NET 4.0 version of the framework")
    .Does(() =>
    {
        BuildProject("src/NUnitFramework/framework/nunit.framework-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite/nunitlite-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/testdata/nunit.testdata-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/slow-tests/slow-nunit-tests-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/tests/nunit.framework.tests-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-4.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-4.0.csproj", configuration);
    });

Task("Build35")
    .Description("Builds the .NET 3.5 version of the framework")
    .Does(() =>
    {
        BuildProject("src/NUnitFramework/framework/nunit.framework-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite/nunitlite-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/testdata/nunit.testdata-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/slow-tests/slow-nunit-tests-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/tests/nunit.framework.tests-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-3.5.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-3.5.csproj", configuration);
    });

Task("Build20")
    .Description("Builds the .NET 2.0 version of the framework")
    .Does(() =>
    {
        BuildProject("src/NUnitFramework/framework/nunit.framework-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite/nunitlite-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/testdata/nunit.testdata-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/slow-tests/slow-nunit-tests-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/tests/nunit.framework.tests-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-2.0.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-2.0.csproj", configuration);
    });

Task("BuildPortable")
    .Description("Builds the PCL version of the framework")
    .WithCriteria(IsRunningOnWindows())
    .Does(() =>
    {
        BuildProject("src/NUnitFramework/framework/nunit.framework-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite/nunitlite-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/testdata/nunit.testdata-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/tests/nunit.framework.tests-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-portable.csproj", configuration);
        BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-portable.csproj", configuration);
    });

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("CheckForError")
    .Description("Checks for errors running the test suites")
    .Does(() => CheckForError(ref ErrorDetail));

//////////////////////////////////////////////////////////////////////
// TEST FRAMEWORK
//////////////////////////////////////////////////////////////////////

Task("Test45")
    .Description("Tests the .NET 4.5 version of the framework")
    .IsDependentOn("Build45")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = "net-4.5";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
    });

Task("Test40")
    .Description("Tests the .NET 4.0 version of the framework")
    .IsDependentOn("Build40")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = "net-4.0";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
    });

Task("Test35")
    .Description("Tests the .NET 3.5 version of the framework")
    .IsDependentOn("Build35")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = "net-3.5";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
    });

Task("Test20")
    .Description("Tests the .NET 2.0 version of the framework")
    .IsDependentOn("Build20")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = "net-2.0";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
    });

Task("TestPortable")
    .Description("Tests the PCL version of the framework")
    .WithCriteria(IsRunningOnWindows())
    .IsDependentOn("BuildPortable")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
    .Does(() =>
    {
        var runtime = "portable";
        var dir = BIN_DIR + runtime + "/";
        RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
        RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
    });

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

var RootFiles = new FilePath[]
{
    "LICENSE.txt",
    "NOTICES.txt",
    "CHANGES.txt"
};

// Not all of these are present in every framework
// The Microsoft and System assemblies are part of the BCL
// used by the .NET 4.0 framework. 4.0 tests will not run without them.
// NUnit.System.Linq is only present for the .NET 2.0 build.
var FrameworkFiles = new FilePath[]
{
    "mock-assembly.dll",
    "mock-assembly.exe",
    "nunit.framework.dll",
    "nunit.framework.xml",
    "NUnit.System.Linq.dll",
    "nunit.framework.tests.dll",
    "nunit.testdata.dll",
    "nunitlite.dll",
    "nunitlite.tests.exe",
    "nunitlite.tests.dll",
    "slow-nunit-tests.dll",
    "nunitlite-runner.exe",
    "Microsoft.Threading.Tasks.dll",
    "Microsoft.Threading.Tasks.Extensions.Desktop.dll",
    "Microsoft.Threading.Tasks.Extensions.dll",
    "System.IO.dll",
    "System.Runtime.dll",
    "System.Threading.Tasks.dll"
};

Task("PackageSource")
    .Description("Creates a ZIP file of the source code")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);
        RunGitCommand(string.Format("archive -o {0} HEAD", SRC_PACKAGE));
    });

Task("CreateImage")
    .Description("Copies all files into the image directory")
    .Does(() =>
    {
        var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";
        var imageBinDir = currentImageDir + "bin/";

        CleanDirectory(currentImageDir);

        CopyFiles(RootFiles, currentImageDir);

        CreateDirectory(imageBinDir);
        Information("Created directory " + imageBinDir);

        foreach (var runtime in AllFrameworks)
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
        }
    });

Task("PackageFramework")
    .Description("Creates NuGet packages of the framework")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";

        CreateDirectory(PACKAGE_DIR);

        NuGetPack("nuget/framework/nunit.nuspec", new NuGetPackSettings()
        {
            Version = packageVersion,
            BasePath = currentImageDir,
            OutputDirectory = PACKAGE_DIR
        });

        NuGetPack("nuget/nunitlite/nunitlite.nuspec", new NuGetPackSettings()
        {
            Version = packageVersion,
            BasePath = currentImageDir,
            OutputDirectory = PACKAGE_DIR
        });
    });

Task("PackageZip")
    .Description("Creates a ZIP file of the framework")
    .IsDependentOn("CreateImage")
    .Does(() =>
    {
        CreateDirectory(PACKAGE_DIR);

        var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";

        var zipFiles =
            GetFiles(currentImageDir + "*.*") +
            GetFiles(currentImageDir + "bin/net-2.0/*.*") +
            GetFiles(currentImageDir + "bin/net-3.5/*.*") +
            GetFiles(currentImageDir + "bin/net-4.0/*.*") +
            GetFiles(currentImageDir + "bin/net-4.5/*.*") +
            GetFiles(currentImageDir + "bin/portable/*.*");
        Zip(currentImageDir, File(ZIP_PACKAGE), zipFiles);
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
        UploadArtifacts(PACKAGE_DIR, "*.zip");
    });

//////////////////////////////////////////////////////////////////////
// SETUP AND TEARDOWN TASKS
//////////////////////////////////////////////////////////////////////

Teardown(context => CheckForError(ref ErrorDetail));

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - GENERAL
//////////////////////////////////////////////////////////////////////

void RunGitCommand(string arguments)
{
    StartProcess("git", new ProcessSettings()
    {
        Arguments = arguments
    });
}

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
// HELPER METHODS - BUILD
//////////////////////////////////////////////////////////////////////

void BuildProject(string projectPath, string configuration)
{
    DotNetBuild(projectPath, settings =>
        settings.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .WithTarget("Build")
        .WithProperty("NodeReuse", "false"));
}

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - TEST
//////////////////////////////////////////////////////////////////////

void RunTest(FilePath exePath, DirectoryPath workingDir, string framework, ref List<string> errorDetail)
{
    RunTest(exePath, workingDir, null, framework, ref errorDetail);
}

void RunTest(FilePath exePath, DirectoryPath workingDir, string arguments, string framework, ref List<string> errorDetail)
{
    int rc = StartProcess(
        MakeAbsolute(exePath),
        new ProcessSettings()
        {
            Arguments = arguments,
            WorkingDirectory = workingDir
        });

    if (rc > 0)
        errorDetail.Add(string.Format("{0}: {1} tests failed", framework, rc));
    else if (rc < 0)
        errorDetail.Add(string.Format("{0} returned rc = {1}", exePath, rc));
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
    .Description("Rebuilds all versions of the framework")
    .IsDependentOn("Clean")
    .IsDependentOn("Build");

Task("Build")
    .Description("Builds all versions of the framework")
    .IsDependentOn("InitializeBuild")
    .IsDependentOn("Build45")
    .IsDependentOn("Build40")
    .IsDependentOn("Build35")
    .IsDependentOn("Build20")
// NOTE: The following tasks use Criteria and will be skipped on Linux
    .IsDependentOn("BuildPortable");

Task("Test")
    .Description("Builds and tests all versions of the framework")
    .IsDependentOn("Build")
    .IsDependentOn("Test45")
    .IsDependentOn("Test40")
    .IsDependentOn("Test35")
    .IsDependentOn("Test20")
// NOTE: The following tasks use Criteria and will be skipped on Linux
    .IsDependentOn("TestPortable");

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

Task("Travis")
    .Description("Builds and tests on Travis")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Default")
    .Description("Builds all versions of the framework")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
