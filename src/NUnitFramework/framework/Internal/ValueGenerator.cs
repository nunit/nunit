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

        public virtual int Compare(T x, T y)
        {
            if (!typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                throw CreateNotSupportedException("comparisons");

            return Comparer<T>.Default.Compare(x, y);
        }

        public IEnumerable<T> GenerateRange(T start, T end, Step step)
        {
            var startToEnd = Compare(start, end);

            if (startToEnd == 0)
            {
                yield return start;
            }
            else if ((startToEnd < 0 && !step.IsPositive) || (startToEnd > 0 && !step.IsNegative))
            {
                throw new ArgumentException("Step must be in the direction of the end.");
            }
            else
            {
                for (var current = start;;)
                {
                    yield return current;

                    T next;
                    try
                    {
                        next = step.Apply(current);
                    }
                    catch (OverflowException)
                    {
                        // We overflowed which means we tried to step past the end.
                        break;
                    }

                    if (startToEnd < 0)
                    {
                        if (Compare(next, end) > 0) break; // We stepped past the end of the range.
                        if (Compare(next, current) <= 0)
                            throw new InvalidOperationException("The step must strictly increase.");
                    }
                    else
                    {
                        if (Compare(next, end) < 0) break; // We stepped past the end of the range.
                        if (Compare(next, current) >= 0)
                            throw new InvalidOperationException("The step must strictly decrease.");
                    }

                    current = next;
                }
            }
        }

        public sealed override IEnumerable GenerateRange(object start, object end, ValueGenerator.Step step)
        {
            return GenerateRange((T)start, (T)end, (Step)step);
        }

        /// <summary>
        /// Provides a convenient shorthand when <typeparamref name="TStep"/> is <see cref="IComparable{TStep}"/>
        /// and the default value of <typeparamref name="TStep"/> represents zero.
        /// </summary>
        public sealed class ComparableStep<TStep> : Step where TStep : IComparable<TStep>
        {
            private readonly TStep _step;
            private readonly Func<T, TStep, T> _apply;

            /// <summary>
            /// Initializes a new instance of the <see cref="ComparableStep{TStep}"/> class.
            /// </summary>
            /// <param name="value">The amount by which to increment each time this step is applied.</param>
            /// <param name="apply">
            /// Must increment the given value and return the result.
            /// If the result is outside the range representable by <typeparamref name="T"/>,
            /// must throw <see cref="OverflowException"/>. If the result does not change due to lack
            /// of precision representable by <typeparamref name="T"/>, must throw <see cref="ArithmeticException"/>.
            /// </param>
            public ComparableStep(TStep value, Func<T, TStep, T> apply)
            {
                if (apply == null) throw new ArgumentNullException(nameof(apply));
                _step = value;
                _apply = apply;
            }

            public override bool IsPositive => Comparer<TStep>.Default.Compare(default(TStep), _step) < 0;
            public override bool IsNegative => Comparer<TStep>.Default.Compare(_step, default(TStep)) < 0;

            /// <summary>
            /// Increments the given value and returns the result.
            /// If the result is outside the range representable by <typeparamref name="T"/>,
            /// throws <see cref="OverflowException"/>. If the result does not change due to lack
            /// of precision representable by <typeparamref name="T"/>, throws <see cref="ArithmeticException"/>.
            /// </summary>
            /// <exception cref="OverflowException"/>
            /// <exception cref="ArithmeticException"/>
            public override T Apply(T value) => _apply.Invoke(value, _step);
        }

        /// <summary>
        /// Encapsulates the ability to increment a <typeparamref name="T"/> value by an amount
        /// which may be of a different type.
        /// </summary>
        public new abstract class Step : ValueGenerator.Step
        {
            /// <summary>
            /// Increments the given value and returns the result.
            /// If the result is outside the range representable by <typeparamref name="T"/>,
            /// throws <see cref="OverflowException"/>. If the result does not change due to lack
            /// of precision representable by <typeparamref name="T"/>, throws <see cref="ArithmeticException"/>.
            /// </summary>
            /// <exception cref="OverflowException"/>
            /// <exception cref="ArithmeticException"/>
            public abstract T Apply(T value);
        }

        /// <summary>
        /// Creates a <see cref="Step"/> from the specified value if the current instance is able to
        /// use it to increment values of type <typeparamref name="T"/>. If the creation fails,
        /// <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public sealed override ValueGenerator.Step CreateStep(object value)
        {
            ValueGenerator.Step step;
            if (TryCreateStep(value, out step)) return step;
            throw CreateNotSupportedException($"creating a step of type {value.GetType()}");
        }

        /// <summary>
        /// Creates a <see cref="Step"/> from the specified value if the current instance is able to
        /// use it to increment values of type <typeparamref name="T"/>. A return value indicates
        /// whether the creation succeeded.
        /// </summary>
        public override bool TryCreateStep(object value, out ValueGenerator.Step step)
        {
            Guard.ArgumentNotNull(value, nameof(value));
            step = null;
            return false;
        }
    }

    internal abstract partial class ValueGenerator
    {
        public abstract IEnumerable GenerateRange(object start, object end, Step step);

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
            // The JIT removes all branches which do not match T since it generates a separate version
            // of this method at runtime for every value type T.

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

        /// <summary>
        /// Encapsulates the ability to increment a value by an amount which may be of a different type.
        /// </summary>
        public abstract class Step
        {
            public abstract bool IsPositive { get; }
            public abstract bool IsNegative { get; }
        }

        /// <summary>
        /// Creates a <see cref="Step"/> from the specified value if the current instance is able to
        /// use it to increment the on values which it operates. If the creation fails,
        /// <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public abstract Step CreateStep(object value);

        /// <summary>
        /// Creates a <see cref="Step"/> from the specified value if the current instance is able to
        /// use it to increment values on which it operates. A return value indicates
        /// whether the creation succeeded.
        /// </summary>
        public abstract bool TryCreateStep(object value, out Step step);
    }
}
