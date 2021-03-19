// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Represents a constraint that succeeds if any of the 
    /// members of a collection match a base constraint.
    /// </summary>
    public class SomeOperator : CollectionOperator
    {
        /// <summary>
        /// Returns a constraint that will apply the argument
        /// to the members of a collection, succeeding if
        /// any of them succeed.
        /// </summary>
        public override IConstraint ApplyPrefix(IConstraint constraint)
        {
            return new SomeItemsConstraint(constraint);
        }
    }
}