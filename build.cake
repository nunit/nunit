#load "CakeScripts/MinVerTool.cs"
#load "CakeScripts/TestResultsParser.cs"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var minimal = Argument("minimal", false);
var quiet = Argument("quiet", true);

//////////////////////////////////////////////////////////////////////
// SET PACKAGE VERSION
//////////////////////////////////////////////////////////////////////

var version = MinVerTool.GetVersion(AutoIncrement.Minor, tagPrefix: "v");

var packageVersion = version;

//////////////////////////////////////////////////////////////////////
// DEFINE RUN CONSTANTS
//////////////////////////////////////////////////////////////////////

var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath + "/";
var PACKAGE_DIR = Argument("artifact-dir", PROJECT_DIR + "package") + "/";
var BIN_DIR = PROJECT_DIR + "bin/" + configuration + "/";
var IMAGE_DIR = PROJECT_DIR + "images/";
var NUNITLITETESTSBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite.tests/bin/" + configuration + "/";
var NUNITFRAMEWORKBIN = PROJECT_DIR + "src/NUnitFramework/framework/bin/" + configuration + "/";
var NUNITFRAMEWORKLEGACYBIN = PROJECT_DIR + "src/NUnitFramework/nunit.framework.legacy/bin/" + configuration + "/";
var NUNITLITEBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite/bin/" + configuration + "/";
var NUNITLITERUNNERBIN = PROJECT_DIR + "src/NUnitFramework/nunitlite-runner/bin/" + configuration + "/";

var SOLUTION_FILE = "./nunit.slnx";

var DIRECTORY_BUILD_PROPS = PROJECT_DIR + "src/NUnitFramework/Directory.Build.props";

// Test Assemblies (for NUnitLite dogfood tests)
var EXECUTABLE_NUNITLITE_TESTS_EXE = "nunitlite.tests.exe";
var EXECUTABLE_NUNITLITE_TESTS_DLL = "nunitlite.tests.dll";

// Packages
var ZIP_PACKAGE = PACKAGE_DIR + "NUnit.Framework-" + packageVersion + ".zip";

//////////////////////////////////////////////////////////////////////
// SUPPORTED FRAMEWORKS
//////////////////////////////////////////////////////////////////////

var LibraryFrameworks = XmlPeek(DIRECTORY_BUILD_PROPS, "/Project/PropertyGroup/NUnitLibraryFrameworks").Split(';');
var RuntimeFrameworks = XmlPeek(DIRECTORY_BUILD_PROPS, "/Project/PropertyGroup/NUnitRuntimeFrameworks").Split(';');

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information($"Building {configuration} version {packageVersion} of NUnit.");
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
    // Version settings removed - MinVer package now handles AssemblyVersion, FileVersion, InformationalVersion
    var msBuildSettings = new DotNetMSBuildSettings {
        ContinuousIntegrationBuild = BuildSystem.GitHubActions.IsRunningOnGitHubActions
    };

    var settings = new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        Verbosity = DotNetVerbosity.Quiet,
        MSBuildSettings = msBuildSettings
    };
    return settings;
}

//////////////////////////////////////////////////////////////////////
// TEST
//////////////////////////////////////////////////////////////////////

Task("Test")
    .Description("Runs all tests using dotnet test. Use --minimal=true for minimal output.")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var resultsDir = PROJECT_DIR + "TestResults";
        CleanDirectory(resultsDir);

        var loggers = minimal
            ? new[] { "trx", "console;verbosity=minimal" }
            : (quiet 
               ? new[] { "trx", "console;verbosity=quiet" }
               : new[] { "trx", "console;verbosity=normal"}) ;

        var settings = new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            Settings = minimal
               ? "minimal.runsettings"
               : (quiet ? "quiet.runsettings" : ".runsettings"),
            // ResultsDirectory is set in runsettings files - keeping single source of truth
            Loggers = loggers,
            Verbosity = minimal ? DotNetVerbosity.Minimal : (quiet ? DotNetVerbosity.Quiet : DotNetVerbosity.Normal)
        };

        // Run tests but don't throw on failure - we want to show the summary first
        int exitCode = 0;
        try
        {
            DotNetTest(SOLUTION_FILE, settings);
        }
        catch (Exception)
        {
            exitCode = 1;
        }

        // Parse TRX files and show summary
        var summary = TestResultsParser.ParseTrxFilesDetailed(resultsDir);

        Information("");
        Information("═══════════════════════════════════════════════════════════════════");
        Information("  TEST RESULTS BY FRAMEWORK");
        Information("───────────────────────────────────────────────────────────────────");

        foreach (var fw in summary.ByFramework)
        {
            var status = fw.Failed > 0 ? "FAIL" : "PASS";
            Information($"  {fw.Framework,-15} {status}  {fw.Total,5} total, {fw.Passed,5} passed, {fw.Failed,3} failed, {fw.Skipped,3} skipped");
        }

        Information("───────────────────────────────────────────────────────────────────");
        Information($"  {"TOTAL",-15}       {summary.Total,5} total, {summary.Passed,5} passed, {summary.Failed,3} failed, {summary.Skipped,3} skipped");
        Information("═══════════════════════════════════════════════════════════════════");

        // Throw after showing summary if tests failed
        if (exitCode != 0 || summary.Failed > 0)
            throw new Exception($"Tests failed. {summary.Failed} test(s) reported as failed.");
    });

Task("TestNUnitLite")
    .Description("Tests NUnitLite by running nunitlite.tests (dogfooding)")
    .IsDependentOn("Build")
    .Does(() =>
    {
        // Run nunitlite.tests for each runtime framework
        foreach (var runtime in RuntimeFrameworks)
        {
            var dir = NUNITLITETESTSBIN + runtime + "/";

            if (runtime.StartsWith("net4"))
            {
                // .NET Framework - run exe directly
                var exePath = dir + EXECUTABLE_NUNITLITE_TESTS_EXE;
                if (FileExists(exePath))
                {
                    Information($"Running NUnitLite tests for {runtime}");
                    var rc = StartProcess(MakeAbsolute(new FilePath(exePath)), new ProcessSettings { WorkingDirectory = dir });
                    if (rc != 0)
                        throw new Exception($"NUnitLite tests failed for {runtime} with exit code {rc}");
                }
            }
            else
            {
                // .NET Core - run with dotnet
                var dllPath = dir + EXECUTABLE_NUNITLITE_TESTS_DLL;
                if (FileExists(dllPath))
                {
                    Information($"Running NUnitLite tests for {runtime}");
                    var rc = StartProcess("dotnet", new ProcessSettings
                    {
                        Arguments = new ProcessArgumentBuilder().AppendQuoted(dllPath).Render(),
                        WorkingDirectory = dir
                    });
                    if (rc != 0)
                        throw new Exception($"NUnitLite tests failed for {runtime} with exit code {rc}");
                }
            }
        }
    });

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
            Information($"Signing {file.FullPath}...");

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
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Rebuild")
    .Description("Rebuilds all versions of the framework")
    .IsDependentOn("Clean")
    .IsDependentOn("Build");

Task("TestAll")
    .Description("Runs all tests including NUnitLite dogfood tests")
    .IsDependentOn("Test")
    .IsDependentOn("TestNUnitLite");

Task("Package")
    .Description("Packages all versions of the framework")
    .IsDependentOn("PackageFramework")
    .IsDependentOn("PackageZip");



Task("Default")
    .Description("Builds all versions of the framework")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
