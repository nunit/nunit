// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Identifies a method that is called once to perform setup before any child tests are run.
    /// </summary>
    /// <remarks>
    /// The method may have a single <see cref="System.Threading.CancellationToken"/> parameter,
    /// which receives the same token as <see cref="TestContext.CancellationToken"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OneTimeSetUpAttribute : NUnitAttribute
    {
    }
}
