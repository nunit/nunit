using System;
using System.Reflection;
using NUnit.Framework.Api;
using System.Diagnostics;

namespace NUnit.Framework.Internal
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestSuiteCommand : TestCommand
    {
        private readonly TestSuite suite;
        private readonly Type fixtureType;
        private readonly Object[] arguments;
        private TestSuiteResult suiteResult;

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="test"></param>
        public TestSuiteCommand(TestSuite test) : base(test)
        {
            this.suite = test;
            this.fixtureType = test.FixtureType;
            this.arguments = test.arguments;
        }

        /// <summary>
        /// TODO: Documentation needed for method
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(object testObject, ITestListener listener)
        {
            this.suiteResult = CurrentResult as TestSuiteResult;
            Debug.Assert(suiteResult != null);

            bool oneTimeSetUpComplete = false;

            try
            {
                ApplySettingsToExecutionContext();

                if (testObject == null && fixtureType != null && !IsStaticClass(fixtureType))
                    testObject = Reflect.Construct(fixtureType, arguments);

                DoOneTimeSetUp(testObject);
                oneTimeSetUpComplete = true;

                // SetUp may have changed some things
                TestExecutionContext.CurrentContext.Update();

                this.suiteResult = RunChildCommands(testObject, listener);
            }
            catch (Exception ex)
            {
                if (ex is NUnitException || ex is System.Reflection.TargetInvocationException)
                    ex = ex.InnerException;


                if (oneTimeSetUpComplete)
                    this.suiteResult.RecordException(ex);
                else
                    this.suiteResult.RecordException(ex, FailureSite.SetUp);
            }
            finally
            {
                DoOneTimeTearDown(testObject);
            }

            return this.suiteResult;
        }

        /// <summary>
        /// Does the one time set up.
        /// </summary>
        /// <param name="testObject">The object to use in running the test.</param>
        protected virtual void DoOneTimeSetUp(object testObject)
        {
            if (fixtureType != null)
            {
                if (Test.OneTimeSetUpMethods != null)
                    foreach (MethodInfo method in Test.OneTimeSetUpMethods)
                        Reflect.InvokeMethod(method, method.IsStatic ? null : testObject);
            }
        }

        /// <summary>
        /// Does the one time tear down.
        /// </summary>
        /// <param name="testObject"></param>
        protected virtual void DoOneTimeTearDown(object testObject)
        {
            if (fixtureType != null)
            {
                try
                {
                    if (Test.OneTimeTearDownMethods != null)
                    {
                        int index = Test.OneTimeTearDownMethods.Length;
                        while (--index >= 0)
                        {
                            MethodInfo fixtureTearDown = Test.OneTimeTearDownMethods[index];
                            Reflect.InvokeMethod(fixtureTearDown, fixtureTearDown.IsStatic ? null : testObject);
                        }
                    }

                    IDisposable disposable = testObject as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
                catch (Exception ex)
                {
                    // Error in TestFixtureTearDown or Dispose causes the
                    // suite to be marked as a error, even if
                    // all the contained tests passed.
                    NUnitException nex = ex as NUnitException;
                    if (nex != null)
                        ex = nex.InnerException;

                    // TODO: Can we move this logic into TestResult itself?
                    string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
                    if (suiteResult.Message != null)
                        message = suiteResult.Message + NUnit.Env.NewLine + message;

#if !NETCF_1_0
                    string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
                    if (suiteResult.StackTrace != null)
                        stackTrace = suiteResult.StackTrace + NUnit.Env.NewLine + stackTrace;

                    // TODO: What about ignore exceptions in teardown?
                    suiteResult.SetResult(ResultState.Error, message, stackTrace);
#else
                    Result.SetResult(ResultState.Error, message);
#endif
                }
            }
        }

        //private TestSuiteResult RunChildTests()
        //{
        //    this.suiteResult.SetResult(ResultState.Success);

        //    foreach (Test test in Test.Tests)
        //    {
        //        if (suite.Filter.Pass(test))
        //        {
        //            TestResult childResult = test.Run(suite.Listener, suite.Filter);

        //            this.suiteResult.AddResult(childResult);

        //            if (childResult.ResultState == ResultState.Cancelled)
        //                break;
        //        }
        //    }

        //    return this.suiteResult;
        //}

        private TestSuiteResult RunChildCommands(object testObject, ITestListener listener)
        {
            this.suiteResult.SetResult(ResultState.Success);

            foreach (TestCommand command in Children)
            {
                TestResult childResult = command.Execute(testObject, listener);

                this.suiteResult.AddResult(childResult);

                if (childResult.ResultState == ResultState.Cancelled)
                    break;
            }

            return this.suiteResult;
        }

        private static bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        /// <summary>
        /// Applies the culture settings specified on the test
        /// to the TestExecutionContext.
        /// </summary>
        private void ApplySettingsToExecutionContext()
        {
#if !NETCF
            string setCulture = (string)suite.Properties.Get(PropertyNames.SetCulture);
            if (setCulture != null)
                TestExecutionContext.CurrentContext.CurrentCulture =
                    new System.Globalization.CultureInfo(setCulture);

            string setUICulture = (string)suite.Properties.Get(PropertyNames.SetUICulture);
            if (setUICulture != null)
                TestExecutionContext.CurrentContext.CurrentUICulture =
                    new System.Globalization.CultureInfo(setUICulture);
#endif

#if !NUNITLITE
            if (suite.Properties.ContainsKey(PropertyNames.Timeout))
                TestExecutionContext.CurrentContext.TestCaseTimeout = (int)suite.Properties.Get(PropertyNames.Timeout);
#endif
        }
    }
}
