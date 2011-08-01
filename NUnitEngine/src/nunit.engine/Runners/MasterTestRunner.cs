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
            AdjustPackageSettings(package);

            this.package = package;
            this.realRunner = GetRealRunner(package);

            return this.realRunner.Explore(package);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Load(TestPackage package)
        {
            AdjustPackageSettings(package);

            this.package = package;
            this.realRunner = GetRealRunner(package);

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

        public static void AdjustPackageSettings(TestPackage package)
        {
            ConvertSettingToEnum(package, "ProcessModel", typeof(ProcessModel));
            ConvertSettingToEnum(package, "DomainUsage", typeof(DomainUsage));
        }

        private static void ConvertSettingToEnum(TestPackage package, string name, Type type)
        {
            if (package.Settings.ContainsKey(name))
            {
                string s = package.Settings[name] as string;
                if (s != null)
                    package.Settings[name] = Enum.Parse(type, s);
            }
        }

        private AbstractTestRunner GetRealRunner(TestPackage package)
        {
            int projectCount = 0;
            int assemblyCount = 0;

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

            if (projectCount > 1 || projectCount > 0 && assemblyCount > 0)
                return new AggregatingTestRunner(services);
            else
                return (AbstractTestRunner)services.TestRunnerFactory.MakeTestRunner(package);

        }

        #endregion
    }
}
