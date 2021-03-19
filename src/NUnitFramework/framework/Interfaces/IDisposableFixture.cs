// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Any ITest that implements this interface is at a level that the implementing
    /// class should be disposed at the end of the test run
    /// </summary>
    internal interface IDisposableFixture
    {
    }
}
