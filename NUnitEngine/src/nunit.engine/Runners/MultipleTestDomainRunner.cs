#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// MultipleTestDomainRunner runs tests using separate
    /// AppDomains for each assembly.
    /// </summary>
    public class MultipleTestDomainRunner : ITestRunner
    {
#if CLR_2_0 || CLR_4_0
        private List<ITestRunner> runners = new List<ITestRunner>();
#else
        private ArrayList runners = new ArrayList();
#endif

        private ServiceContext Services;

        public MultipleTestDomainRunner(ServiceContext services)
        {
            this.Services = services;
        }

        #region ITestRunner Members

        public bool Load(TestPackage package)
        {
            int count = 0;

#if CLR_2_0 || CLR_4_0
            List<TestPackage> packages = new List<TestPackage>();
#else
            ArrayList packages = new ArrayList();
#endif
            if (package.HasSubPackages)
                packages.AddRange(package.SubPackages);
            else
                packages.Add(package);

            foreach (TestPackage subPackage in packages)
            {
                //foreach (string key in package.Settings.Keys)
                //    subPackage.Settings[key] = package.Settings[key];

                ITestRunner runner = new TestDomainRunner(this.Services);
                runners.Add(runner);

                if (runner.Load(subPackage))
                    count++;
            }

            return count == runners.Count;
        }

        public void Unload()
        {
            foreach (ITestRunner runner in runners)
                runner.Unload();

            runners.Clear();
        }

        public TestResult Run(ITestFilter filter)
        {
#if CLR_2_0 || CLR_4_0
            List<TestResult> results = new List<TestResult>();
#else
            ArrayList results = new ArrayList();
#endif

            foreach (ITestRunner runner in runners)
                results.Add(runner.Run(filter));

            switch (results.Count)
            {
                case 0:
                    return null;
                case 1:
                    return (TestResult)results[0];
                default:
                    return XmlHelper.CombineResults(results);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Unload();
        }

        #endregion
    }
}
