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
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Explore(TestPackage package)
        {
            PerformPackageSetup(package);

            return this.realRunner.Explore(package);
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

        public ITestEngineResult Run(ITestEventHandler listener, ITestFilter filter)
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

            // Convert certain package settings, specified as strings,
            // to their internal representation before further use.
            ConvertPackageSetting("ProcessModel");
            ConvertPackageSetting("DomainUsage");
            ConvertPackageSetting("RuntimeFramework");
            ConvertPackageSetting("InternalTraceLevel");

            // Expand projects, updating the count of projects and assemblies
            ExpandProjects();

            // If there is more than one project or a mix of assemblies and 
            // projects, AggregatingTestRunner will call MakeTestRunner for
            // each project or assembly.
            this.realRunner = projectCount > 1 || projectCount > 0 && assemblyCount > 0
                ? new AggregatingTestRunner(services)
                : (AbstractTestRunner)services.TestRunnerFactory.MakeTestRunner(package);
        }

        /// <summary>
        /// Convert a single setting, throwing an NUnitEngineException
        /// if the setting cannot be converted.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        private void ConvertPackageSetting(string name)
        {
            if (this.package.Settings.ContainsKey(name))
            {
                string value = package.Settings[name] as string;

                if (value != null)
                    try
                    {
                        switch (name)
                        {
                            case "ProcessModel":
                                package.Settings[name] = Enum.Parse(typeof(ProcessModel), value);
                                break;

                            case "DomainUsage":
                                package.Settings[name] = Enum.Parse(typeof(DomainUsage), value);
                                break;

                            case "InternalTraceLevel":
                                package.Settings[name] = Enum.Parse(typeof(InternalTraceLevel), value);
                                break;

                            case "RuntimeFramework":
                                package.Settings[name] = RuntimeFramework.Parse(value);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        string msg = string.Format("Invalid {0} setting: {1}", name, value);
                        throw new NUnitEngineException(msg);
                    }
            }
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
