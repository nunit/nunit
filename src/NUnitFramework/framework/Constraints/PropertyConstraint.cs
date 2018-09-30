// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// PropertyConstraint extracts a named property and uses
    /// its value as the actual value for a chained constraint.
    /// </summary>
    public class PropertyConstraint : PrefixConstraint
    {
        private readonly string name;
        private object propValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyConstraint"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="baseConstraint">The constraint to apply to the property.</param>
        public PropertyConstraint(string name, IConstraint baseConstraint)
            : base(baseConstraint)
        {
            this.name = name;
            this.DescriptionPrefix = "property " + name;
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // TODO: Use an error result for null
            Guard.ArgumentNotNull(actual, nameof(actual));

            Type actualType = actual as Type;
            if (actualType == null)
                actualType = actual.GetType();

            PropertyInfo property = Reflect.GetUltimateShadowingProperty(actualType, name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // TODO: Use an error result here
            if (property == null)
                throw new ArgumentException($"Property {name} was not found on {actualType}.", "name");

            propValue = property.GetValue(actual, null);
            var baseResult = BaseConstraint.ApplyTo(propValue);
            return new PropertyConstraintResult(this, baseResult);              
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return string.Format("<property {0} {1}>", name, BaseConstraint);
        }
    }
}
