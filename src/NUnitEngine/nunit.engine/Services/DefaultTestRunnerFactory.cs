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
using NUnit.Common;
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
        private IProjectService _projectService;

        #region Service Overrides

        public override void StartService()
        {
            // TestRunnerFactory requires the ProjectService
            _projectService = ServiceContext.GetService<IProjectService>();

            // Anything returned from ServiceContext is known to be an IService
            Status = _projectService != null && ((IService)_projectService).Status == ServiceStatus.Started
                ? ServiceStatus.Started
                : ServiceStatus.Error;
        }

        #endregion

        #region InProcessTestRunnerFactory Overrides
        
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

            int assemblyCount = 0;
            int projectCount = 0;

            foreach (var subPackage in package.SubPackages)
            {
                var testFile = subPackage.FullName;

                if (PathUtils.IsAssemblyFileType(testFile))
                    assemblyCount++;
                else if (_projectService.CanLoadFrom(testFile))
                    projectCount++;
            }

            // If we have multiple projects or a project plus assemblies
            // then defer to the AggregatingTestRunner, which will make
            // the decision on a file by file basis so that each project
            // runs with its own settings.
            if (projectCount > 1 || projectCount > 0 && assemblyCount > 0)
                return new AggregatingTestRunner(ServiceContext, package);

            // If we have a single project by itself, make it the top level project.
            if (projectCount > 0 && assemblyCount == 0)
                package = package.SubPackages[0];

            // TODO: What about bad extensions?

            ProcessModel processModel = GetTargetProcessModel(package);

            switch (processModel)
            {
                default:
                case ProcessModel.Default:
                    if (package.SubPackages.Count > 1)
                        return new MultipleTestProcessRunner(this.ServiceContext, package);
                    else
                        return new ProcessRunner(this.ServiceContext, package);

                case ProcessModel.Multiple:
                    return new MultipleTestProcessRunner(this.ServiceContext, package);

                case ProcessModel.Separate:
                    return new ProcessRunner(this.ServiceContext, package);

                case ProcessModel.InProcess:
                    return base.MakeTestRunner(package);
            }
        }

        // TODO: Review this method once we have a gui - not used by console runner
        public override bool CanReuse(ITestEngineRunner runner, TestPackage package)
        {
            ProcessModel processModel = GetTargetProcessModel(package);

            switch (processModel)
            {
                case ProcessModel.Default:
                case ProcessModel.Multiple:
                    return runner is MultipleTestProcessRunner;
                case ProcessModel.Separate:
                    return runner is ProcessRunner;
                default:
                    return base.CanReuse(runner, package);
            }
        }

        #endregion

        #region Helper Methods

        private ProcessModel GetTargetProcessModel(TestPackage package)
        {
            return (ProcessModel)System.Enum.Parse(
                typeof(ProcessModel),
                package.GetSetting(PackageSettings.ProcessModel, "Default"));
        }

        #endregion
    }
}
