// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Abstractions
{
    internal interface IBits
    {
        /// <summary>
        /// Gets the maximum number of elements contained in the current instance.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the number of bits set.
        /// </summary>
        int BitsSet { get; }

        /// <summary>
        /// Sets a bit a <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the bit to set.</param>
        void Set(int index);

        /// <summary>
        /// Gets the value of a bit a <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the bit to get.</param>
        /// <returns>The value of the bit at the specified index.</returns>
        bool Get(int index);
    }
}
