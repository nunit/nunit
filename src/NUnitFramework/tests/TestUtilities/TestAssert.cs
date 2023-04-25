// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    public class TestAssert
    {
        #region IsRunnable
        public static void IsRunnable(Test test)
        {
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        public static void IsRunnable(Type type)
        {
            TestSuite suite = TestBuilder.MakeFixture(type);
            Assert.That(suite, Is.Not.Null, "Unable to construct fixture");
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            ITestResult result = TestBuilder.RunTest(suite, null);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
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
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            //ITestResult result = TestBuilder.RunTest(test, null);
            //Assert.AreEqual(TestStatus.Failed, result.ResultState.Status);
            //Assert.AreEqual("Invalid", result.ResultState.Label);
        }

        public static void IsNotRunnable(Type type)
        {
            TestSuite fixture = TestBuilder.MakeFixture(type);
            Assert.That(fixture, Is.Not.Null, "Unable to construct fixture");
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
