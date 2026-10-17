// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !NET471_OR_GREATER && !NETCOREAPP2_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Defines a general-purpose Tuple implementation that allows access to Tuple instance members
    /// without knowing the underlying Tuple type.
    /// </summary>
    /// <remarks>
    /// Only available in .NET Framework 4.7.1 or greater and .NET Core 2.0 or greater.
    /// </remarks>
    internal interface ITuple
    {
        /// <summary>
        /// Gets the number of elements in this tuple instance.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Returns the value of the specified tuple element.
        /// </summary>
        /// <returns></returns>
        public object? this[int index] { get; }
    }
}

#endif
