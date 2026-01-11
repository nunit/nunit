// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AttributeConstraint tests that a specified attribute is present
    /// on a Type or other provider and that the value of the attribute
    /// satisfies some other constraint.
    /// </summary>
    public class AttributeConstraint : PrefixConstraint
    {
        private readonly Type _expectedType;

        /// <summary>
        /// Constructs an AttributeConstraint for a specified attribute
        /// Type and base constraint.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseConstraint"></param>
        public AttributeConstraint(Type type, IConstraint baseConstraint)
            : base(baseConstraint, "attribute " + type.FullName)
        {
            _expectedType = type;

            if (!typeof(Attribute).IsAssignableFrom(_expectedType))
                throw new ArgumentException($"Type {_expectedType} is not an attribute", nameof(type));
        }

        /// <summary>
        /// Determines whether the Type or other provider has the
        /// expected attribute and if its value matches the
        /// additional constraint specified.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            ArgumentNullException.ThrowIfNull(actual);
            Attribute[] attrs = AttributeHelper.GetCustomAttributes(actual, _expectedType, true);
            if (attrs.Length == 0)
                throw new ArgumentException($"Attribute {_expectedType} was not found", nameof(actual));

            Attribute attrFound = attrs[0];
            return BaseConstraint.ApplyTo(attrFound);
        }

        /// <summary>
        /// Returns a string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return $"<attribute {_expectedType} {BaseConstraint}>";
        }
    }
}
