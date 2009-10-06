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
using NUnit.Core;
using NUnit.Framework;

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
            Assert.AreEqual(RunState.Runnable, suite.RunState);
            TestResult result = suite.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.Success, result.ResultState);
        }

        public static void IsRunnable(Type type, string name)
        {
            IsRunnable(type, name, ResultState.Success);
        }

        public static void IsRunnable(Type type, string name, ResultState resultState)
        {
            Test test = TestBuilder.MakeTestCase(type, name);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            if (result.HasResults)
                result = (TestResult)result.Results[0];
            Assert.That(result.ResultState, Is.EqualTo(resultState));
        }
        #endregion

        #region IsNotRunnable
        public static void IsNotRunnable(Test test)
        {
            Assert.AreEqual(RunState.NotRunnable, test.RunState);
            TestResult result = test.Run(NullListener.NULL, TestFilter.Empty);
            Assert.AreEqual(ResultState.NotRunnable, result.ResultState);
        }

        public static void IsNotRunnable(Type type)
        {
            IsNotRunnable(TestBuilder.MakeFixture(type));
        }

        public static void IsNotRunnable(Type type, string name)
        {
            IsNotRunnable(TestBuilder.MakeTestCase(type, name));
        }

        public static void ChildNotRunnable(Type type, string name)
        {
            IsNotRunnable((Test)TestBuilder.MakeTestCase(type, name).Tests[0]);
        }
        #endregion

        private TestAssert() { }
    }
}
