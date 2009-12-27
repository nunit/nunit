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
using System.Reflection;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// PropertyExistsConstraint tests that a named property
    /// exists on the object provided through Match.
    /// 
    /// Originally, PropertyConstraint provided this feature
    /// in addition to making optional tests on the vaue
    /// of the property. The two constraints are now separate.
    /// </summary>
    public class PropertyExistsConstraint : Constraint
    {
        private string name;

        Type actualType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PropertyExistConstraint"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public PropertyExistsConstraint(string name) : base(name)
        {
            this.name = name;
        }

        /// <summary>
        /// Test whether the property exists for a given object
        /// </summary>
        /// <param name="actual">The object to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override bool Matches(object actual)
        {
            this.actual = actual;
            
            if (actual == null)
                throw new ArgumentNullException("actual");

            this.actualType = actual as Type;
            if (actualType == null)
                actualType = actual.GetType();

            PropertyInfo property = actualType.GetProperty(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);

            return property != null;
        }

        /// <summary>
        /// Write the constraint description to a MessageWriter
        /// </summary>
        /// <param name="writer">The writer on which the description is displayed</param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write("property " + name);
        }

        /// <summary>
        /// Write the actual value for a failing constraint test to a
        /// MessageWriter.
        /// </summary>
        /// <param name="writer">The writer on which the actual value is displayed</param>
        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue(actualType);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<propertyexists {0}>", name);
        }
    }

	/// <summary>
	/// PropertyConstraint extracts a named property and uses
    /// its value as the actual value for a chained constraint.
	/// </summary>
	public class PropertyConstraint : PrefixConstraint
	{
		private string name;
		private object propValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PropertyConstraint"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="baseConstraint">The constraint to apply to the property.</param>
        public PropertyConstraint(string name, Constraint baseConstraint)
			: base( baseConstraint ) 
		{ 
			this.name = name;
		}

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override bool Matches(object actual)
		{
            this.actual = actual;
            if (actual == null) 
                throw new ArgumentNullException("actual");

            Type actualType = actual as Type;
            if ( actualType == null )
                actualType = actual.GetType();

            PropertyInfo property = actualType.GetProperty(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);

            if (property == null)
                throw new ArgumentException(string.Format("Property {0} was not found",name), "name");

			propValue = property.GetValue( actual, null );
			return baseConstraint.Matches( propValue );
		}

		/// <summary>
		/// Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer">The writer on which the description is displayed</param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate( "property " + name );
            if (baseConstraint != null)
            {
                if (baseConstraint is EqualConstraint)
                    writer.WritePredicate("equal to");
                baseConstraint.WriteDescriptionTo(writer);
            }
        }

		/// <summary>
		/// Write the actual value for a failing constraint test to a
		/// MessageWriter. The default implementation simply writes
		/// the raw value of actual, leaving it to the writer to
		/// perform any formatting.
		/// </summary>
		/// <param name="writer">The writer on which the actual value is displayed</param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
            writer.WriteActualValue(propValue);
		}

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<property {0} {1}>", name, baseConstraint);
        }
	}
}
