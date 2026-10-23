// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// Provides indexed access to values for instances of a tuple type and exposes the number of elements.
    /// </summary>
    /// <remarks>Used to retrieve tuple element values without coupling to a specific tuple
    /// implementation.</remarks>
    internal interface ITupleTypeValuesSupplier
    {
        /// <summary>
        /// Number of members.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the value of the item <paramref name="index"/> from <paramref name="tuple"/>.
        /// </summary>
        /// <param name="tuple">The tuple instance from which to retrieve the value.</param>
        /// <param name="index">The zero-based index of the item to retrieve.</param>
        /// <returns>The value of the specified item.</returns>
        object? GetValue(object tuple, int index);
    }
}
