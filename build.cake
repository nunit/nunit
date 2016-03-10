//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var framework = Argument("framework", "net-4.5");

//////////////////////////////////////////////////////////////////////
// SET ERROR LEVELS
//////////////////////////////////////////////////////////////////////
var ErrorDetail = new List<string>();

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var version = "3.3.0";
var modifier = "";

var isCompactFrameworkInstalled = FileExists(Environment.GetEnvironmentVariable("windir") + "\\Microsoft.NET\\Framework\\v3.5\\Microsoft.CompactFramework.CSharp.targets");
var isAppveyor = BuildSystem.IsRunningOnAppVeyor;
var dbgSuffix = configuration == "Debug" ? "-dbg" : "";
var packageVersion = version + modifier + dbgSuffix;

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

var WindowsFrameworks = new string[] {
	"net-4.5", "net-4.0", "net-2.0", "portable", "sl-5.0", "netcf-3.5" };

var LinuxFrameworks = new string[] {
	"net-4.5", "net-4.0", "net-2.0" };

var AllFrameworks = IsRunningOnWindows() ? WindowsFrameworks : LinuxFrameworks;

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = PROJECT_DIR + "package/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";
var IMAGE_DIR = PROJECT_DIR + "images/";

// Test Runners
var NUNIT3_CONSOLE = BIN_DIR + "nunit3-console.exe";
var NUNITLITE_RUNNER = "nunitlite-runner.exe";

// Test Assemblies
var FRAMEWORK_TESTS = "nunit.framework.tests.dll";
var EXECUTABLE_FRAMEWORK_TESTS = "nunit.framework.tests.exe";
var NUNITLITE_TESTS = "nunitlite.tests.dll";
var EXECUTABLE_NUNITLITE_TESTS = "nunitlite.tests.exe";
var ENGINE_TESTS = "nunit.engine.tests.dll";
var PORTABLE_AGENT_TESTS = "agents/nunit.portable.agent.tests.dll";
var ADDIN_TESTS = "addins/tests/addin-tests.dll";
var V2_PORTABLE_AGENT_TESTS = "addins/v2-tests/nunit.v2.driver.tests.dll";
var CONSOLE_TESTS = "nunit3-console.tests.dll";

// Packages
var SRC_PACKAGE = PACKAGE_DIR + "NUnit-" + version + modifier + "-src.zip";
var ZIP_PACKAGE = PACKAGE_DIR + "NUnit-" + packageVersion + ".zip";
var ZIP_PACKAGE_SL = PACKAGE_DIR + "NUnitSL-" + packageVersion + ".zip";
var ZIP_PACKAGE_CF = PACKAGE_DIR + "NUnitCF-" + packageVersion + ".zip";

//////////////////////////////////////////////////////////////////////
// CLEAN
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(BIN_DIR);
    });


//////////////////////////////////////////////////////////////////////
// INITIALIZE FOR BUILD
//////////////////////////////////////////////////////////////////////

Task("InitializeBuild")
    .Does(() =>
    {
    if (IsRunningOnWindows())
        NuGetRestore("./nunit.sln");
    else
        NuGetRestore("./nunit.linux.sln");

	if (BuildSystem.IsRunningOnAppVeyor)
	{
		var tag = AppVeyor.Environment.Repository.Tag;

		if (tag.IsTag)
		{
			packageVersion = tag.Name;
		}
		else
		{
			var buildNumber = AppVeyor.Environment.Build.Number;
			packageVersion = version + "-CI-" + buildNumber + dbgSuffix;
			if (AppVeyor.Environment.PullRequest.IsPullRequest)
				packageVersion += "-PR-" + AppVeyor.Environment.PullRequest.Number;
			else if (AppVeyor.Environment.Repository.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase))
                packageVersion += "-PRE-" + buildNumber;
            else
                packageVersion += "-" + AppVeyor.Environment.Repository.Branch;
		}

		AppVeyor.UpdateBuildVersion(packageVersion);
	}
});

//////////////////////////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////////////////////////

Task("BuildAllFrameworks")
    .IsDependentOn("InitializeBuild")
    .Does(() =>
    {
        foreach (var runtime in AllFrameworks)
				BuildFramework(configuration, runtime, isCompactFrameworkInstalled);
    });

Task("BuildFramework")
    .IsDependentOn("InitializeBuild")
    .Does(() =>
    {
        BuildFramework(configuration, framework, isCompactFrameworkInstalled);
    });

Task("BuildEngine")
    .IsDependentOn("InitializeBuild")
    .Does(() =>
    {
        BuildEngine(configuration);
    });

Task("BuildConsole")
    .IsDependentOn("InitializeBuild")
    .Does(() =>
    {
        BuildConsole(configuration);
    });

Task("BuildCppTestFiles")
	.IsDependentOn("InitializeBuild")
	.WithCriteria(IsRunningOnWindows)
	.Does(() =>
	{
        MSBuild("./src/NUnitEngine/mock-cpp-clr/mock-cpp-clr-x86.vcxproj", new MSBuildSettings()
            .SetConfiguration(configuration)
			.WithProperty("Platform", "x86")
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false)
        );
        MSBuild("./src/NUnitEngine/mock-cpp-clr/mock-cpp-clr-x64.vcxproj", new MSBuildSettings()
            .SetConfiguration(configuration)
			.WithProperty("Platform", "x64")
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false)
        );
	});

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("CheckForError")
    .Does(() => CheckForError(ref ErrorDetail));

Task("Test45")
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "net-4.5";
	    var dir = BIN_DIR + runtime + "/";
		RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
		RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
	});

Task("Test40")
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "net-4.0";
	    var dir = BIN_DIR + runtime + "/";
		RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
		RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
	});

Task("Test20")
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "net-2.0";
	    var dir = BIN_DIR + runtime + "/";
		RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
		RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
	});

Task("TestPortable")
	.WithCriteria(IsRunningOnWindows())
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "portable";
	    var dir = BIN_DIR + runtime + "/";
		RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
		RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
	});

Task("TestSL")
	.WithCriteria(IsRunningOnWindows())
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "sl-5.0";
	    var dir = BIN_DIR + runtime + "/";
		RunTest(dir + NUNITLITE_RUNNER, dir, FRAMEWORK_TESTS, runtime, ref ErrorDetail);
		RunTest(dir + NUNITLITE_RUNNER, dir, NUNITLITE_TESTS, runtime, ref ErrorDetail);
	});

Task("TestCF")
	.WithCriteria(IsRunningOnWindows())
	.OnError(exception => {ErrorDetail.Add(exception.Message); })
	.Does(() =>
	{
	    var runtime = "netcf-3.5";
	    var dir = BIN_DIR + runtime + "/";
        RunTest(dir + EXECUTABLE_FRAMEWORK_TESTS, dir, runtime, ref ErrorDetail);
		RunTest(dir + EXECUTABLE_NUNITLITE_TESTS, dir, runtime, ref ErrorDetail);
	});

Task("TestEngine")
  .IsDependentOn("Build")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
  .Does(() =>
	{
		RunTest(NUNIT3_CONSOLE, BIN_DIR, ENGINE_TESTS, "TestEngine", ref ErrorDetail);
	});

Task("TestDriver")
  .IsDependentOn("Build")
  .WithCriteria(IsRunningOnWindows)
  .Does(() =>
	{
		RunTest(NUNIT3_CONSOLE, BIN_DIR, PORTABLE_AGENT_TESTS, ref ErrorDetail);
	});

Task("TestAddins")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
  .IsDependentOn("Build")
  .Does(() =>
	{
		RunTest(NUNIT3_CONSOLE, BIN_DIR, ADDIN_TESTS,"TestAddins", ref ErrorDetail);
	});

Task("TestV2Driver")
  .IsDependentOn("Build")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
  .Does(() =>
	{
		RunTest(NUNIT3_CONSOLE, BIN_DIR, V2_PORTABLE_AGENT_TESTS,"TestV2Driver", ref ErrorDetail);
	});

Task("TestConsole")
  .IsDependentOn("Build")
    .OnError(exception => { ErrorDetail.Add(exception.Message); })
  .Does(() =>
	{
		RunTest(NUNIT3_CONSOLE, BIN_DIR, CONSOLE_TESTS, "TestConsole", ref ErrorDetail);
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
	"mock-assembly.exe",
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
	"mock-assembly.dll",
	"mock-assembly.exe",
	"nunit.framework.dll",
	"nunit.framework.xml",
	"nunit.framework.tests.dll",
	"nunit.framework.tests.xap",
	"nunit.framework.tests_TestPage.html",
	"nunit.testdata.dll",
	"nunitlite.dll",
	"nunitlite.tests.exe",
	"nunitlite.tests.dll",
	"slow-nunit-tests.dll",
	"nunitlite-runner.exe"
};

Task("PackageSource")
  .Does(() =>
	{
		CreateDirectory(PACKAGE_DIR);
		RunGitCommand(string.Format("archive -o {0} HEAD", SRC_PACKAGE));
	});

Task("CreateImage")
	.Does(() =>
	{
		var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";
		var imageBinDir = currentImageDir + "bin/";

		CleanDirectory(currentImageDir);

		CopyFiles(RootFiles, currentImageDir);

		CreateDirectory(imageBinDir);
		Information("Created directory " + imageBinDir);

		foreach(FilePath file in BinFiles)
		{
		  if (FileExists(BIN_DIR + file))
		  {
			  CreateDirectory(imageBinDir + file.GetDirectory());
			  CopyFile(BIN_DIR + file, imageBinDir + file);
			}
		}

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

Task("PackageZip")
  .IsDependentOn("CreateImage")
	.Does(() =>
	{
		CreateDirectory(PACKAGE_DIR);

		var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";

		var zipFiles =
			GetFiles(currentImageDir + "*.*") +
			GetFiles(currentImageDir + "bin/*.*") +
			GetFiles(currentImageDir + "bin/addins/*.*") +
			GetFiles(currentImageDir + "bin/addins/tests/*.*") +
			GetFiles(currentImageDir + "bin/addins/v2-tests/*.*") +
			GetFiles(currentImageDir + "bin/net-2.0/*.*") +
			GetFiles(currentImageDir + "bin/net-4.0/*.*") +
			GetFiles(currentImageDir + "bin/net-4.5/*.*") +
			GetFiles(currentImageDir + "bin/portable/*.*");
		Zip(currentImageDir, File(ZIP_PACKAGE), zipFiles);

		zipFiles =
			GetFiles(currentImageDir + "*.*") +
			GetFiles(currentImageDir + "bin/sl-5.0/*.*");
		Zip(currentImageDir, File(ZIP_PACKAGE_SL), zipFiles);
	});

Task("PackageNuGet")
  .IsDependentOn("CreateImage")
	.Does(() =>
	{
		var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";

		CreateDirectory(PACKAGE_DIR);

		// Package framework
		NuGetPack("nuget/framework/nunit.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});
		NuGetPack("nuget/framework/nunitSL.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});

		// Package NUnitLite
		NuGetPack("nuget/nunitlite/nunitlite.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});
		NuGetPack("nuget/nunitlite/nunitliteSL.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});

		// Package Runners
		NuGetPack("nuget/runners/nunit.console-runner.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/runners/nunit.console-runner-with-extensions.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/runners/nunit.runners.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});

		// Package engine
		NuGetPack("nuget/engine/nunit.engine.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/engine/nunit.engine.tool.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});

		// Package Extensions
		NuGetPack("nuget/extensions/nunit-project-loader.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/extensions/vs-project-loader.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/extensions/nunit-v2-result-writer.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
		NuGetPack("nuget/extensions/nunit.v2.driver.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR,
			NoPackageAnalysis = true
		});
	});

Task("PackageMsi")
  .IsDependentOn("CreateImage")
  .WithCriteria(IsRunningOnWindows)
	.Does(() =>
	{
        MSBuild("install/master/nunit.wixproj", new MSBuildSettings()
			.WithTarget("Rebuild")
            .SetConfiguration(configuration)
			.WithProperty("PackageVersion", packageVersion)
			.WithProperty("DisplayVersion", version)
			.WithProperty("OutDir", PACKAGE_DIR)
			.WithProperty("InstallImage", IMAGE_DIR + "NUnit-" + packageVersion)
			.SetMSBuildPlatform(MSBuildPlatform.x86)
            .SetNodeReuse(false)
        );
	});

Task("PackageCF")
	.IsDependentOn("CreateImage")
	.Does(() =>
	{
		CreateDirectory(PACKAGE_DIR);

		var currentImageDir = IMAGE_DIR + "NUnit-" + packageVersion + "/";

		var zipFiles =
			GetFiles(currentImageDir + "*.*") +
			GetFiles(currentImageDir + "bin/netcf-3.5/*.*");

		Zip(currentImageDir, File(ZIP_PACKAGE_CF), zipFiles);

		NuGetPack("nuget/framework/nunitCF.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});
		NuGetPack("nuget/nunitlite/nunitLiteCF.nuspec", new NuGetPackSettings()
		{
			Version = packageVersion,
			BasePath = currentImageDir,
			OutputDirectory = PACKAGE_DIR
		});
	});

//////////////////////////////////////////////////////////////////////
// SETUP AND TEARDOWN TASKS
//////////////////////////////////////////////////////////////////////
Setup(() =>
{
    // Executed BEFORE the first task.
	});

Teardown(() =>
{
    // Executed AFTER the last task.
    CheckForError(ref ErrorDetail);
});

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

void CheckForError(ref List<string> errorDetail)
{
    if(errorDetail.Count != 0)
    {
        var copyError = new List<string>();
        copyError = errorDetail.Select(s => s).ToList();
        errorDetail.Clear();
        throw new Exception("One or more unit test failed, breaking the build.\n"
                              + copyError.Aggregate((x,y) => x + "\n" + y));
    }
}

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - BUILD
//////////////////////////////////////////////////////////////////////

void BuildFramework(string configuration, string framework, bool isCompactFrameworkInstalled)
{
	switch(framework)
	{
		case "net-4.5":
		case "net-4.0":
		case "net-2.0":
			var suffix = framework.Substring(4);
			BuildProject("src/NUnitFramework/framework/nunit.framework-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite/nunitlite-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/testdata/nunit.testdata-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/slow-tests/slow-nunit-tests-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/tests/nunit.framework.tests-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-" + suffix +".csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-" + suffix + ".csproj", configuration);
			break;

		case "portable":
			BuildProject("src/NUnitFramework/framework/nunit.framework-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite/nunitlite-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/testdata/nunit.testdata-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/tests/nunit.framework.tests-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-portable.csproj", configuration);
			BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-portable.csproj", configuration);
			break;

		case "sl-5.0":
			BuildProject("src/NUnitFramework/framework/nunit.framework-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/nunitlite/nunitlite-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/mock-assembly/mock-assembly-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/testdata/nunit.testdata-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/tests/nunit.framework.tests-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/nunitlite.tests/nunitlite.tests-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			BuildProject("src/NUnitFramework/nunitlite-runner/nunitlite-runner-sl-5.0.csproj", configuration, MSBuildPlatform.x86);
			break;
        case "netcf-3.5":
            if(isCompactFrameworkInstalled)
            {
                BuildProjectCF("src/NUnitFramework/framework/nunit.framework-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/mock-assembly/mock-assembly-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/testdata/nunit.testdata-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/tests/nunit.framework.tests-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/slow-tests/slow-nunit-tests-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/nunitlite/nunitlite-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/nunitlite.tests/nunitlite.tests-netcf-3.5.csproj", configuration);
                BuildProjectCF("src/NUnitFramework/nunitlite-runner/nunitlite-runner-netcf-3.5.csproj", configuration);
            }
            else
            {
                Information("Compact framework build skipped because files were not present.");
                if(isAppveyor) 
                    throw new Exception("Running Build on Appveyor, but CF not installed, please check that the appveyor-tools.ps1 script ran correctly.");
            }
            break;
	}
}

void BuildEngine(string configuration)
{
    BuildProject("./src/NUnitEngine/nunit.engine.api/nunit.engine.api.csproj", configuration);
    BuildProject("./src/NUnitEngine/nunit.engine/nunit.engine.csproj", configuration);
    BuildProject("./src/NUnitEngine/nunit-agent/nunit-agent.csproj", configuration);
    BuildProject("./src/NUnitEngine/nunit-agent/nunit-agent-x86.csproj", configuration);

    // Engine tests
    BuildProject("./src/NUnitEngine/nunit.engine.tests/nunit.engine.tests.csproj", configuration);

    // Driver and tests
    if(IsRunningOnWindows())
    {
        BuildProject("./src/NUnitEngine/Portable/nunit.portable.agent/nunit.portable.agent.csproj", configuration);
        BuildProject("./src/NUnitEngine/Portable/nunit.portable.agent.tests/nunit.portable.agent.tests.csproj", configuration);
    }

    // Addins
    BuildProject("./src/NUnitEngine/Addins/nunit-project-loader/nunit-project-loader.csproj", configuration);
    BuildProject("./src/NUnitEngine/Addins/vs-project-loader/vs-project-loader.csproj", configuration);
    BuildProject("./src/NUnitEngine/Addins/nunit-v2-result-writer/nunit-v2-result-writer.csproj", configuration);
    BuildProject("./src/NUnitEngine/Addins/nunit.v2.driver/nunit.v2.driver.csproj", configuration);

    // Addin tests
    BuildProject("./src/NUnitEngine/Addins/addin-tests/addin-tests.csproj", configuration);
    BuildProject("./src/NUnitEngine/Addins/nunit.v2.driver.tests/nunit.v2.driver.tests.csproj", configuration);
}

void BuildConsole(string configuration)
{
    BuildProject("src/NUnitConsole/nunit3-console/nunit3-console.csproj", configuration);
    BuildProject("src/NUnitConsole/nunit3-console.tests/nunit3-console.tests.csproj", configuration);
}

void BuildProject(string projectPath, string configuration)
{
	BuildProject(projectPath, configuration, MSBuildPlatform.Automatic);
}

void BuildProjectCF(string projectPath, string configuration)
{
    if(IsRunningOnWindows())
    {
        // Use MSBuild
        MSBuild(projectPath, new MSBuildSettings()
            .SetConfiguration(configuration)
			.SetMSBuildPlatform(MSBuildPlatform.x86)
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false)
            .UseToolVersion(MSBuildToolVersion.VS2008)
        );
    }
}

void BuildProject(string projectPath, string configuration, MSBuildPlatform buildPlatform)
{
    if(IsRunningOnWindows())
    {
        // Use MSBuild
        MSBuild(projectPath, new MSBuildSettings()
            .SetConfiguration(configuration)
			.SetMSBuildPlatform(buildPlatform)
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false)
        );
    }
    else
    {
        // Use XBuild
        XBuild(projectPath, new XBuildSettings()
            .WithTarget("Build")
            .WithProperty("Configuration", configuration)
            .SetVerbosity(Verbosity.Minimal)
        );
    }
}

//////////////////////////////////////////////////////////////////////
// HELPER METHODS - TEST
//////////////////////////////////////////////////////////////////////

void RunTest(FilePath exePath, DirectoryPath workingDir, string framework, ref List<string> errorDetail)
{
	int rc = StartProcess(
	  MakeAbsolute(exePath),
	  new ProcessSettings()
	  {
		  WorkingDirectory = workingDir
	  });

	if (rc > 0)
        errorDetail.Add(string.Format("{0}: {1} tests failed",framework, rc));
	else if (rc < 0)
        errorDetail.Add(string.Format("{0} returned rc = {1}", exePath, rc));
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
        errorDetail.Add(string.Format("{0}: {1} tests failed",framework, rc));
	else if (rc < 0)
        errorDetail.Add(string.Format("{0} returned rc = {1}", exePath, rc));
}

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build")
  .IsDependentOn("BuildAllFrameworks")
  .IsDependentOn("BuildEngine")
  .IsDependentOn("BuildConsole");

Task("Rebuild")
    .IsDependentOn("Clean")
	.IsDependentOn("Build");

Task("TestAll")
	.IsDependentOn("TestAllFrameworks")
	.IsDependentOn("TestEngine")
    .IsDependentOn("TestDriver")
	.IsDependentOn("TestAddins")
	.IsDependentOn("TestV2Driver")
	.IsDependentOn("TestConsole");

// NOTE: Test has been changed to now be a synonym of TestAll    
Task("Test")
	.IsDependentOn("TestAllFrameworks")
	.IsDependentOn("TestEngine")
    .IsDependentOn("TestDriver")
	.IsDependentOn("TestAddins")
	.IsDependentOn("TestV2Driver")
	.IsDependentOn("TestConsole");

Task("TestAllFrameworks")
    .IsDependentOn("Build")
	.IsDependentOn("Test45")
	.IsDependentOn("Test40")
	.IsDependentOn("Test20")
// NOTE: The following tasks use Criteria and will be skipped on Linux
	.IsDependentOn("TestPortable")
	.IsDependentOn("TestSL")
	.IsDependentOn("TestCF");

Task("Package")
    .IsDependentOn("CheckForError")
	.IsDependentOn("PackageSource")
	.IsDependentOn("PackageZip")
	.IsDependentOn("PackageNuGet")
	.IsDependentOn("PackageMsi");

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
