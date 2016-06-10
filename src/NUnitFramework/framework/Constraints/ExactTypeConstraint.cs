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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExactTypeConstraint is used to test that an object
    /// is of the exact type provided in the constructor
    /// </summary>
    public class ExactTypeConstraint : TypeConstraint
    {
        /// <summary>
        /// Construct an ExactTypeConstraint for a given Type
        /// </summary>
        /// <param name="type">The expected Type.</param>
        public ExactTypeConstraint(Type type)
            : base(type, string.Empty)
        {
        }

        /// <summary> 
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName { get { return "TypeOf"; } }

        /// <summary>
        /// Apply the constraint to an actual value, returning true if it succeeds
        /// </summary>
        /// <param name="actual">The actual argument</param>
        /// <returns>True if the constraint succeeds, otherwise false.</returns>
        protected override bool Matches(object actual)
        {
            return actual != null && actual.GetType() == expectedType;
        }
    }
}