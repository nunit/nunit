// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Implemented by filters for use in deciding which
    /// Types and Methods should be used to generate tests.
    /// </summary>
    public interface IPreFilter
    {
        /// <summary>
        /// Use the filter on a Type, returning true if the type matches the filter
        /// and should therefore be included in the discovery process.
        /// </summary>
        bool IsMatch(Type type);

        /// <summary>
        /// Use the filter on a Type, returning true if the type matches the filter
        /// and should therefore be included in the discovery process.
        /// </summary>
        bool IsMatch(Type type, MethodInfo method);
    }
}
