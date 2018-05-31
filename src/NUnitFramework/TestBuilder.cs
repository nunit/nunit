// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
#if !NETCOREAPP1_1
using NUnit.Compatibility;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Utility Class used to build and run NUnit tests used as test data
    /// </summary>
    public static class TestBuilder
    {
        #region Build Tests

        public static TestSuite MakeSuite(string name)
        {
            return new TestSuite(name);
        }

        public static TestSuite MakeFixture(Type type)
        {
            return (TestSuite)new DefaultSuiteBuilder().BuildFrom(type);
        }

        public static TestSuite MakeFixture(object fixture)
        {
            TestSuite suite = MakeFixture(fixture.GetType());
            suite.Fixture = fixture;
            return suite;
        }

        public static TestSuite MakeParameterizedMethodSuite(Type type, string methodName)
        {
            var suite = MakeTestFromMethod(type, methodName) as TestSuite;
            Assert.NotNull(suite, "Unable to create parameterized suite - most likely there is no data provided");
            return suite;
        }

        public static TestMethod MakeTestCase(Type type, string methodName)
        {
            var test = MakeTestFromMethod(type, methodName) as TestMethod;
            Assert.NotNull(test, "Unable to create TestMethod from {0}", methodName);

            return test;
        }

        // Will return either a ParameterizedMethodSuite or an NUnitTestMethod
        // depending on whether the method takes arguments or not
        internal static Test MakeTestFromMethod(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
                Assert.Fail("Method not found: " + methodName);
            return new DefaultTestCaseBuilder().BuildFrom(new FixtureMethod(type, method));
        }

        #endregion

        #region Create WorkItems

        public static WorkItem CreateWorkItem(Type type)
        {
            return CreateWorkItem(MakeFixture(type));
        }

        public static WorkItem CreateWorkItem(Type type, string methodName)
        {
            return CreateWorkItem(MakeTestFromMethod(type, methodName));
        }

        public static WorkItem CreateWorkItem(Test test)
        {
            var context = new TestExecutionContext();
            context.Dispatcher = new SuperSimpleDispatcher();

            return CreateWorkItem(test, context);
        }

        public static WorkItem CreateWorkItem(Test test, object testObject)
        {
            var context = new TestExecutionContext();
            context.TestObject = testObject;
            context.Dispatcher = new SuperSimpleDispatcher();

            return CreateWorkItem(test, context);
        }

        public static WorkItem CreateWorkItem(Test test, TestExecutionContext context)
        {
            var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, true);
            work.InitializeContext(context);

            return work;
        }

        #endregion

        #region Run Tests

        public static ITestResult RunTestFixture(Type type)
        {
            return RunTest(MakeFixture(type), null);
        }

        public static ITestResult RunTestFixture(object fixture)
        {
            return RunTest(MakeFixture(fixture), fixture);
        }

        public static ITestResult RunParameterizedMethodSuite(Type type, string methodName)
        {
            var suite = MakeParameterizedMethodSuite(type, methodName);

            object testObject = null;
            if (!type.IsStatic())
                testObject = Reflect.Construct(type);

            return RunTest(suite, testObject);
        }

        public static ITestResult RunTestCase(Type type, string methodName)
        {
            var testMethod = MakeTestCase(type, methodName);

            object testObject = null;
            if (!type.IsStatic())
                testObject = Reflect.Construct(type);

            return RunTest(testMethod, testObject);
        }

        public static ITestResult RunTestCase(object fixture, string methodName)
        {
            var testMethod = MakeTestCase(fixture.GetType(), methodName);

            return RunTest(testMethod, fixture);
        }

        public static ITestResult RunAsTestCase(Action action)
        {
            var method = action.GetMethodInfo();
            var testMethod = MakeTestCase(method.DeclaringType, method.Name);
            return RunTest(testMethod);
        }

        public static ITestResult RunTest(Test test)
        {
            return RunTest(test, null);
        }

        public static ITestResult RunTest(Test test, object testObject)
        {
            return ExecuteWorkItem(CreateWorkItem(test, testObject));
        }

        public static ITestResult ExecuteWorkItem(WorkItem work)
        {
            work.Execute();

            // TODO: Replace with an event - but not while method is static
            while (work.State != WorkItemState.Complete)
            {
                Thread.Sleep(1);
            }

            return work.Result;
        }

        #endregion

        #region Nested TestDispatcher Class

        /// <summary>
        /// SuperSimpleDispatcher merely executes the work item.
        /// It is needed particularly when running suites, since
        /// the child items require a dispatcher in the context.
        /// </summary>
        class SuperSimpleDispatcher : IWorkItemDispatcher
        {
            public int LevelOfParallelism { get { return 0; } }

            public void Start(WorkItem topLevelWorkItem)
            {
                topLevelWorkItem.Execute();
            }

            public void Dispatch(WorkItem work)
            {
                work.Execute();
            }

            public void CancelRun(bool force)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
