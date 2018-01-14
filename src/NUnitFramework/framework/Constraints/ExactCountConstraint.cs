// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExactCountConstraint applies another constraint to each
    /// item in a collection, succeeding only if a specified
    /// number of items succeed.
    /// </summary>
    public class ExactCountConstraint : Constraint
    {
        private readonly int _expectedCount;
        private readonly IConstraint _itemConstraint;

        /// <summary>
        /// Construct a standalone ExactCountConstraint
        /// </summary>
        /// <param name="expectedCount"></param>
        public ExactCountConstraint(int expectedCount)
        {
            _expectedCount = expectedCount;
        }

        /// <summary>
        /// Construct an ExactCountConstraint on top of an existing constraint
        /// </summary>
        /// <param name="expectedCount"></param>
        /// <param name="itemConstraint"></param>
        public ExactCountConstraint(int expectedCount, IConstraint itemConstraint)
            : base(itemConstraint)
        {
            Guard.ArgumentNotNull(itemConstraint, nameof(itemConstraint));

            _itemConstraint = itemConstraint.Resolve();
            _expectedCount = expectedCount;
        }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// succeeding only if the expected number of items pass.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var enumerable = ConstraintUtils.RequireActual<IEnumerable>(actual, nameof(actual));

            int count = 0;
            if (_itemConstraint == null)
            {
                foreach (object item in enumerable)
                    count++;
            }
            else
            {
                foreach (object item in enumerable)
                    if (_itemConstraint.ApplyTo(item).IsSuccess)
                        count++;
            }

            return new ConstraintResult(this, actual, count == _expectedCount);
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var descriptionPrefix =
                    _expectedCount == 0 ? "no item" :
                    _expectedCount == 1 ? "exactly one item" :
                    string.Format("exactly {0} items", _expectedCount);

                return _itemConstraint != null ? PrefixConstraint.FormatDescription(descriptionPrefix, _itemConstraint) : descriptionPrefix;
            }
        }
    }
}

