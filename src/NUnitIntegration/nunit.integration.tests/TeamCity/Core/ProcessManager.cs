// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
