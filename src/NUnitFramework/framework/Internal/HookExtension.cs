// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Hook Extension interface to run custom code synchronously before or after any test activity.
    /// </summary>
    public class HookExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookExtension"/> class.
        /// </summary>
        public HookExtension()
        {
            BeforeAnySetUps = (context, method) => { };
            AfterAnySetUps = (context, method) => { };
            BeforeTest = (context, testMethod) => { };
            AfterTest = (context, testMethod) => { };
            BeforeAnyTearDowns = (context, method) => { };
            AfterAnyTearDowns = (context, method) => { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
        /// </summary>
        /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
        public HookExtension(HookExtension other) : this()
        {
            other.BeforeAnySetUps?.GetInvocationList()?.ToList().ForEach(d => BeforeAnySetUps += d as SetUpTearDownHookHandler);
            other.AfterAnySetUps?.GetInvocationList()?.ToList().ForEach(d => AfterAnySetUps += d as SetUpTearDownHookHandler);
            other.BeforeTest?.GetInvocationList()?.ToList().ForEach(d => BeforeTest += d as TestHookHandler);
            other.AfterTest?.GetInvocationList()?.ToList().ForEach(d => AfterTest += d as TestHookHandler);
            other.BeforeAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => BeforeAnyTearDowns += d as SetUpTearDownHookHandler);
            other.AfterAnyTearDowns?.GetInvocationList()?.ToList().ForEach(d => AfterAnyTearDowns += d as SetUpTearDownHookHandler);
        }

        /// <summary/>
        public event SetUpTearDownHookHandler BeforeAnySetUps;
        /// <summary/>
        public event SetUpTearDownHookHandler AfterAnySetUps;

        /// <summary/>
        public event TestHookHandler BeforeTest;
        /// <summary/>
        public event TestHookHandler AfterTest;

        /// <summary/>
        public event SetUpTearDownHookHandler BeforeAnyTearDowns;
        /// <summary/>
        public event SetUpTearDownHookHandler AfterAnyTearDowns;

        /// <summary/>
        public void OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
        {
            BeforeAnySetUps?.Invoke(context, method);
        }

        /// <summary/>
        public void OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method)
        {
            AfterAnySetUps?.GetInvocationList()?.Reverse().ToList().ForEach(d => (d as SetUpTearDownHookHandler)?.Invoke(context, method));
        }

        /// <summary/>
        public void OnBeforeTest(TestExecutionContext context, TestMethod testMethod)
        {
            BeforeTest?.Invoke(context, testMethod);
        }

        /// <summary/>
        public void OnAfterTest(TestExecutionContext context, TestMethod testMethod)
        {
            AfterTest?.GetInvocationList()?.Reverse().ToList().ForEach(d => (d as TestHookHandler)?.Invoke(context, testMethod));
        }

        /// <summary/>
        public void OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
        {
            BeforeAnyTearDowns?.Invoke(context, method);
        }

        /// <summary/>
        public void OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method)
        {
            AfterAnyTearDowns?.GetInvocationList()?.Reverse().ToList().ForEach(d => (d as SetUpTearDownHookHandler)?.Invoke(context, method));
        }
    }

    /// <summary/>
    public delegate void SetUpTearDownHookHandler(TestExecutionContext context, IMethodInfo method);

    /// <summary/>
    public delegate void TestHookHandler(TestExecutionContext context, TestMethod testMethod);
}
