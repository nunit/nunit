using System.Collections;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// MultipleTestDomainRunner runs tests using separate
    /// AppDomains for each assembly.
    /// </summary>
    public class MultipleTestDomainRunner : ITestRunner
    {
        private ArrayList runners = new ArrayList();

        private ServiceContext Services;

        public MultipleTestDomainRunner(ServiceContext services)
        {
            this.Services = services;
        }

        #region ITestRunner Members

        public bool Load(TestPackage package)
        {
            int count = 0;

            foreach (string testFile in package.TestFiles)
            {
                TestPackage subPackage = new TestPackage(testFile);
                foreach (string key in package.Settings.Keys)
                    subPackage.Settings[key] = package.Settings[key];

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
            ArrayList results = new ArrayList();

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
