// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Predicate constraint wraps a Predicate in a constraint,
    /// returning success if the predicate is true.
    /// </summary>
    public class PredicateConstraint<T> : Constraint
    {
        readonly Predicate<T> predicate;

        /// <summary>
        /// Construct a PredicateConstraint from a predicate
        /// </summary>
        public PredicateConstraint(Predicate<T> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get
            {
#if NETSTANDARD1_3 || NETSTANDARD1_6
                var name = predicate.GetMethodInfo().Name;
#else
                var name = predicate.Method.Name;
#endif
                return name.StartsWith("<")
                    ? "value matching lambda expression"
                    : "value matching " + name;
            }
        }

        /// <summary>
        /// Determines whether the predicate succeeds when applied
        /// to the actual value.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (!(actual is T))
                throw new ArgumentException("The actual value is not of type " + typeof(T).Name, "actual");

            return new ConstraintResult(this, actual, predicate((T)(object)actual));
        }
    }
}
