// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SomeItemsConstraint applies another constraint to each
    /// item in a collection, succeeding if any of them succeeds.
    /// </summary>
    public class SomeItemsConstraint : PrefixConstraint
    {
        private readonly EqualConstraint _equalConstraint;

        /// <summary>
        /// Construct a SomeItemsConstraint on top of an existing constraint
        /// </summary>
        /// <param name="itemConstraint"></param>
        public SomeItemsConstraint(IConstraint itemConstraint)
            : base(itemConstraint)
        {
            _equalConstraint = itemConstraint as EqualConstraint;
            DescriptionPrefix = "some item";
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "Some"; } }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// succeeding if any item succeeds.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));

            foreach (object item in enumerable)
                if (BaseConstraint.ApplyTo(item).IsSuccess)
                    return new ConstraintResult(this, actual, ConstraintStatus.Success);

            return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="Func{TCollectionType, TMemberType, Boolean}"/> object.
        /// </summary>
        /// <typeparam name="TCollectionType">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TMemberType">The type of the member.</typeparam>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
        {
            CheckPrecondition(nameof(comparison));
            _equalConstraint.Using(comparison);
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="IComparer"/> object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using(IComparer comparer)
        {
            CheckPrecondition(nameof(comparer));
            _equalConstraint.Using(comparer);
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="IComparer{T}"/> object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using<T>(IComparer<T> comparer)
        {
            CheckPrecondition(nameof(comparer));
            _equalConstraint.Using(comparer);
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="Comparison{T}"/> object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using<T>(Comparison<T> comparer)
        {
            CheckPrecondition(nameof(comparer));
            _equalConstraint.Using(comparer);
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="IEqualityComparer"/> object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using(IEqualityComparer comparer)
        {
            CheckPrecondition(nameof(comparer));
            _equalConstraint.Using(comparer);
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied <see cref="IEqualityComparer{T}"/> object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public SomeItemsConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            CheckPrecondition(nameof(comparer));
            _equalConstraint.Using(comparer);
            return this;
        }

        private void CheckPrecondition(string argument)
        {
            if (_equalConstraint == null)
            {
                var message = "Using may only be used with constraints that check the equality of the items";
                throw new ArgumentException(message, argument);
            }
        }
    }
}
