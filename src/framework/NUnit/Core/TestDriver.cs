using System;
using System.IO;
using System.Reflection;
using NUnit.Core.Builders;

namespace NUnit.Core
{
    public class TestDriver : MarshalByRefObject
    {
        private TestRunner runner;

        public TestDriver()
        {
            this.runner = new RemoteTestRunner();
        }

        public TestDriver(string runnerType)
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();

            this.runner = (TestRunner)Assembly.GetExecutingAssembly().CreateInstance(runnerType);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region TestControllerHelper Classes

        /// <summary>
        /// TestControllerHelper is the base class for all helpers
        /// used to invoke methods on TestDriver. It provides
        /// the callback for state reporting.
        /// </summary>
        public abstract class TestControllerHelper : MarshalByRefObject
        {
            private AsyncCallback callback;

            protected TestControllerHelper(AsyncCallback callback)
            {
                this.callback = callback;
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void Report(object result, bool isCompleted)
            {
                callback(new TestDriverResult(result, isCompleted));
            }
        }

        public class LoadTests : TestControllerHelper
        {
            public LoadTests(TestDriver driver, string assemblyFilename, AsyncCallback callback) : base(callback)
            {
                Report( driver.runner.Load(new TestPackage(assemblyFilename)), true );
            }
        }

        public class CountTests : TestControllerHelper
        {
            public CountTests(TestDriver driver, AsyncCallback callback) : base(callback)
            {
                Report(driver.runner.CountTestCases(TestFilter.Empty), true);
            }
        }

        public class RunTests : TestControllerHelper, ITestListener
        {
            public RunTests(TestDriver driver, AsyncCallback callback)
                : base(callback)
            {
                Report(driver.runner.Run(this, TestFilter.Empty), true);
            }

            #region ITestListener Members

            public void RunStarted(TestName testName, int testCount)
            {
            }

            public void RunFinished(TestResult result)
            {
            }

            public void RunFinished(Exception exception)
            {
            }

            public void TestStarted(TestName testName)
            {
            }

            public void TestFinished(TestResult result)
            {
            }

            public void SuiteStarted(TestName testName)
            {
            }

            public void SuiteFinished(TestResult result)
            {
            }

            public void UnhandledException(Exception exception)
            {
            }

            public void TestOutput(TestOutput testOutput)
            {
                Report(testOutput, false);
            }

            #endregion
        }

        #endregion

        [Serializable]
        public class TestDriverResult : IAsyncResult
        {
            private object state;
            private bool isCompleted;

            public TestDriverResult(object state, bool isCompleted)
            {
                this.state = state;
                this.isCompleted = isCompleted;
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return state; }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return isCompleted; }
            }

            #endregion
        }
    }
}
