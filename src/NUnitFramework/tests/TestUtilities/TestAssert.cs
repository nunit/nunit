// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities
{
    public class TestAssert
    {
        #region IsRunnable
        public static void IsRunnable(Test test)
        {
            Assert.AreEqual(RunState.Runnable, test.RunState);
        }

        public static void IsRunnable(Type type)
        {
            TestSuite suite = TestBuilder.MakeFixture(type);
            Assert.NotNull(suite, "Unable to construct fixture");
            Assert.AreEqual(RunState.Runnable, suite.RunState);
            ITestResult result = TestBuilder.RunTest(suite, null);
            Assert.AreEqual(ResultState.Success, result.ResultState);
        }

        public static void IsRunnable(Type type, string name)
        {
            IsRunnable(type, name, ResultState.Success);
        }

        public static void IsRunnable(Type type, string name, ResultState resultState)
        {
            Test test = TestBuilder.MakeTestFromMethod(type, name);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
            object testObject = Reflect.Construct(type);
            ITestResult result = TestBuilder.RunTest(test, testObject);
            if (result.HasChildren) // In case it's a parameterized method
                result = result.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(resultState));
        }
        #endregion

        #region IsNotRunnable
        public static void IsNotRunnable(Test test)
        {
            Assert.AreEqual(RunState.NotRunnable, test.RunState);
            //ITestResult result = TestBuilder.RunTest(test, null);
            //Assert.AreEqual(TestStatus.Failed, result.ResultState.Status);
            //Assert.AreEqual("Invalid", result.ResultState.Label);
        }

        public static void IsNotRunnable(Type type)
        {
            TestSuite fixture = TestBuilder.MakeFixture(type);
            Assert.NotNull(fixture, "Unable to construct fixture");
            IsNotRunnable(fixture);
        }

        public static void IsNotRunnable(Type type, string name)
        {
            IsNotRunnable(TestBuilder.MakeTestFromMethod(type, name));
        }

        public static void ChildNotRunnable(Type type, string name)
        {
            IsNotRunnable((Test)TestBuilder.MakeParameterizedMethodSuite(type, name).Tests[0]);
        }
        #endregion

        private TestAssert() { }
    }
}
