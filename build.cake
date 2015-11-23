//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var framework = Argument("framework", "net-4.5");

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

string[] AllFrameworks = new string[] {
	"net-4.5", "net-4.0", "net-2.0", "portable" };

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Versioning
var packageVersion = "3.1.0";

// Directories
//var baseDir = Directory(".");
//var srcDir = Directory("./src");
//var nugetDir = Directory("./nuget");
//var toolsDir = Directory("./tools");
//var libDir = Directory ("./lib");
var binDir = Directory("./bin") + Directory(configuration);

// Test Runners
var NUNIT3_CONSOLE = binDir + File("nunit3-console.exe");
var PORTABLE_RUNNER = binDir + File("portable/nunit.portable.tests.exe");

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var NUNITLITE_TESTS = "nunitlite.tests.exe";
var ENGINE_TESTS = "nunit.engine.tests.dll";
var ADDIN_TESTS = "addins/tests/addin-tests.dll";
var V2_DRIVER_TESTS = "addins/v2-tests/nunit.v2.driver.tests.dll";
var CONSOLE_TESTS = "nunit3-console.tests.dll";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    Information("Building version {0} of Nunit.", packageVersion);
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(binDir);
});


Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("./nunit.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        // Use MSBuild
        MSBuild("./nunit.sln", new MSBuildSettings()
            .SetConfiguration(configuration)
            .SetPlatformTarget(PlatformTarget.MSIL)
//            .WithProperty("TreatWarningsAsErrors", "true")
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false)
        );
    }
    else
    {
        // Use XBuild
        XBuild("./nunit.sln", new XBuildSettings()
            .SetConfiguration(configuration)
            .WithTarget("AnyCPU")
//            .WithProperty("TreatWarningsAsErrors", "true")
            .SetVerbosity(Verbosity.Minimal)
        );
    }
});

Task("TestAllFrameworks")
    .IsDependentOn("Build")
	.Does(() =>
	{
		foreach(string runtime in AllFrameworks)
		{
			RunFrameworkTests(runtime);
			RunNUnitLiteTests(runtime);
		}
	});

Task("TestFramework")
    .IsDependentOn("Build")
	.Does(() => 
	{ 
		RunFrameworkTests(framework);
	});

Task("TestNUnitLite")
    .IsDependentOn("Build")
	.Does(() => 
	{ 
		RunNUnitLiteTests(framework);
	});

Task("TestEngine")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunTestWithConsoleRunner(ENGINE_TESTS);
	});

Task("TestAddins")
    .IsDependentOn("Build")
    .Does(() => 
	{
		RunTestWithConsoleRunner(ADDIN_TESTS); 
	});

Task("TestV2Driver")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunTestWithConsoleRunner(V2_DRIVER_TESTS);
	});

Task("TestConsole")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunTestWithConsoleRunner(CONSOLE_TESTS);
	});

//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

void RunExecutableTest(FilePath exePath)
{
	StartProcess(exePath, new ProcessSettings()
	{
		WorkingDirectory = binDir
	});
}

void RunTestWithConsoleRunner(string testAssembly)
{
	StartProcess(NUNIT3_CONSOLE, new ProcessSettings()
	{
		Arguments = testAssembly,
		WorkingDirectory = binDir
	});
}

void RunNUnitLiteTests(string runtime)
{
	RunExecutableTest(binDir + File(runtime + "/" + NUNITLITE_TESTS));
}

void RunFrameworkTests(string runtime)
{
	if (runtime == "portable")
		RunExecutableTest(PORTABLE_RUNNER);
	else
		RunTestWithConsoleRunner(runtime + "/" + FRAMEWORK_TESTS);
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
    .IsDependentOn("Clean")
	.IsDependentOn("Build");

Task("TestAll")
	.IsDependentOn("TestAllFrameworks")
	.IsDependentOn("TestEngine")
	.IsDependentOn("TestAddins")
	.IsDependentOn("TestV2Driver")
	.IsDependentOn("TestConsole");

Task("Test")
	.IsDependentOn("TestFramework")
	.IsDependentOn("TestNUnitLite")
	.IsDependentOn("TestEngine")
	.IsDependentOn("TestAddins")
	.IsDependentOn("TestV2Driver")
	.IsDependentOn("TestConsole");

Task("Default")
    .IsDependentOn("Build"); // Rebuild?

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
