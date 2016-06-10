// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CollectionEquivalentConstraint is used to determine whether two
    /// collections are equivalent.
    /// </summary>
    public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
    {
        private readonly IEnumerable _expected;

        /// <summary>
        /// Construct a CollectionEquivalentConstraint
        /// </summary>
        /// <param name="expected"></param>
        public CollectionEquivalentConstraint(IEnumerable expected)
            : base(expected)
        {
            _expected = expected;
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "Equivalent"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "equivalent to " + MsgUtils.FormatValue(_expected); }
        }

        /// <summary>
        /// Test whether two collections are equivalent
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            // This is just an optimization
            if (_expected is ICollection && actual is ICollection)
                if (((ICollection)actual).Count != ((ICollection)_expected).Count)
                    return false;

            CollectionTally tally = Tally(_expected);
            return tally.TryRemove(actual) && tally.Count == 0;
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        public CollectionEquivalentConstraint Using<TActual, TExpected>(Func<TActual, TExpected, bool> comparison)
        {
            base.Using(EqualityAdapter.For(comparison));
            return this;
        }
    }
}