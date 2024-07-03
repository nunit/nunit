// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces;

/// <summary>
/// Hook Extension interface to run custom code synchronously before or after any test activity.
/// </summary>
public interface IHookExtension
{
    /// <summary/>
    void BeforeAnySetUps(TestExecutionContext context, IMethodInfo setUpMethod);
    /// <summary/>
    void AfterAnySetUps(TestExecutionContext context, IMethodInfo setUpMethod);

    /// <summary/>
    void BeforeTest(TestExecutionContext context, TestMethod testMethod);
    /// <summary/>
    void AfterTest(TestExecutionContext context, TestMethod testMethod);

    /// <summary/>
    void BeforeAnyTearDowns(TestExecutionContext context, IMethodInfo tearDownMethod);
    /// <summary/>
    void AfterAnyTearDowns(TestExecutionContext context, IMethodInfo tearDownMethod);
}
