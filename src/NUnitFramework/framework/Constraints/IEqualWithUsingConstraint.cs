// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// Interface for equal constraints which support user comparisons.
    /// </summary>
    public interface IEqualWithUsingConstraint<T>
    {
        /// <summary>
        /// The expected value.
        /// </summary>
        public T Expected { get; }
    }
}
