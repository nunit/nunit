using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : ITestRunner
    {
        private TestPackage package;
        private ServiceContext services;
        private AbstractTestRunner realRunner;

        // Count of assemblies and projects passed in package
        private int assemblyCount;
        private int projectCount;

        public MasterTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a loaded TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Explore(TestFilter filter)
        {
            return this.realRunner.Explore(filter);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Load(TestPackage package)
        {
            PerformPackageSetup(package);
            return this.realRunner.Load(package);
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public void Unload()
        {
            if (this.realRunner != null)
                this.realRunner.Unload();
        }

        public ITestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            DateTime startTime = DateTime.Now;

            TestEngineResult result = realRunner.Run(listener, filter);

            return TestEngineResult.MakeTestRunResult(this.package, startTime, result);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.realRunner != null)
                this.realRunner.Dispose();
        }

        #endregion

        #region HelperMethods

        private void PerformPackageSetup(TestPackage package)
        {
            this.package = package;

            // Expand projects, updating the count of projects and assemblies
            ExpandProjects();

            // If there is more than one project or a mix of assemblies and 
            // projects, AggregatingTestRunner will call MakeTestRunner for
            // each project or assembly.
            this.realRunner = projectCount > 1 || projectCount > 0 && assemblyCount > 0
                ? new AggregatingTestRunner(services)
                : (AbstractTestRunner)services.TestRunnerFactory.MakeTestRunner(package);
        }

        private void ExpandProjects()
        {
            if (package.TestFiles.Length > 0)
            {
                foreach (string testFile in package.TestFiles)
                {
                    TestPackage subPackage = new TestPackage(testFile);
                    if (services.ProjectService.IsProjectFile(testFile))
                    {
                        services.ProjectService.ExpandProjectPackage(subPackage);
                        projectCount++;
                    }
                    else
                        assemblyCount++;
                }
            }
            else
            {
                if (services.ProjectService.IsProjectFile(package.FullName))
                {
                    services.ProjectService.ExpandProjectPackage(package);
                    projectCount++;
                }
                else
                    assemblyCount++;
            }
        }

        #endregion
    }
}
