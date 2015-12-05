//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var framework = Argument("framework", "net-4.5");

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var baseVersion = "3.1.0";
var preRelease = "";
var displayVersion = "3.1";

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

string[] AllFrameworks = new string[] {
	"net-4.5", "net-4.0", "net-2.0", "portable", "sl-5.0" };

// These are the ones we run tests for
string[] TestFrameworks = new String[] {
	"net-4.5", "net-4.0", "net-2.0", "portable" };

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var dbgSuffix = configuration == "Debug" ? "-dbg" : "";
var PACKAGE_VERSION = baseVersion + preRelease + dbgSuffix;

var PACKAGE_NAME = "NUnit-" + PACKAGE_VERSION;
var PACKAGE_NAME_CF = "NUnitCF-" + PACKAGE_VERSION;
var PACKAGE_NAME_SL = "NUnitSL-" + PACKAGE_VERSION;

// Directories
var PACKAGE_DIR = Directory("./package");
var BIN_DIR = Directory("./bin/" + configuration);
var IMAGE_DIR = Directory("./images/" + PACKAGE_NAME);
var IMAGE_BIN_DIR = IMAGE_DIR + Directory("bin");

// Test Runners
var NUNIT3_CONSOLE = BIN_DIR + File("nunit3-console.exe");
var PORTABLE_RUNNER = BIN_DIR + File("portable/nunit.portable.tests.exe");

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var NUNITLITE_TESTS = "nunitlite.tests.exe";
var ENGINE_TESTS = "nunit.engine.tests.dll";
var ADDIN_TESTS = "addins/tests/addin-tests.dll";
var V2_DRIVER_TESTS = "addins/v2-tests/nunit.v2.driver.tests.dll";
var CONSOLE_TESTS = "nunit3-console.tests.dll";

// Packages
var SRC_PACKAGE = PACKAGE_DIR + File(
	"NUnit-" + baseVersion + dbgSuffix + "-src.zip");
var ZIP_PACKAGE = PACKAGE_DIR + File(PACKAGE_NAME + ".zip");

//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(BIN_DIR);
});


//////////////////////////////////////////////////////////////////////
// RESTORE PACKAGES
//////////////////////////////////////////////////////////////////////

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("./nunit.sln");
});

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

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
        XBuild("./nunit.linux.sln", new XBuildSettings()
            .SetConfiguration(configuration)
//            .WithTarget("AnyCPU")
//            .WithProperty("TreatWarningsAsErrors", "true")
            .SetVerbosity(Verbosity.Minimal)
        );
    }
});

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("TestAllFrameworks")
    .IsDependentOn("Build")
	.Does(() =>
	{
		foreach(string runtime in TestFrameworks)
		{
			if (runtime == "portable")
				RunProcess(PORTABLE_RUNNER, BIN_DIR);
			else
				RunProcess(NUNIT3_CONSOLE, BIN_DIR, runtime + "/" + FRAMEWORK_TESTS);

			RunProcess(BIN_DIR + File(runtime + "/" + NUNITLITE_TESTS), BIN_DIR);
		}
	});

Task("TestFramework")
    .IsDependentOn("Build")
	.Does(() => 
	{ 
		if (framework == "portable")
			RunProcess(PORTABLE_RUNNER, BIN_DIR);
		else
			RunProcess(NUNIT3_CONSOLE, BIN_DIR, framework + "/" + FRAMEWORK_TESTS);
	});

Task("TestNUnitLite")
    .IsDependentOn("Build")
	.Does(() => 
	{ 
		RunProcess(BIN_DIR + File(framework + "/" + NUNITLITE_TESTS), BIN_DIR);
	});

Task("TestEngine")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunProcess(NUNIT3_CONSOLE, BIN_DIR, ENGINE_TESTS);
	});

Task("TestAddins")
    .IsDependentOn("Build")
    .Does(() => 
	{
		RunProcess(NUNIT3_CONSOLE, BIN_DIR, ADDIN_TESTS); 
	});

Task("TestV2Driver")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunProcess(NUNIT3_CONSOLE, BIN_DIR, V2_DRIVER_TESTS);
	});

Task("TestConsole")
    .IsDependentOn("Build")
    .Does(() => 
	{ 
		RunProcess(NUNIT3_CONSOLE, BIN_DIR, CONSOLE_TESTS);
	});

//////////////////////////////////////////////////////////////////////
// PACKAGE
//////////////////////////////////////////////////////////////////////

var RootFiles = new FilePath[]
{
	"LICENSE.txt",
	"NOTICES.txt",
	"CHANGES.txt", 
	"nunit.ico"
};

var BinFiles = new FilePath[]
{
	"ConsoleTests.nunit",
	"EngineTests.nunit",
	"mock-nunit-assembly.exe",
	"Mono.Cecil.dll",
	"nunit-agent-x86.exe",
	"nunit-agent-x86.exe.config",
	"nunit-agent.exe",
	"nunit-agent.exe.config",
	"nunit.engine.addin.xml",
	"nunit.engine.addins",
	"nunit.engine.api.dll",
	"nunit.engine.api.xml",
	"nunit.engine.dll",
	"nunit.engine.tests.dll",
	"nunit.engine.tests.dll.config",
	"nunit.framework.dll",
	"nunit.framework.xml",
	"NUnit2TestResult.xsd",
	"nunit3-console.exe",
	"nunit3-console.exe.config",
	"nunit3-console.tests.dll",
	"nunitlite.dll",
	"TextSummary.xslt",
	"addins/nunit-project-loader.dll",
	"addins/nunit-v2-result-writer.dll",
	"addins/nunit.core.dll",
	"addins/nunit.core.interfaces.dll",
	"addins/nunit.v2.driver.dll",
	"addins/vs-project-loader.dll",
	"addins/tests/addin-tests.dll",
	"addins/tests/nunit-project-loader.dll",
	"addins/tests/nunit.engine.api.dll",
	"addins/tests/nunit.engine.api.xml",
	"addins/tests/nunit.framework.dll",
	"addins/tests/nunit.framework.xml",
	"addins/tests/vs-project-loader.dll",
	"addins/v2-tests/nunit.framework.dll",
	"addins/v2-tests/nunit.framework.xml",
	"addins/v2-tests/nunit.v2.driver.tests.dll"
};

// Not all of these are present in every framework
var FrameworkFiles = new FilePath[]
{
	"AppManifest.xaml",
	"mock-nunit-assembly.dll",
	"mock-nunit-assembly.exe",
	"nunit.framework.dll",
	"nunit.framework.xml",
	"nunit.framework.tests.dll",
	"nunit.framework.tests.xap",
	"nunit.framework.tests_TestPage.html",
	"nunit.testdata.dll",
	"nunitlite.dll",
	"nunitlite.tests.exe",
	"slow-nunit-tests.dll"
};

Task("PackageSource")
    .Does(() =>
	{
		RunGitCommand(string.Format("archive -o {0} HEAD", SRC_PACKAGE));
	});

Task("CreateImage")
	.Does(() =>
	{
		CleanDirectory(IMAGE_DIR);

		CopyFiles(RootFiles, IMAGE_DIR);

		CreateDirectory(IMAGE_BIN_DIR);

		foreach(FilePath file in BinFiles)
		{
			CreateDirectory(IMAGE_BIN_DIR + file.GetDirectory());
			CopyFile(BIN_DIR + file, IMAGE_BIN_DIR + file);
		}			

		foreach (var runtime in AllFrameworks)
		{
			var targetDir = IMAGE_BIN_DIR + Directory(runtime);
			var sourceDir = BIN_DIR + Directory(runtime);
			CreateDirectory(targetDir);
			foreach (FilePath file in FrameworkFiles)
				if (FileExists(sourceDir + file))
					CopyFileToDirectory(sourceDir + file, targetDir);
		}
	});

Task("PackageZip")
    .IsDependentOn("CreateImage")
	.Does(() =>
	{
		Zip(IMAGE_DIR, ZIP_PACKAGE);
	});

Task("PackageNuGet")
    .IsDependentOn("CreateImage")
	.Does(() =>
	{
		NuGetPack("nuget/nunit.nuspec", new NuGetPackSettings()
		{
			Version = PACKAGE_VERSION,
			BasePath = IMAGE_DIR,
			OutputDirectory = PACKAGE_DIR
		});
		NuGetPack("nuget/nunitlite.nuspec", new NuGetPackSettings()
		{
			Version = PACKAGE_VERSION,
			BasePath = IMAGE_DIR,
			OutputDirectory = PACKAGE_DIR
		});
		NuGetPack("nuget/nunit.console.nuspec", new NuGetPackSettings()
		{
			Version = PACKAGE_VERSION,
			BasePath = IMAGE_DIR,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/nunit.runners.nuspec", new NuGetPackSettings()
		{
			Version = PACKAGE_VERSION,
			BasePath = IMAGE_DIR,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/nunit.engine.nuspec", new NuGetPackSettings()
		{
			Version = PACKAGE_VERSION,
			BasePath = IMAGE_DIR,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
	});


//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

void RunProcess(FilePath exePath, DirectoryPath workingDir)
{
	StartProcess(exePath, new ProcessSettings()
	{
		WorkingDirectory = workingDir
	});
}

void RunProcess(FilePath exePath, DirectoryPath workingDir, string arguments)
{
	StartProcess(exePath, new ProcessSettings()
	{
		Arguments = arguments,
		WorkingDirectory = workingDir
	});
}

void RunGitCommand(string arguments)
{
	StartProcess("git", new ProcessSettings()
	{
		Arguments = arguments
	});
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

Task("Package")
	.IsDependentOn("PackageSource")
	.IsDependentOn("PackageZip")
	.IsDependentOn("PackageNuGet");

Task("Appveyor")
	.IsDependentOn("Build")
	.IsDependentOn("TestAll")
	.IsDependentOn("Package");

Task("Travis")
	.IsDependentOn("Build")
	.IsDependentOn("TestAll");

Task("Default")
    .IsDependentOn("Build"); // Rebuild?

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
