// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
            IsNotRunnable(TestBuilder.MakeTestCase(type, name));
        }

        public static void ChildNotRunnable(Type type, string name)
        {
            IsNotRunnable((Test)TestBuilder.MakeParameterizedMethodSuite(type, name).Tests[0]);
        }
        #endregion

        private TestAssert() { }
    }
}
