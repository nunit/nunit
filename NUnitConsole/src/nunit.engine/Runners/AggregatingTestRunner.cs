using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// AggregatingTestRunner runs tests using multiple
    /// subordinate runners and combines the results.
    /// </summary>
    public class AggregatingTestRunner : AbstractTestRunner
    {
        private List<ITestRunner> runners = new List<ITestRunner>();

        public AggregatingTestRunner(ServiceContext services) : base(services) { }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Explore a TestPackage and return information about
        /// the tests found.
        /// </summary>
        /// <param name="package">The TestPackage to be explored</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Explore(TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            // TODO: Eliminate need for implicit cast to AbstractTestRunner
            foreach (AbstractTestRunner runner in runners)
                results.Add(runner.Explore(filter));

            return MakePackageResult(results);
        }

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.package = package;

            List<TestPackage> packages = new List<TestPackage>();

            foreach (string testFile in package.TestFiles)
            {
                TestPackage subPackage = new TestPackage(testFile);
                if (Services.ProjectService.IsProjectFile(testFile))
                    Services.ProjectService.ExpandProjectPackage(subPackage);
                foreach (string key in package.Settings.Keys)
                    subPackage.Settings[key] = package.Settings[key];
                packages.Add(subPackage);
            }

            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (TestPackage subPackage in packages)
            {
                //foreach (string key in package.Settings.Keys)
                //    subPackage.Settings[key] = package.Settings[key];

                AbstractTestRunner runner = CreateRunner(subPackage);
                runners.Add(runner);
                results.Add(runner.Load(subPackage));
            }

            return TestEngineResult.Wrap("load", results);
        }

        /// <summary>
        /// Unload any loaded TestPackages and clear the
        /// list of runners.
        /// </summary>
        public override void Unload()
        {
            foreach (ITestRunner runner in runners)
                runner.Unload();

            runners.Clear();
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>
        /// A TestEngineResult giving the result of the test execution.
        /// </returns>
        public override TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            // TODO: Eliminate need for implicit cast to AbstractTestRunner
            foreach (AbstractTestRunner runner in runners)
                results.Add(runner.Run(listener, filter));

            return MakePackageResult(results);
        }

        #endregion

        protected virtual AbstractTestRunner CreateRunner(TestPackage package)
        {
            return Services.TestRunnerFactory.MakeTestRunner(package) as AbstractTestRunner;
        }
    }
}
