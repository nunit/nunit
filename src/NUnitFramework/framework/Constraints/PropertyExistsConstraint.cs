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
        private readonly string _name;
        private Type? _actualType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyExistsConstraint"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public PropertyExistsConstraint(string name)
            : base(name)
        {
            _name = name;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "property " + _name;

        /// <summary>
        /// Test whether the property exists for a given object
        /// </summary>
        /// <param name="actual">The object to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            _actualType = typeof(TActual);
            PropertyInfo? property = Reflect.GetUltimateShadowingProperty(_actualType, _name, bindingFlags);

            if (property is null && typeof(TActual).IsInterface)
            {
                foreach (var @interface in typeof(TActual).GetInterfaces())
                {
                    property = Reflect.GetUltimateShadowingProperty(@interface, _name, bindingFlags);
                    if (property is not null)
                        break;
                }
            }

            if (property is null)
            {
                if (actual is Type actualIsType)
                {
                    _actualType = actualIsType;
                    property = Reflect.GetUltimateShadowingProperty(_actualType, _name, bindingFlags);

                    if (property is null)
                    {
                        // Do not set actualType to System.RuntimeType as that is no expected
                        property = Reflect.GetUltimateShadowingProperty(actual.GetType(), _name, bindingFlags);
                    }
                }
                else
                {
                    _actualType = actual.GetType();
                    property = Reflect.GetUltimateShadowingProperty(_actualType, _name, bindingFlags);
                }
            }

            return new ConstraintResult(this, _actualType, property is not null);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        protected override string GetStringRepresentation()
        {
            return $"<propertyexists {_name}>";
        }
    }
}
