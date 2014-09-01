// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using NUnit.Engine.Internal;
using NUnit.Engine.Runners;

namespace NUnit.Engine.Services
{
    /// <summary>
    /// DefaultTestRunnerFactory handles creation of a suitable test 
    /// runner for a given package to be loaded and run either in a 
    /// separate process or within the same process. 
    /// </summary>
    public class DefaultTestRunnerFactory : InProcessTestRunnerFactory, ITestRunnerFactory
    {
        /// <summary>
        /// Returns a test runner based on the settings in a TestPackage.
        /// Any setting that is "consumed" by the factory is removed, so
        /// that downstream runners using the factory will not repeatedly
        /// create the same type of runner.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded and run</param>
        /// <returns>A TestRunner</returns>
        public override ITestEngineRunner MakeTestRunner(TestPackage package)
        {
            ProcessModel processModel = GetTargetProcessModel(package);

            switch (processModel)
            {
                case ProcessModel.Multiple:
                    package.Settings.Remove("ProcessModel");
                    return new MultipleTestProcessRunner(this.ServiceContext, package);
                case ProcessModel.Separate:
                    package.Settings.Remove("ProcessModel");
                    return new ProcessRunner(this.ServiceContext, package);
                default:
                    return base.MakeTestRunner(package);
            }
        }

        public override bool CanReuse(ITestEngineRunner runner, TestPackage package)
        {
            RuntimeFramework currentFramework = RuntimeFramework.CurrentFramework;
            RuntimeFramework targetFramework = ServiceContext.RuntimeFrameworkSelector.SelectRuntimeFramework(package);

            ProcessModel processModel = (ProcessModel)System.Enum.Parse(
                typeof(ProcessModel),
                package.GetSetting(RunnerSettings.ProcessModel, "Default"));
            if (processModel == ProcessModel.Default)
                if (!currentFramework.Supports(targetFramework))
                    processModel = ProcessModel.Separate;

            switch (processModel)
            {
                case ProcessModel.Multiple:
                    return runner is MultipleTestProcessRunner;
                case ProcessModel.Separate:
                    ProcessRunner processRunner = runner as ProcessRunner;
                    return processRunner != null && processRunner.RuntimeFramework == targetFramework;
                default:
                    return base.CanReuse(runner, package);
            }
        }

        private ProcessModel GetTargetProcessModel(TestPackage package)
        {
            RuntimeFramework currentFramework = RuntimeFramework.CurrentFramework;
            RuntimeFramework targetFramework = ServiceContext.RuntimeFrameworkSelector.SelectRuntimeFramework(package);

            ProcessModel processModel = (ProcessModel)System.Enum.Parse(
                typeof(ProcessModel),
                package.GetSetting(RunnerSettings.ProcessModel, "Default"));

            if (processModel == ProcessModel.Default)
                if (!currentFramework.Supports(targetFramework))
                    processModel = ProcessModel.Separate;

            return processModel;
        }
    }
}
