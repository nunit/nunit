// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions;

/// <summary>
/// Hook Extension interface to run custom code synchronously before or after any test activity.
/// </summary>
public class HookExtension
{
    /// <summary>
    /// Default ctor of <see cref="HookExtension"/> class.
    /// </summary>
    public HookExtension()
    {
        BeforeAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeBeforeAnySetUps);
        AfterAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeAfterAnySetUps);
        BeforeTest = new AsyncEvent<TestHookTestMethodEventArgs>(out _invokeBeforeTest);
        AfterTest = new AsyncEvent<TestHookTestMethodEventArgs>(out _invokeAfterTest);
        BeforeAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeBeforeAnyTearDowns);
        AfterAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>(out _invokeAfterAnyTearDowns);
    }

    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeBeforeAnySetUps;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeAfterAnySetUps;
    private AsyncEventHandler<TestHookTestMethodEventArgs> _invokeBeforeTest;
    private AsyncEventHandler<TestHookTestMethodEventArgs> _invokeAfterTest;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeBeforeAnyTearDowns;
    private AsyncEventHandler<TestHookIMethodEventArgs> _invokeAfterAnyTearDowns;

    internal AsyncEvent<TestHookIMethodEventArgs> BeforeAnySetUps { get; set; }
    internal AsyncEvent<TestHookIMethodEventArgs> AfterAnySetUps { get; set; }
    internal AsyncEvent<TestHookTestMethodEventArgs> BeforeTest { get; set; }
    internal AsyncEvent<TestHookTestMethodEventArgs> AfterTest { get; set; }
    internal AsyncEvent<TestHookIMethodEventArgs> BeforeAnyTearDowns { get; set; }
    internal AsyncEvent<TestHookIMethodEventArgs> AfterAnyTearDowns { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
    /// </summary>
    /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
    public HookExtension(HookExtension other) : this()
    {
        other._invokeBeforeAnySetUps?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeAnySetUps += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeAfterAnySetUps?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterAnySetUps += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeBeforeTest?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeTest += d as AsyncEventHandler<TestHookTestMethodEventArgs>);
        other._invokeAfterTest?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterTest += d as AsyncEventHandler<TestHookTestMethodEventArgs>);
        other._invokeBeforeAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => _invokeBeforeAnyTearDowns += d as AsyncEventHandler<TestHookIMethodEventArgs>);
        other._invokeAfterAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => _invokeAfterAnyTearDowns += d as AsyncEventHandler<TestHookIMethodEventArgs>);
    }

    internal async Task OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        await _invokeBeforeAnySetUps(this, new TestHookIMethodEventArgs(context, method));
    }

    internal async Task OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        await _invokeAfterAnySetUps(this, new TestHookIMethodEventArgs(context, method));
    }

    internal async Task OnBeforeTest(TestExecutionContext context, TestMethod testMethod)
    {
        await _invokeBeforeTest(this, new TestHookTestMethodEventArgs(context, testMethod));
    }

    internal async Task OnAfterTest(TestExecutionContext context, TestMethod testMethod)
    {
        await _invokeAfterTest(this, new TestHookTestMethodEventArgs(context, testMethod));
    }

    internal async Task OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        await _invokeBeforeAnyTearDowns(this, new TestHookIMethodEventArgs(context, method));
    }

    internal async Task OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        await _invokeAfterAnyTearDowns(this, new TestHookIMethodEventArgs(context, method));
    }
}

/// <summary/>
public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
