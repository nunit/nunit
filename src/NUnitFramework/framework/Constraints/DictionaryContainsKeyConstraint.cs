// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// DictionaryContainsKeyConstraint is used to test whether a dictionary
    /// contains an expected object as a key.
    /// </summary>
    public class DictionaryContainsKeyConstraint : CollectionItemsEqualConstraint
    {
        private bool _deprecatedFlag = false;

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
        public override string DisplayName { get { return "ContainsKey"; } }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "dictionary containing key " + MsgUtils.FormatValue(Expected); }
        }

        /// <summary>
        /// Gets the expected object
        /// </summary>
        protected object Expected { get; private set; }

        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        [Obsolete("Deprecated, use Does.ContainKey")]
        public new CollectionItemsEqualConstraint IgnoreCase
        {
            get
            {
                _deprecatedFlag = true;
                return base.IgnoreCase;
            }
        }

        /// <summary>
        /// Test whether the expected key is contained in the dictionary
        /// </summary>
        protected override bool Matches(IEnumerable actual)
        {
            if (_deprecatedFlag)
            {
                var dictionary = ConstraintUtils.RequireActual<IDictionary>(actual, nameof(actual));
                foreach (object obj in dictionary.Keys)
                    if (ItemsEqual(obj, Expected))
                        return true;

                return false;
            }

            var method = GetContainsKeyMethod(actual);
            if (method != null)
                return (bool)method.Invoke(actual, new[] { Expected });

            throw new ArgumentException("Not a collection supporting ContainsKey method.");
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <returns>Self.</returns>
        [Obsolete("Deprecated, use Does.ContainKey")]
        public DictionaryContainsKeyConstraint Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
        {
            // reverse the order of the arguments to match expectations of PredicateEqualityComparer
            Func<TMemberType, TCollectionType, bool> invertedComparison = (actual, expected) => comparison.Invoke(expected, actual);

            _deprecatedFlag = true;
            base.Using(EqualityAdapter.For(invertedComparison));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        [Obsolete("Deprecated, use Does.ContainKey")]
        public new CollectionItemsEqualConstraint Using<T>(Comparison<T> comparer)
        {
            _deprecatedFlag = true;
            return base.Using(comparer);
        }

        /// <summary>
        /// Checks if the key is contained in a "keyed item container".
        /// </summary>
        /// <param name="actual">Keyed container.</param>
        /// <returns>method to call.</returns>
        private MethodInfo GetContainsKeyMethod(object actual)
        {
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            var instanceType = actual.GetType();

            var method = FindContainsKeyMethod(instanceType)
                         ?? instanceType
                            .GetInterfaces()
                            .Concat(GetBaseTypes(instanceType))
                            .Select(FindContainsKeyMethod)
                            .FirstOrDefault(m => m != null);

            return method;
        }

        /// <summary>
        /// Looks for a base type that implements ContainsKey method
        /// </summary>
        /// <param name="type">Type to look for method ContainsKey</param>
        /// <returns>Returns the method to call.</returns>
        private MethodInfo FindContainsKeyMethod(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var method = methods.FirstOrDefault(m =>
                m.ReturnType == typeof(bool)
                && m.Name == "ContainsKey"
                && m.GetParameters().Length == 1);

            if (method == null && type.GetTypeInfo().IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                var tKeyGenericArg = definition.GetGenericArguments().FirstOrDefault(typeArg => typeArg.Name == "TKey");

                if (tKeyGenericArg != null)
                {
                    method = definition
                             .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                             .FirstOrDefault(m =>
                                 m.ReturnType == typeof(bool)
                                 && m.Name == "Contains"
                                 && m.GetParameters().Any()
                                 && m.GetParameters().First()?.ParameterType == tKeyGenericArg);

                    if (method != null)
                        method = methods.Single(m => m.MetadataToken == method.MetadataToken);
                }
            }

            return method;
        }

        /// <summary>
        /// Returns all the base types of the class
        /// </summary>
        /// <param name="type">Type to search for base types implemeted.</param>
        /// <returns>Base types / interfaces implemented by the class</returns>
        private IEnumerable<Type> GetBaseTypes(Type type)
        {
            for (; ; )
            {
                type = type.GetTypeInfo().BaseType;
                if (type == null) break;
                yield return type;
            }
        }
    }
}
