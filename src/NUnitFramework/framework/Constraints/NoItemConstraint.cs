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
            if (!(actual is IEnumerable))
                throw new ArgumentException("The actual value must be an IEnumerable", "actual");

            foreach (object item in (IEnumerable)actual)
                if (BaseConstraint.ApplyTo(item).IsSuccess)
                    return new ConstraintResult(this, actual, ConstraintStatus.Failure);

            return new ConstraintResult(this, actual, ConstraintStatus.Success);
        }
    }
}