// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
    /// DictionaryContainsValueConstraint is used to test whether a dictionary
    /// contains an expected object as a value.
    /// </summary>
    public class DictionaryContainsValueConstraint : CollectionContainsConstraint
    {
        /// <summary>
        /// Construct a DictionaryContainsValueConstraint
        /// </summary>
        /// <param name="expected"></param>
        public DictionaryContainsValueConstraint(object expected)
            : base(expected)
        {
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "ContainsValue"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "dictionary containing value " + MsgUtils.FormatValue(Expected); }
        }

        /// <summary>
        /// Test whether the expected value is contained in the dictionary
        /// </summary>
        protected override bool Matches(IEnumerable actual)
        {
            IDictionary dictionary = actual as IDictionary;

            if (dictionary == null)
                throw new ArgumentException("The actual value must be an IDictionary", "actual");

            return base.Matches(dictionary.Values);
        }
    }
}