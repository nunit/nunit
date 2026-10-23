// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// Provides indexed access to values in a tuple instance and exposes the number of elements.
    /// </summary>
    /// <remarks>Used to retrieve tuple element values without coupling to a specific tuple
    /// implementation.</remarks>
    internal interface ITupleValuesSupplier
    {
        /// <summary>
        /// Number of members.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the value of the item <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the item to retrieve.</param>
        /// <returns>The value of the specified item.</returns>
        object? GetValue(int index);
    }
}
