// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two types related by <see cref="IEquatable{T}"/>.
    /// </summary>
    internal sealed class EquatablesComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal EquatablesComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (_equalityComparer.CompareAsCollection && state.TopLevelComparison)
                return null;

            Type xType = x.GetType();
            Type yType = y.GetType();

            MethodInfo equals = FirstImplementsIEquatableOfSecond(xType, yType);
            if (equals != null)
                return InvokeFirstIEquatableEqualsSecond(x, y, equals);

            equals = FirstImplementsIEquatableOfSecond(yType, xType);
            if (xType != yType && equals != null)
                return InvokeFirstIEquatableEqualsSecond(y, x, equals);

            return null;
        }

        private static MethodInfo FirstImplementsIEquatableOfSecond(Type first, Type second)
        {
            var mostDerived = default(EquatableMethodImpl);

            foreach (var implementation in GetEquatableImplementations(first))
            {
                if (implementation.Argument.IsAssignableFrom(second))
                {
                    if (mostDerived.Argument == null || mostDerived.Argument.IsAssignableFrom(implementation.Argument))
                        mostDerived = implementation;
                }
            }

            return mostDerived.Method;
        }

        private static EquatableMethodImpl[] GetEquatableImplementations(Type type)
        {
            static bool IsIEquatableOfT(Type t, object filter) => t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(IEquatable<>));
            
            var interfaces = type.FindInterfaces((t,f) => IsIEquatableOfT(t,f), string.Empty);
            var implementations = new EquatableMethodImpl[interfaces.Length];

            for(var i = 0; i < interfaces.Length; i++)
            {
                var iMap = type.GetInterfaceMap(interfaces[i]);
                var method = iMap.TargetMethods[0];

                implementations[i] = new EquatableMethodImpl(method, method.GetParameters()[0].ParameterType);
            }

            return implementations;
        }

        private static bool InvokeFirstIEquatableEqualsSecond(object first, object second, MethodInfo equals)
        {
            return equals != null ? (bool)equals.Invoke(first, new object[] { second }) : false;
        }

        private readonly struct EquatableMethodImpl
        {
            public readonly MethodInfo Method { get; }
            public readonly Type Argument { get; }

            public EquatableMethodImpl(MethodInfo method, Type arg)
            {
                Method = method;
                Argument = arg;
            }
        }
    }
}
