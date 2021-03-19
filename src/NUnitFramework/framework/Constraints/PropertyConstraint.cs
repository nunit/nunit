// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            PropertyInfo property = Reflect.GetUltimateShadowingProperty(typeof(TActual), name, bindingFlags);

            if (property == null && typeof(TActual).IsInterface)
            {
                foreach (var @interface in typeof(TActual).GetInterfaces())
                {
                    property = Reflect.GetUltimateShadowingProperty(@interface, name, bindingFlags);
                    if (property != null) break;
                }
            }

            if (property == null)
            {
                Type actualType = actual as Type ?? actual.GetType();

                property = Reflect.GetUltimateShadowingProperty(actualType, name, bindingFlags);

                // TODO: Use an error result here
                if (property == null)
                    throw new ArgumentException($"Property {name} was not found on {actualType}.", nameof(name));
            }

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
