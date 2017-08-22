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
#if !NETCOREAPP1_0
using NUnit.Compatibility;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;

#if NETSTANDARD1_3 && !NETSTANDARD1_6 && !NETCOREAPP1_0
using BindingFlags = NUnit.Compatibility.BindingFlags;
#endif

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Utility Class used to build and run NUnit tests used as test data
    /// </summary>
    public static class TestBuilder
    {
#region Build Tests

        public static TestSuite MakeFixture(Type type)
        {
            return (TestSuite)new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(type));
        }

        public static TestSuite MakeFixture(object fixture)
        {
            TestSuite suite = MakeFixture(fixture.GetType());
            suite.Fixture = fixture;
            return suite;
        }

        public static TestSuite MakeFixture(List<Type> types)
        {
            // following the pattern found in DefaultTestAssemblyBuilder
            var fixtures = new List<Test>();
            var defaultSuiteBuilder = new DefaultSuiteBuilder();
            foreach (var type in types)
            {
                var typeInfo = new TypeWrapper(type);
                var test = defaultSuiteBuilder.BuildFrom(typeInfo);
                fixtures.Add(test);
            }

            var assembly = AssemblyHelper.Load("nunit.testdata");
            var assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
            TestSuite testSuite = new TestAssembly(assembly, assemblyPath);

            var treeBuilder = new NamespaceTreeBuilder(testSuite);
            treeBuilder.Add(fixtures);

            return treeBuilder.RootSuite;
        }

        public static TestSuite MakeParameterizedMethodSuite(Type type, string methodName)
        {
            var suite = MakeTestFromMethod(type, methodName) as TestSuite;
            Assert.NotNull(suite, "Unable to create parameterized suite - most likely there is no data provided");
            return suite;
        }

        public static TestSuite MakeParameterizedMethodSuite(object fixture, string methodName)
        {
            var test = MakeTestFromMethod(fixture.GetType(), methodName) as ParameterizedMethodSuite;
            Assert.That(test, Is.TypeOf<ParameterizedMethodSuite>());

            TestSuite suite = test as TestSuite;
            suite.Fixture = fixture;
            return suite;
        }

        public static TestMethod MakeTestCase(Type type, string methodName)
        {
            var test = MakeTestFromMethod(type, methodName);
            Assert.That(test, Is.TypeOf<TestMethod>());

            return (TestMethod)test;
        }

        // Will return either a ParameterizedMethodSuite or an NUnitTestMethod
        // depending on whether the method takes arguments or not
        internal static Test MakeTestFromMethod(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
                Assert.Fail("Method not found: " + methodName);
            return new DefaultTestCaseBuilder().BuildFrom(new MethodWrapper(type, method));
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
            if (!IsStaticClass(type))
                testObject = Reflect.Construct(type);

            return RunTest(suite, testObject);
        }

        public static ITestResult RunTestCase(Type type, string methodName)
        {
            var testMethod = MakeTestCase(type, methodName);

            object testObject = null;
            if (!IsStaticClass(type))
                testObject = Reflect.Construct(type);

            return RunTest(testMethod, testObject);
        }

        public static ITestResult RunTestCase(object fixture, string methodName)
        {
            var testMethod = MakeTestCase(fixture.GetType(), methodName);

            return RunTest(testMethod, fixture);
        }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
        public static ITestResult RunAsTestCase(Action action)
        {
            var method = action.Method;
            var testMethod = MakeTestCase(method.DeclaringType, method.Name);
            return RunTest(testMethod);
        }
#endif

        public static ITestResult RunTest(Test test)
        {
            return RunTest(test, null);
        }

        public static ITestResult RunTest(Test test, object testObject)
        {
            return ExecuteWorkItem(PrepareWorkItem(test, testObject));
        }

        // NOTE: The following two methods are separate in order to support
        // tests that need to access the WorkItem before or after execution.

        public static WorkItem PrepareWorkItem(Test test, object testObject)
        {
            var context = new TestExecutionContext();
            context.TestObject = testObject;
            context.Dispatcher = new SuperSimpleDispatcher();

            var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, true);
            work.InitializeContext(context);

            return work;
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

#region Helper Methods

        private static bool IsStaticClass(Type type)
        {
            return type.GetTypeInfo().IsAbstract && type.GetTypeInfo().IsSealed;
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
