// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.HookExtensions;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension;

internal class ActivateHookLogging : NUnitAttribute, IApplyToContext
{
    public static LoggerHook LoggingHook = null!;

    public virtual void ApplyToContext(TestExecutionContext context)
    {
        LoggingHook = new LoggerHook();
        context?.HookExtension?.BeforeAnySetUps.AddHandler((sender, eventArgs) => LoggingHook.BeforeAnySetUps(sender, eventArgs));
        context?.HookExtension?.AfterAnySetUps.AddHandler((sender, eventArgs) => LoggingHook.AfterAnySetUps(sender, eventArgs));
        context?.HookExtension?.BeforeTest.AddHandler((sender, eventArgs) => LoggingHook.BeforeTest(sender, eventArgs));
        context?.HookExtension?.AfterTest.AddHandler((sender, eventArgs) => LoggingHook.AfterTest(sender, eventArgs));
        context?.HookExtension?.BeforeAnyTearDowns.AddHandler((sender, eventArgs) => LoggingHook.BeforeAnyTearDowns(sender, eventArgs));
        context?.HookExtension?.AfterAnyTearDowns.AddHandler((sender, eventArgs) => LoggingHook.AfterAnyTearDowns(sender, eventArgs));
    }
}

internal class AssemblyLoggingHookExtension : ActivateHookLogging, ITestAction
{
    public override void ApplyToContext(TestExecutionContext context)
    {
        BeforeTestRunHook();
        LoggingHook = null!;
        base.ApplyToContext(context);
    }

    public void BeforeTestRunHook() => TestLog.Log("BeforeTestRunHook");

    public void AfterTestRunHook() => TestLog.Log("AfterTestRunHook");

    public void BeforeTest(ITest test)
    {
    }

    public void AfterTest(ITest test)
    {
        AfterTestRunHook();
    }

    public ActionTargets Targets => ActionTargets.Suite;
}

internal class LoggerHook
{
    public void BeforeOneTimeSetUp(string methodName) => TestLog.Log($"- BeforeOneTimeSetUp({methodName})");
    public void AfterOneTimeSetUp(string methodName) => TestLog.Log($"- AfterOneTimeSetUp({methodName})");
    public void BeforeSetUp(string methodName) => TestLog.Log($"- BeforeSetUp({methodName})");
    public void AfterSetUp(string methodName) => TestLog.Log($"- AfterSetUp({methodName})");
    public void BeforeTearDown(string methodName) => TestLog.Log($"- BeforeTearDown({methodName})");
    public void AfterTearDown(string methodName) => TestLog.Log($"- AfterTearDown({methodName})");
    public void BeforeOneTimeTearDown(string methodName) => TestLog.Log($"- BeforeOneTimeTearDown({methodName})");
    public void AfterOneTimeTearDown(string methodName) => TestLog.Log($"- AfterOneTimeTearDown({methodName})");

    public void BeforeAnySetUps(object? sender, TestHookIMethodEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            BeforeOneTimeSetUp(eventArgs.Method.Name);
        }
        else
        {
            BeforeSetUp(eventArgs.Method.Name);
        }
    }

    public void AfterAnySetUps(object? sender, TestHookIMethodEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            AfterOneTimeSetUp(eventArgs.Method.Name);
        }
        else
        {
            AfterSetUp(eventArgs.Method.Name);
        }
    }

    public void BeforeAnyTearDowns(object? sender, TestHookIMethodEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            BeforeOneTimeTearDown(eventArgs.Method.Name);
        }
        else
        {
            BeforeTearDown(eventArgs.Method.Name);
        }
    }

    public void AfterAnyTearDowns(object? sender, TestHookIMethodEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            AfterOneTimeTearDown(eventArgs.Method.Name);
        }
        else
        {
            AfterTearDown(eventArgs.Method.Name);
        }
    }

    public void BeforeTest(object? sender, TestHookTestMethodEventArgs eventArgs)
    {
        TestLog.Log($"- BeforeTestCase({eventArgs.TestMethod.MethodName})");
    }

    public void AfterTest(object? sender, TestHookTestMethodEventArgs eventArgs)
    {
        TestLog.Log($"- AfterTestCase({eventArgs.TestMethod.MethodName})");
    }
}
