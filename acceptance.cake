#load lib.cake

using System.Xml.Linq;

Task("Acceptance")
    .Description("Ensures that known project configurations can use the produced NuGet packages to restore, build, and run tests.")
    .IsDependentOn("Acceptance-uap10.0");

Task("Build-AppxRunner")
    .Does(() =>
    {
        MSBuild(@"src\AppxRunner", new MSBuildSettings
        {
            Verbosity = Verbosity.Minimal,
            Configuration = configuration,
            Restore = true
        });
    });

Task("Acceptance-uap10.0")
    .WithCriteria(IsRunningOnWindows())
    .IsDependentOn("Build-AppxRunner")
    .Does(() =>
    {
        DeleteDirectoryRobust(
            @"tests\Isolated package cache\nunit",
            @"tests\Isolated package cache\nunitlite",
            @"tests\uap10.0\uap10.0\bin");

        const string acceptanceTestConfiguration = "Release";

        MSBuild(@"tests\uap10.0", new MSBuildSettings
        {
            WorkingDirectory = @"tests\uap10.0",
            Verbosity = Verbosity.Minimal,
            Configuration = acceptanceTestConfiguration,
            PlatformTarget = PlatformTarget.x86,
            Restore = true
        });

        using (var tempDirectory = new TempDirectory())
        {
            var exitCodeFile = tempDirectory.Path.CombineWithFilePath("exitcode.log");
            var outputFile = tempDirectory.Path.CombineWithFilePath("output.log");
            var resultsFile = tempDirectory.Path.CombineWithFilePath("results.xml");

            RunAppx(
                $@"tests\uap10.0\uap10.0\bin\x86\{acceptanceTestConfiguration}\uap10.0.build.appxrecipe",
                applicationId: "NUnit.AcceptanceTests.Universal.App",
                applicationArgs: new ProcessArgumentBuilder()
                    .AppendQuoted(exitCodeFile.FullPath)
                    .AppendSwitchQuoted("--output", outputFile.FullPath)
                    .AppendSwitchQuoted("--result", resultsFile.FullPath)
                    .Render());

            foreach (var line in System.IO.File.ReadAllLines(outputFile.FullPath))
                Console.WriteLine(line);

            var exitCode = int.Parse(System.IO.File.ReadAllText(exitCodeFile.FullPath));
            if (exitCode != 0) throw new Exception($"NUnitLite exited with code {exitCode}");

            var results = XDocument.Load(resultsFile.FullPath).Root;

            if (results.Attribute("total").Value != "1") throw new Exception("Expected a single test result to be reported.");
            if (results.Attribute("failed").Value != "0") throw new Exception("Expected no tests to fail.");
        }
    });

void RunAppx(string appxRecipePath, string applicationId, string applicationArgs)
{
    StartProcess(
        $@"src\AppxRunner\AppxRunner\bin\{configuration}\net46\AppxRunner.exe",
        new ProcessArgumentBuilder()
            .AppendQuoted(appxRecipePath)
            .AppendQuoted(applicationId)
            .Append(applicationArgs)
            .Render());
}
