// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Identifies a method that is called once to perform setup before any child tests are run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OneTimeSetUpAttribute : NUnitAttribute { }
}
