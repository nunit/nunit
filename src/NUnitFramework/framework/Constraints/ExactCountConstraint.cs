// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
    /// ExactCountConstraint applies another constraint to each
    /// item in a collection, succeeding only if a specified
    /// number of items succeed.
    /// </summary>
    public class ExactCountConstraint : PrefixConstraint
    {
        private int expectedCount;

        /// <summary>
        /// Construct an ExactCountConstraint on top of an existing constraint
        /// </summary>
        /// <param name="expectedCount"></param>
        /// <param name="itemConstraint"></param>
        public ExactCountConstraint(int expectedCount, IConstraint itemConstraint)
            : base(itemConstraint)
        {
            this.expectedCount = expectedCount;
            this.DescriptionPrefix = expectedCount == 0
                ? "no item"
                : expectedCount == 1
                    ? "exactly one item"
                    : string.Format("exactly {0} items", expectedCount);
        }

        /// <summary>
        /// Apply the item constraint to each item in the collection,
        /// succeeding only if the expected number of items pass.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (!(actual is IEnumerable))
                throw new ArgumentException("The actual value must be an IEnumerable", "actual");

            int count = 0;
            foreach (object item in (IEnumerable)actual)
                if (BaseConstraint.ApplyTo(item).IsSuccess)
                    count++;

            return new ConstraintResult(this, actual, count == expectedCount);
        }
    }
}

