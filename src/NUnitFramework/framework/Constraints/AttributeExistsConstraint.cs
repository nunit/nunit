// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AttributeExistsConstraint tests for the presence of a
    /// specified attribute on a Type.
    /// </summary>
    public class AttributeExistsConstraint : Constraint
    {
        private readonly Type expectedType;

        /// <summary>
        /// Constructs an AttributeExistsConstraint for a specific attribute Type
        /// </summary>
        /// <param name="type"></param>
        public AttributeExistsConstraint(Type type)
            : base(type)
        {
            this.expectedType = type;

            if (!typeof(Attribute).IsAssignableFrom(expectedType))
                throw new ArgumentException($"Type {expectedType} is not an attribute", nameof(type));
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "type with attribute " + MsgUtils.FormatValue(expectedType);

        /// <summary>
        /// Tests whether the object provides the expected attribute.
        /// </summary>
        /// <param name="actual">A Type, MethodInfo, or other ICustomAttributeProvider</param>
        /// <returns>True if the expected attribute is present, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));
            Attribute[] attrs = AttributeHelper.GetCustomAttributes(actual, expectedType, true);
            ConstraintResult result = new ConstraintResult(this, actual);
            result.Status = attrs.Length > 0
                ? ConstraintStatus.Success : ConstraintStatus.Failure;
            return result;
        }
    }
}
