// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly Type expectedType;
        private Attribute attrFound;

        /// <summary>
        /// Constructs an AttributeConstraint for a specified attribute
        /// Type and base constraint.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseConstraint"></param>
        public AttributeConstraint(Type type, IConstraint baseConstraint)
            : base(baseConstraint)
        {
            this.expectedType = type;
            this.DescriptionPrefix = "attribute " + expectedType.FullName;

            if (!typeof(Attribute).GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo()))
                throw new ArgumentException($"Type {expectedType} is not an attribute", nameof(type));
        }

        /// <summary>
        /// Determines whether the Type or other provider has the 
        /// expected attribute and if its value matches the
        /// additional constraint specified.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));
            Attribute[] attrs = AttributeHelper.GetCustomAttributes(actual, expectedType, true);
            if (attrs.Length == 0)
                throw new ArgumentException($"Attribute {expectedType} was not found", nameof(actual));

            attrFound = attrs[0];
            return BaseConstraint.ApplyTo(attrFound);
        }

        /// <summary>
        /// Returns a string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return $"<attribute {expectedType} {BaseConstraint}>";
        }
    }
}
