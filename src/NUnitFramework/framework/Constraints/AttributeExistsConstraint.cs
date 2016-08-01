// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AttributeExistsConstraint tests for the presence of a
    /// specified attribute on  a Type.
    /// </summary>
    public class AttributeExistsConstraint : Constraint
    {
        private Type expectedType;

        /// <summary>
        /// Constructs an AttributeExistsConstraint for a specific attribute Type
        /// </summary>
        /// <param name="type"></param>
        public AttributeExistsConstraint(Type type)
            : base(type)
        {
            this.expectedType = type;

            if (!typeof(Attribute).GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo()))
                throw new ArgumentException(string.Format(
                    "Type {0} is not an attribute", expectedType), "type");
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "type with attribute " + MsgUtils.FormatValue(expectedType); }
        }

        /// <summary>
        /// Tests whether the object provides the expected attribute.
        /// </summary>
        /// <param name="actual">A Type, MethodInfo, or other ICustomAttributeProvider</param>
        /// <returns>True if the expected attribute is present, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, "actual");
            Attribute[] attrs = AttributeHelper.GetCustomAttributes(actual, expectedType, true);
            ConstraintResult result = new ConstraintResult(this, actual);
            result.Status = attrs.Length > 0
                ? ConstraintStatus.Success : ConstraintStatus.Failure;
            return result;
        }
    }
}