namespace nunit.integration.tests.Dsl
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class NUnitRunner
    {
        public TestSession Run(TestContext ctx, CommandLineSetup setup)
        {
            var cmd = Path.Combine(ctx.SandboxPath, "run.cmd");
            File.WriteAllText(
                cmd,
                $"@pushd \"{ctx.CurrentDirectory}\""
                + Environment.NewLine + $"\"{setup.ToolName}\" {setup.Arguments}"
                + Environment.NewLine + "@set exitCode=%errorlevel%"
                + Environment.NewLine + "@popd"
                + Environment.NewLine + "@exit /b %exitCode%");

            var process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            foreach (var envVariable in setup.EnvVariables)
            {
                process.StartInfo.EnvironmentVariables.Add(envVariable.Key, envVariable.Value);
            }

            foreach (var artifact in setup.Artifacts)
            {
                File.WriteAllText(artifact.FileName, artifact.Content);
            }

            process.Start();
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            return new TestSession(ctx, process.StartInfo.FileName, process.StartInfo.Arguments, process.ExitCode, output);            
        }        
    }
}
