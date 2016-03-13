// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.IO;

namespace NUnit.Common
{
    /// <summary>
    /// ConsoleOptions encapsulates the option settings for
    /// the nunit3-console program.
    /// </summary>
    public class ConsoleOptions : CommandLineOptions
    {
        #region Constructors

        internal ConsoleOptions(IDefaultOptionsProvider provider, params string[] args) : base(provider, args) { }

        public ConsoleOptions(params string[] args) : base(args) { }

        #endregion

        #region Properties

        public string ActiveConfig { get; private set; }
        public bool ActiveConfigSpecified { get { return ActiveConfig != null; } }

        // Where to Run Tests

        public string ProcessModel { get; private set; }
        public bool ProcessModelSpecified { get { return ProcessModel != null; } }

        public string DomainUsage { get; private set; }
        public bool DomainUsageSpecified { get { return DomainUsage != null; } }

        // How to Run Tests

        public string Framework { get; private set; }
        public bool FrameworkSpecified { get { return Framework != null; } }

        public bool RunAsX86 { get; private set; }

        public bool DisposeRunners { get; private set; }

        public bool ShadowCopyFiles { get; private set; }

        private int maxAgents = -1;
        public int MaxAgents { get { return maxAgents; } }
        public bool MaxAgentsSpecified { get { return maxAgents >= 0; } }

        public bool DebugTests { get; private set; }

        public bool DebugAgent { get; private set; }

        public bool PauseBeforeRun { get; private set; }

        #endregion

        #region Overrides

        protected override void CheckOptionCombinations()
        {
            base.CheckOptionCombinations();

            if (RunAsX86 && ProcessModel == "InProcess")
                ErrorMessages.Add("The --x86 and --inprocess options are incompatible.");
        }

        #endregion

        #region Configure Additional Options for Console

        protected override void ConfigureOptions()
        {
            base.ConfigureOptions();

            this.Add("config=", "{NAME} of a project configuration to load (e.g.: Debug).",
                v => ActiveConfig = RequiredValue(v, "--config"));

            // Where to Run Tests
            this.Add("process=", "{PROCESS} isolation for test assemblies.\nValues: InProcess, Separate, Multiple. If not specified, defaults to Separate for a single assembly or Multiple for more than one.",
                v =>
                {
                    ProcessModel = RequiredValue(v, "--process", "Single", "InProcess", "Separate", "Multiple");
                    // Change so it displays correctly even though it isn't absolutely needed
                    if (ProcessModel.ToLower() == "single")
                        ProcessModel = "InProcess";
                });

            this.Add("inprocess", "Synonym for --process:InProcess",
                v => ProcessModel = "InProcess");

            this.Add("domain=", "{DOMAIN} isolation for test assemblies.\nValues: None, Single, Multiple. If not specified, defaults to Single for a single assembly or Multiple for more than one.",
                v => DomainUsage = RequiredValue(v, "--domain", "None", "Single", "Multiple"));

            // How to Run Tests
            this.Add("framework=", "{FRAMEWORK} type/version to use for tests.\nExamples: mono, net-3.5, v4.0, 2.0, mono-4.0. If not specified, tests will run under the framework they are compiled with.",
                v => Framework = RequiredValue(v, "--framework"));

            this.Add("x86", "Run tests in an x86 process on 64 bit systems",
                v => RunAsX86 = v != null);

            this.Add("dispose-runners", "Dispose each test runner after it has finished running its tests.",
                v => DisposeRunners = v != null);

            this.Add("shadowcopy", "Shadow copy test files",
                v => ShadowCopyFiles = v != null);

            this.Add("agents=", "Specify the maximum {NUMBER} of test assembly agents to run at one time. If not specified, there is no limit.",
                v => maxAgents = RequiredInt(v, "--agents"));

            this.Add("debug", "Launch debugger to debug tests.",
                v => DebugTests = v != null);

            this.Add("pause", "Pause before running to allow attaching a debugger.",
                v => PauseBeforeRun = v != null);

#if DEBUG
            this.Add("debug-agent", "Launch debugger in nunit-agent when it starts.",
                v => DebugAgent = v != null);
#endif
        }

        #endregion
    }
}
