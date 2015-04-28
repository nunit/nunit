using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ProcessManager : IProcessManager
    {
        public ProcessOutputDto StartProcess(string cmdLineFileName, IEnumerable<string> args, IDictionary<string, string> environmentVariables)
        {
            Contract.Requires<ArgumentNullException>(cmdLineFileName != null);
            Contract.Requires<ArgumentNullException>(args != null);
            Contract.Requires<ArgumentNullException>(environmentVariables != null);
            Contract.Ensures(Contract.Result<ProcessOutputDto>() != null);

            var process = new Process();
            process.StartInfo.FileName = cmdLineFileName;
            process.StartInfo.Arguments = string.Join(" ", args);
            foreach (var envVar in environmentVariables)
            {
                process.StartInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
            
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            var output = new List<string>();
            while (!process.StandardOutput.EndOfStream)
            {
                output.Add(process.StandardOutput.ReadLine());
            }

            process.WaitForExit();
            return new ProcessOutputDto(process.ExitCode, output);
        }
    }
}
