// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    using System;

    /// <summary>
    /// Identifies a method to be called immediately before each test is run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited=true)]
    public class SetUpAttribute : NUnitAttribute
    { }
}
