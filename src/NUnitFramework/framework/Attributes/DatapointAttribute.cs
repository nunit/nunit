// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a field for use as a datapoint when executing a theory within
    /// the same fixture that requires an argument of the field's Type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DatapointAttribute : NUnitAttribute
    {
    }
}
