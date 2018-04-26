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
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two types related by <see cref="IEquatable{T}"/>.
    /// </summary>
    internal sealed class EquatablesComparer : ChainComparer
    {
        public override bool CanCompare(object obj)
        {
            return false;
        }

        public override bool? Equals(object x, object y, ref Tolerance tolerance)
        {
            Type xType = x.GetType();
            Type yType = y.GetType();

            MethodInfo equals = FirstImplementsIEquatableOfSecond(xType, yType);
            if (equals != null)
                return (bool)@equals.Invoke(x, new[] { y });

            if (xType != yType)
            {
                equals = FirstImplementsIEquatableOfSecond(yType, xType);
                if (equals != null)
                    return (bool)@equals.Invoke(y, new[] { x });
            }

            return null;
        }

        public override int GetHashCode(object obj)
        {
            return obj != null ? 1 : 0;
        }

        private static MethodInfo FirstImplementsIEquatableOfSecond(Type first, Type second)
        {
            Type bestMatchingInterface = null;
            Type bestMatchingEquatableType = null;

            foreach (var @interface in first.GetInterfaces())
            {
                Type equatableType;
                if (TryGetEquatableInterfaceArg(@interface, out equatableType) && equatableType.IsAssignableFrom(second))
                {
                    if (bestMatchingEquatableType == null || bestMatchingEquatableType.IsAssignableFrom(equatableType))
                    {
                        bestMatchingInterface = @interface;
                        bestMatchingEquatableType = equatableType;
                    }
                }
            }

            return bestMatchingInterface != null
                ? bestMatchingInterface.GetMethod("Equals")
                : null;
        }

        private static bool TryGetEquatableInterfaceArg(Type @interface, out Type result)
        {
            if (@interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEquatable<>))
            {
                result = @interface.GetGenericArguments()[0];
                return true;
            }

            result = null;
            return false;
        }
    }
}
