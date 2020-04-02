// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// DictionaryContainsKeyValuePairConstraint is used to test whether a dictionary
    /// contains an expected object as a key-value-pair.
    /// </summary>
    public class DictionaryContainsKeyValuePairConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// Construct a DictionaryContainsKeyValuePairConstraint
        /// </summary>
        public DictionaryContainsKeyValuePairConstraint(object key, object value)
            : this(new KeyValuePair<object, object>(key, value))
        {
        }

        /// <summary>
        /// Construct a DictionaryContainsKeyValuePairConstraint
        /// </summary>
        protected DictionaryContainsKeyValuePairConstraint(KeyValuePair<object, object> arg)
            : base(arg)
        {
            Expected = arg;
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "ContainsKeyValuePair"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "dictionary containing entry " + MsgUtils.FormatValue(Expected); }
        }

        /// <summary>
        /// Gets the expected key
        /// </summary>
        protected object ExpectedKey { get { return Expected.Key; } }

        /// <summary>
        /// Gets the expected value
        /// </summary>
        protected object ExpectedValue { get { return Expected.Value; } }

        /// <summary>
        /// Gets the expected entry
        /// </summary>
        protected KeyValuePair<object, object> Expected { get; }

        private bool Matches(object actual)
        {
            var dictionary = ConstraintUtils.RequireActual<IDictionary>(actual, nameof(actual));
			foreach (DictionaryEntry entry in dictionary)
				if (ItemsEqual(entry.Key, ExpectedKey) && ItemsEqual(entry.Value, ExpectedValue))
					return true;

			return false;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, Matches(actual));
        }

        /// <summary>
        /// Test whether the expected key is contained in the dictionary
        /// </summary>
        protected override bool Matches(IEnumerable collection)
        {
            return Matches(collection);
        }
    }
}
