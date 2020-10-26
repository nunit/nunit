// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

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
