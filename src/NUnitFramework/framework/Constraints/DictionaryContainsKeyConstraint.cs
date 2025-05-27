// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// DictionaryContainsKeyConstraint is used to test whether a dictionary
    /// contains an expected object as a key.
    /// </summary>
    public class DictionaryContainsKeyConstraint : Constraint
    {
        private const string ContainsMethodName = nameof(IDictionary.Contains);
        private const string ContainsKeyMethodName = nameof(IDictionary<,>.ContainsKey);

        /// <summary>
        /// Construct a DictionaryContainsKeyConstraint
        /// </summary>
        /// <param name="expected"></param>
        public DictionaryContainsKeyConstraint(object expected)
            : base(expected)
        {
            Expected = expected;
        }

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public override string DisplayName => "ContainsKey";

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "dictionary containing key " + MsgUtils.FormatValue(Expected);

        /// <summary>
        /// Gets the expected object
        /// </summary>
        protected object Expected { get; }

        /// <summary>
        /// Returns a new DictionaryContainsKeyValuePairConstraint checking for the
        /// presence of a particular key-value-pair in the dictionary.
        /// </summary>
        public DictionaryContainsKeyValuePairConstraint WithValue(object? expectedValue)
        {
            return Instead.Append(new DictionaryContainsKeyValuePairConstraint(Expected, expectedValue));
        }

        private bool Matches(object? actual)
        {
            if (actual is null)
                throw new ArgumentException("Expected: IDictionary But was: null", nameof(actual));

            var method = GetContainsKeyMethod(actual);
            if (method is not null)
                return (bool)method.Invoke(actual, [Expected])!;

            if (actual is IDictionary dictionary)
            {
                return dictionary.Contains(Expected);
            }

            throw new ArgumentException($"The {TypeHelper.GetDisplayName(actual.GetType())} value must have a {ContainsKeyMethodName} or {ContainsMethodName}(TKey) method.");
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return new ConstraintResult(this, actual, Matches(actual));
        }

        private static MethodInfo? GetContainsKeyMethod(object keyedItemContainer)
        {
            var instanceType = keyedItemContainer.GetType();

            var method = FindContainsKeyMethod(instanceType)
                         ?? instanceType
                            .GetInterfaces()
                            .Concat(GetBaseTypes(instanceType))
                            .Select(FindContainsKeyMethod)
                            .FirstOrDefault(m => m is not null);

            return method;
        }

        private static MethodInfo? FindContainsKeyMethod(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var method = methods.FirstOrDefault(m =>
                m.ReturnType == typeof(bool)
                && m.Name == ContainsKeyMethodName
                && !m.IsGenericMethod
                && m.GetParameters().Length == 1);

            if (method is null && type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                var tKeyGenericArg = definition.GetGenericArguments().FirstOrDefault(typeArg => typeArg.Name == "TKey");

                if (tKeyGenericArg is not null)
                {
                    method = definition
                             .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                             .FirstOrDefault(m => m.ReturnType == typeof(bool) &&
                                                  m.Name == ContainsMethodName &&
                                                  !m.IsGenericMethod &&
                                                  m.GetParameters() is ParameterInfo[] parameters &&
                                                  parameters.Length == 1 &&
                                                  parameters[0].ParameterType == tKeyGenericArg);

                    if (method is not null)
                    {
                        method = methods.Single(m => m.MetadataToken == method.MetadataToken);
                    }
                }
            }

            return method;
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            for (Type? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                yield return baseType;
            }
        }
    }
}
