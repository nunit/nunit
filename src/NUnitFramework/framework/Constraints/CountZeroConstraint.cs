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

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// CountZeroConstraint tests whether an instance has a property .Count with value zero.
    /// </summary>
    public class CountZeroConstraint : Constraint
    {
        private const string CountPropertyName = "Count";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "<empty>"; }
        }

        /// <summary>
        /// Checks if the specified <paramref name="type"/> has a int Count property.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><see langword="true"/> when <paramref name="type"/> has a 'int Count' property, <see langword="false"/> otherwise.</returns>
        public static bool HasCountProperty(Type type) => type.GetProperty(CountPropertyName)?.PropertyType == typeof(int);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            System.Reflection.PropertyInfo countProperty = actual?.GetType().GetProperty(CountPropertyName);
            int? countValue = (int?)countProperty?.GetValue(actual, null);
            return new ConstraintResult(this, actual, countValue == 0);
        }
    }
}
