// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Internal.Abstractions;

#nullable enable

namespace NUnit.Framework.Tests.TestUtilities;

/// <summary>
/// Utility Class used to build and run NUnit tests used as test data
/// </summary>
internal static class TestBuilder
{
    #region Build Tests

    public static TestSuite MakeSuite(string name)
    {
        return new TestSuite(name);
    }

    public static TestSuite MakeFixture(Type type)
    {
        return new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(type));
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
        Assert.That(suite, Is.Not.Null, "Unable to create parameterized suite - most likely there is no data provided");
        return suite;
    }

    public static TestMethod MakeTestCase(Type type, string methodName)
    {
        var test = MakeTestFromMethod(type, methodName) as TestMethod;
        Assert.That(test, Is.Not.Null, $"Unable to create TestMethod from {methodName}");

        return test!;
    }

    // Will return either a ParameterizedMethodSuite or an NUnitTestMethod
    // depending on whether the method takes arguments or not
    internal static Test MakeTestFromMethod(Type type, string methodName)
    {
        return new DefaultTestCaseBuilder().BuildFrom(new MethodWrapper(type, methodName));
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
        var context = new TestExecutionContext
        {
            Dispatcher = new SuperSimpleDispatcher()
        };

        return CreateWorkItem(test, context);
    }

    public static WorkItem CreateWorkItem(Test test, object? testObject, IDebugger? debugger = null)
    {
        var context = new TestExecutionContext
        {
            TestObject = testObject,
            Dispatcher = new SuperSimpleDispatcher()
        };

        return CreateWorkItem(test, context, debugger);
    }

    public static WorkItem CreateWorkItem(Test test, TestExecutionContext context, IDebugger? debugger = null)
    {
        var work = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, debugger ?? new DebuggerProxy(), true);
        Assert.That(work, Is.Not.Null);
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

        object? testObject = null;
        if (!type.IsStatic())
            testObject = Reflect.Construct(type);

        return RunTest(suite, testObject);
    }

    public static ITestResult RunTestCase(Type type, string methodName)
    {
        var testMethod = MakeTestCase(type, methodName);

        object? testObject = null;
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
        Assert.That(method, Is.Not.Null);
        Assert.That(method.DeclaringType, Is.Not.Null);
        var testMethod = MakeTestCase(method.DeclaringType, method.Name);
        return RunTest(testMethod);
    }

    public static ITestResult RunTest(Test test)
    {
        return RunTest(test, null);
    }

    public static ITestResult RunTest(Test test, object? testObject, IDebugger? debugger = null)
    {
        return ExecuteWorkItem(CreateWorkItem(test, testObject, debugger ?? new DebuggerProxy()));
    }

    public static ITestResult ExecuteWorkItem(WorkItem work)
    {
        var savedContext = TestExecutionContext.CurrentContext;

        try
        {
            return ExecuteUntilComplete(work);
        }
        finally
        {
            savedContext.EstablishExecutionEnvironment();
        }

        static ITestResult ExecuteUntilComplete(WorkItem work)
        {
            work.Execute();

            // TODO: Replace with an event - but not while method is static
            while (work.State != WorkItemState.Complete)
            {
                Thread.Sleep(1);
            }

            return work.Result;
        }
    }

    #endregion

    #region Nested TestDispatcher Class

    /// <summary>
    /// SuperSimpleDispatcher merely executes the work item.
    /// It is needed particularly when running suites, since
    /// the child items require a dispatcher in the context.
    /// </summary>
    private class SuperSimpleDispatcher : IWorkItemDispatcher
    {
        public int LevelOfParallelism => 0;

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
