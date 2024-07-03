// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces;

/// <summary>
/// H-TODO summary missing
/// </summary>
public interface IHookExtension
{
    // H-TODO clarify parameters. Is TestExecutionContext needed?
    /// <summary/>
    void BeforeOneTimeSetUp(string methodName);

    /// <summary/>
    void AfterOneTimeSetUp(string methodName);

    /// <summary/>
    void BeforeSetUp(string methodName);

    /// <summary/>
    void AfterSetUp(string methodName);

    /// <summary/>
    void BeforeTest(string methodName);

    /// <summary/>
    void AfterTest(string methodName);

    /// <summary/>
    void BeforeTearDown(string methodName);

    /// <summary/>
    void AfterTearDown(string methodName);

    /// <summary/>
    void BeforeOneTimeTearDown(string methodName);

    /// <summary/>
    void AfterOneTimeTearDown(string methodName);
}
