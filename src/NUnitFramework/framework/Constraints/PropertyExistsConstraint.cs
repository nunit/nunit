// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// PropertyExistsConstraint tests that a named property
    /// exists on the object provided through Match.
    /// 
    /// Originally, PropertyConstraint provided this feature
    /// in addition to making optional tests on the value
    /// of the property. The two constraints are now separate.
    /// </summary>
    public class PropertyExistsConstraint : Constraint
    {
        private readonly string name;
        private Type? actualType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyExistsConstraint"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public PropertyExistsConstraint(string name)
            : base(name)
        {
            this.name = name;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "property " + name;

        /// <summary>
        /// Test whether the property exists for a given object
        /// </summary>
        /// <param name="actual">The object to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            actualType = typeof(TActual);
            PropertyInfo? property = Reflect.GetUltimateShadowingProperty(actualType, name, bindingFlags);

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
                if (actual is Type actualIsType)
                {
                    actualType = actualIsType;
                    property = Reflect.GetUltimateShadowingProperty(actualType, name, bindingFlags);

                    if (property == null)
                    {
                        // Do not set actualType to System.RuntimeType as that is no expected
                        property = Reflect.GetUltimateShadowingProperty(actual.GetType(), name, bindingFlags);
                    }
                }
                else
                {
                    actualType = actual.GetType();
                    property = Reflect.GetUltimateShadowingProperty(actualType, name, bindingFlags);
                }
            }

            return new ConstraintResult(this, actualType, property != null);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        protected override string GetStringRepresentation()
        {
            return $"<propertyexists {name}>";
        }
    }
}
