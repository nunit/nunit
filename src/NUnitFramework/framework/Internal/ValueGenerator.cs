// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Internal
{
    internal abstract class ValueGenerator<T> : ValueGenerator
    {
        private static Exception CreateNotSupportedException(string description)
        {
            return new NotSupportedException($"{typeof(T)} is using the default value generator which does not support {description}.");
        }

        public virtual IEnumerable<T> GenerateRange(T start, T end, T step)
        {
            throw CreateNotSupportedException("generating a range");
        }

        public override IEnumerable GenerateRange(object start, object end, object step)
        {
            return GenerateRange((T)start, (T)end, (T)step);
        }
    }

    internal abstract partial class ValueGenerator
    {
        public abstract IEnumerable GenerateRange(object start, object end, object step);

        private static readonly MethodInfo GenericCreateMethod =
            typeof(ValueGenerator)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == nameof(Create) && method.IsGenericMethod);

        public static ValueGenerator Create(Type valueType)
        {
            var genericInstantiation = GenericCreateMethod.MakeGenericMethod(valueType);
            var @delegate = (Func<ValueGenerator>)genericInstantiation.CreateDelegate(typeof(Func<ValueGenerator>));
            return @delegate.Invoke();
        }

        public static ValueGenerator<T> Create<T>()
        {
            if (typeof(T) == typeof(sbyte)) return (ValueGenerator<T>)(object)new SByteValueGenerator();

            if (typeof(T) == typeof(byte)) return (ValueGenerator<T>)(object)new ByteValueGenerator();

            if (typeof(T) == typeof(short)) return (ValueGenerator<T>)(object)new Int16ValueGenerator();

            if (typeof(T) == typeof(ushort)) return (ValueGenerator<T>)(object)new UInt16ValueGenerator();

            if (typeof(T) == typeof(int)) return (ValueGenerator<T>)(object)new Int32ValueGenerator();

            if (typeof(T) == typeof(uint)) return (ValueGenerator<T>)(object)new UInt32ValueGenerator();

            if (typeof(T) == typeof(long)) return (ValueGenerator<T>)(object)new Int64ValueGenerator();

            if (typeof(T) == typeof(ulong)) return (ValueGenerator<T>)(object)new UInt64ValueGenerator();

            if (typeof(T) == typeof(float)) return (ValueGenerator<T>)(object)new SingleValueGenerator();

            if (typeof(T) == typeof(double)) return (ValueGenerator<T>)(object)new DoubleValueGenerator();

            if (typeof(T) == typeof(decimal)) return (ValueGenerator<T>)(object)new DecimalValueGenerator();

            return new DefaultValueGenerator<T>();
        }
    }
}
