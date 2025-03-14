// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

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

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        public ConstraintBuilder? Builder { get; set; }
    }
}
