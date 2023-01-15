// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections.Generic;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a field, property or method as providing a set of datapoints for use
    /// in executing any theories within the same fixture that require an argument of
    /// the provided Type. The data source may provide an array of the required Type
    /// or an <see cref="IEnumerable{T}"/>. Synonymous with <see cref="DatapointSourceAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DatapointsAttribute : DatapointSourceAttribute
    {
    }
}
