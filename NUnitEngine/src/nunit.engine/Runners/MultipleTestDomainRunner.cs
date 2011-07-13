using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// MultipleTestDomainRunner runs tests using separate
    /// AppDomains for each assembly.
    /// </summary>
    public class MultipleTestDomainRunner : AbstractTestRunner
    {
        private List<ITestRunner> runners = new List<ITestRunner>();

        private ServiceContext Services;

        public MultipleTestDomainRunner(ServiceContext services)
        {
            this.Services = services;
        }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            List<TestPackage> packages = new List<TestPackage>();
            if (package.HasSubPackages)
                packages.AddRange(package.SubPackages);
            else
                packages.Add(package);

            StringBuilder sb = new StringBuilder("<load>");

            foreach (TestPackage subPackage in packages)
            {
                //foreach (string key in package.Settings.Keys)
                //    subPackage.Settings[key] = package.Settings[key];

                ITestRunner runner = new TestDomainRunner(this.Services);
                runners.Add(runner);

                var result = runner.Load(subPackage);
                sb.Append(result.Text);
            }

            sb.Append("</load>");

            return new TestEngineResult(sb.ToString());
        }

        /// <summary>
        /// Unload any loaded TestPackage. If none is loaded,
        /// the call is ignored.
        /// </summary>
        public override void Unload()
        {
            foreach (ITestRunner runner in runners)
                runner.Unload();

            runners.Clear();
        }

        public override TestEngineResult[] RunDirect(ITestEventHandler listener, ITestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (ITestRunner runner in runners)
                results.AddRange(runner.RunDirect(listener, filter));

            return results.ToArray();
        }

        #endregion
    }
}
