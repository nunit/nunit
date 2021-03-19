// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NoItemConstraint applies another constraint to each
    /// item in a collection, failing if any of them succeeds.
    /// </summary>
    public class NoItemConstraint : PrefixConstraint
    {
        /// <summary>
        /// Construct a SomeItemsConstraint on top of an existing constraint
        /// </summary>
        /// <param name="itemConstraint"></param>
        public NoItemConstraint(IConstraint itemConstraint)
            : base(itemConstraint)
        {
            DescriptionPrefix = "no item";
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "None"; } }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// failing if any item fails.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));

            int index = 0;
            foreach (object item in enumerable)
            {
                if (BaseConstraint.ApplyTo(item).IsSuccess)
                {
                    return new EachItemConstraintResult(this, actual, item, index);
                }

                index++;
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Success);
        }
    }
}
