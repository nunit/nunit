// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Applies a constraint to a collection, testing that exactly one
    /// item within the collection meets the provided constraint.
    /// </summary>
    public class ExactlyOneConstraint : Constraint
    {
        private readonly IConstraint _itemConstraint;
        
        /// <summary>
        /// Construct a standalone <see cref="ExactlyOneConstraint"/>.
        /// </summary>
        public ExactlyOneConstraint()
        { }

        /// <summary>
        /// Construct an <see cref="ExactlyOneConstraint"/> on top of
        /// an existing constraint.
        /// </summary>
        /// <param name="itemConstraint">
        /// The constraint that will be attempted to be applied
        /// to a singular item in the collection.
        /// </param>
        public ExactlyOneConstraint(IConstraint itemConstraint)
            : base(itemConstraint)
        {
            Guard.ArgumentNotNull(itemConstraint, "itemConstraint");

            _itemConstraint = itemConstraint.Resolve();
        }

        /// <summary>
        /// Apply the constraint to the singular item in the collection, 
        /// succeeding only if a singular item passes.
        /// </summary>
        /// <typeparam name="TActual">
        /// Type of the collection.
        /// </typeparam>
        /// <param name="actual">
        /// The collection to apply the <see cref="IConstraint"/> to.
        /// </param>
        /// <returns>
        /// The <see cref="ConstraintResult"/> of the constraint applied to the provided
        /// collection.
        /// </returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (!(actual is IEnumerable))
                throw new ArgumentException("The constraint provided is not an IEnumerable", "actual");

            IEnumerable actualEnumerable = (IEnumerable)actual;

            int count = actualEnumerable.Cast<object>().Count();

            if (_itemConstraint == null)
            {
                //Only checking the count of actualEnumerable.
                return new ConstraintResult(this, count, (count == 1));
            }
            else
            {
                if (count > 0)
                {
                    int successfulItems = 0;

                    foreach(object item in actualEnumerable)
                    {
                        if (_itemConstraint.ApplyTo(item).IsSuccess)
                            successfulItems++;
                    }

                    return new ConstraintResult(this, actual, (successfulItems == 1));
                }
                else
                {
                    return new ConstraintResult(this, actual, false);
                }
                
            }
        }

        /// <summary>
        /// The <see cref="Description"/> of what this constraint tests,
        /// for use in messages and in the <see cref="ConstraintResult"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                if (_itemConstraint == null)
                {
                    return "length of 1";
                }
                else
                {
                    return PrefixConstraint.FormatDescription("one item", _itemConstraint);
                }
            }
        }
    }
}
