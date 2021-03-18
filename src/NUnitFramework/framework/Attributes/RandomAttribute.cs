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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Supplies a set of random values to a single parameter of a parameterized test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RandomAttribute : NUnitAttribute, IParameterDataSource
    {
        private RandomDataSource? _source;
        private readonly int _count;

        /// <summary>
        /// If true, no value will be repeated.
        /// </summary>
        public bool Distinct { get; set; }

        #region Constructors

        /// <summary>
        /// Construct a random set of values appropriate for the Type of the
        /// parameter on which the attribute appears, specifying only the count.
        /// </summary>
        /// <param name="count"></param>
        public RandomAttribute(int count)
        {
            _count = count;
        }

        /// <summary>
        /// Generates <see cref="int"/> values within a specified range.
        /// </summary>
        public RandomAttribute(int min, int max, int count)
        {
            _source = new IntDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="uint"/> values within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(uint min, uint max, int count)
        {
            _source = new UIntDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="long"/> values within a specified range.
        /// </summary>
        public RandomAttribute(long min, long max, int count)
        {
            _source = new LongDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="ulong"/> values within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(ulong min, ulong max, int count)
        {
            _source = new ULongDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="short"/> values within a specified range.
        /// </summary>
        public RandomAttribute(short min, short max, int count)
        {
            _source = new ShortDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="ushort"/> values within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(ushort min, ushort max, int count)
        {
            _source = new UShortDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="double"/> values within a specified range.
        /// </summary>
        public RandomAttribute(double min, double max, int count)
        {
            _source = new DoubleDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="float"/> values within a specified range.
        /// </summary>
        public RandomAttribute(float min, float max, int count)
        {
            _source = new FloatDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="byte"/> values within a specified range.
        /// </summary>
        public RandomAttribute(byte min, byte max, int count)
        {
            _source = new ByteDataSource(min, max, count);
        }

        /// <summary>
        /// Generates <see cref="sbyte"/> values within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(sbyte min, sbyte max, int count)
        {
            _source = new SByteDataSource(min, max, count);
        }

        #endregion

        #region IParameterDataSource Interface

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(IParameterInfo parameter)
        {
            // Since a separate Randomizer is used for each parameter,
            // we can't fill in the data in the constructor of the
            // attribute. Only now, when GetData is called, do we have
            // sufficient information to create the values in a
            // repeatable manner.

            Type parmType = parameter.ParameterType;

            if (_source == null)
            {
                if (parmType == typeof(int))
                    _source = new IntDataSource(_count);
                else if (parmType == typeof(uint))
                    _source = new UIntDataSource(_count);
                else if (parmType == typeof(long))
                    _source = new LongDataSource(_count);
                else if (parmType == typeof(ulong))
                    _source = new ULongDataSource(_count);
                else if (parmType == typeof(short))
                    _source = new ShortDataSource(_count);
                else if (parmType == typeof(ushort))
                    _source = new UShortDataSource(_count);
                else if (parmType == typeof(double))
                    _source = new DoubleDataSource(_count);
                else if (parmType == typeof(float))
                    _source = new FloatDataSource(_count);
                else if (parmType == typeof(byte))
                    _source = new ByteDataSource(_count);
                else if (parmType == typeof(sbyte))
                    _source = new SByteDataSource(_count);
                else if (parmType == typeof(decimal))
                    _source = new DecimalDataSource(_count);
                else if (parmType.GetTypeInfo().IsEnum)
                    _source = new EnumDataSource(_count);
                else // Default
                    _source = new IntDataSource(_count);
            }
            else if (_source.DataType != parmType && WeConvert(_source.DataType, parmType))
            {
                _source.Distinct = Distinct;
                _source = new RandomDataConverter(_source);
            }

            _source.Distinct = Distinct;

            return _source.GetData(parameter);
        }

        private bool WeConvert(Type sourceType, Type targetType)
        {
            if (targetType == typeof(short) || targetType == typeof(ushort) || targetType == typeof(byte) || targetType == typeof(sbyte))
                return sourceType == typeof(int);

            if (targetType == typeof(decimal))
                return sourceType == typeof(int) || sourceType == typeof(double);

            return false;
        }

        #endregion

        #region Nested DataSource Classes

        #region RandomDataSource

        abstract class RandomDataSource : IParameterDataSource
        {
            protected RandomDataSource(Type dataType) => DataType = dataType;

            public Type DataType { get; protected set; }
            public bool Distinct { get; set; }

            public abstract IEnumerable GetData(IParameterInfo parameter);
        }

        abstract class RandomDataSource<T> : RandomDataSource
        {
            [AllowNull, MaybeNull]
            private readonly T _min;

            [AllowNull, MaybeNull]
            private readonly T _max;

            private readonly int _count;
            private readonly bool _inRange;

            private readonly List<T> previousValues = new List<T>();

            protected RandomDataSource(int count) : base(typeof(T))
            {
                _min = default;
                _max = default;
                _count = count;
                _inRange = false;
            }

            protected RandomDataSource(T min, T max, int count) : base(typeof(T))
            {
                _min = min;
                _max = max;
                _count = count;
                _inRange = true;
            }

            public override IEnumerable GetData(IParameterInfo parameter)
            {
                //Guard.ArgumentValid(parameter.ParameterType == typeof(T), "Parameter type must be " + typeof(T).Name, "parameter");

                var randomizer = Randomizer.GetRandomizer(parameter.ParameterInfo);

                Guard.OperationValid(!(Distinct && _inRange && !CanBeDistinct(_min!, _max!, _count)), $"The range of values is [{_min}, {_max}[ and the random value count is {_count} so the values cannot be distinct.");


                for (int i = 0; i < _count; i++)
                {
                    if (Distinct)
                    {
                        T next;

                        do
                        {
                            next = _inRange
                                ? GetNext(randomizer, _min!, _max!)
                                : GetNext(randomizer);
                        } while (previousValues.Contains(next));

                        previousValues.Add(next);

                        yield return next;
                    }
                    else
                        yield return _inRange
                            ? GetNext(randomizer, _min!, _max!)
                            : GetNext(randomizer);
                }
            }

            protected abstract T GetNext(Randomizer randomizer);
            protected abstract T GetNext(Randomizer randomizer, T min, T max);
            protected abstract bool CanBeDistinct(T min, T max, int count);
        }

        #endregion

        #region RandomDataConverter

        class RandomDataConverter : RandomDataSource
        {
            readonly IParameterDataSource _source;

            public RandomDataConverter(RandomDataSource source) : base(source.DataType)
            {
                _source = source;
            }

            public override IEnumerable GetData(IParameterInfo parameter)
            {
                Type parmType = parameter.ParameterType;

                foreach (object obj in _source.GetData(parameter))
                {
                    if (obj is int)
                    {
                        int ival = (int)obj; // unbox first
                        if (parmType == typeof(short))
                            yield return (short)ival;
                        else if (parmType == typeof(ushort))
                            yield return (ushort)ival;
                        else if (parmType == typeof(byte))
                            yield return (byte)ival;
                        else if (parmType == typeof(sbyte))
                            yield return (sbyte)ival;
                        else if (parmType == typeof(decimal))
                            yield return (decimal)ival;
                    }
                    else if (obj is double)
                    {
                        double d = (double)obj; // unbox first
                        if (parmType == typeof(decimal))
                            yield return (decimal)d;
                    }
                }
            }
        }

        #endregion

        #region IntDataSource

        class IntDataSource : RandomDataSource<int>
        {
            public IntDataSource(int count) : base(count) { }

            public IntDataSource(int min, int max, int count) : base(min, max, count) { }

            protected override int GetNext(Randomizer randomizer)
            {
                return randomizer.Next();
            }

            protected override int GetNext(Randomizer randomizer, int min, int max)
            {
                return randomizer.Next(min, max);
            }

            protected override bool CanBeDistinct(int min, int max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region UIntDataSource

        class UIntDataSource : RandomDataSource<uint>
        {
            public UIntDataSource(int count) : base(count) { }

            public UIntDataSource(uint min, uint max, int count) : base(min, max, count) { }

            protected override uint GetNext(Randomizer randomizer)
            {
                return randomizer.NextUInt();
            }

            protected override uint GetNext(Randomizer randomizer, uint min, uint max)
            {
                return randomizer.NextUInt(min, max);
            }

            protected override bool CanBeDistinct(uint min, uint max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region LongDataSource

        class LongDataSource : RandomDataSource<long>
        {
            public LongDataSource(int count) : base(count) { }

            public LongDataSource(long min, long max, int count) : base(min, max, count) { }

            protected override long GetNext(Randomizer randomizer)
            {
                return randomizer.NextLong();
            }

            protected override long GetNext(Randomizer randomizer, long min, long max)
            {
                return randomizer.NextLong(min, max);
            }

            protected override bool CanBeDistinct(long min, long max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region ULongDataSource

        class ULongDataSource : RandomDataSource<ulong>
        {
            public ULongDataSource(int count) : base(count) { }

            public ULongDataSource(ulong min, ulong max, int count) : base(min, max, count) { }

            protected override ulong GetNext(Randomizer randomizer)
            {
                return randomizer.NextULong();
            }

            protected override ulong GetNext(Randomizer randomizer, ulong min, ulong max)
            {
                return randomizer.NextULong(min, max);
            }

            protected override bool CanBeDistinct(ulong min, ulong max, int count)
            {
                return (uint)count <= max - min;
            }
        }

        #endregion

        #region ShortDataSource

        class ShortDataSource : RandomDataSource<short>
        {
            public ShortDataSource(int count) : base(count) { }

            public ShortDataSource(short min, short max, int count) : base(min, max, count) { }

            protected override short GetNext(Randomizer randomizer)
            {
                return randomizer.NextShort();
            }

            protected override short GetNext(Randomizer randomizer, short min, short max)
            {
                return randomizer.NextShort(min, max);
            }

            protected override bool CanBeDistinct(short min, short max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region UShortDataSource

        class UShortDataSource : RandomDataSource<ushort>
        {
            public UShortDataSource(int count) : base(count) { }

            public UShortDataSource(ushort min, ushort max, int count) : base(min, max, count) { }

            protected override ushort GetNext(Randomizer randomizer)
            {
                return randomizer.NextUShort();
            }

            protected override ushort GetNext(Randomizer randomizer, ushort min, ushort max)
            {
                return randomizer.NextUShort(min, max);
            }

            protected override bool CanBeDistinct(ushort min, ushort max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region DoubleDataSource

        class DoubleDataSource : RandomDataSource<double>
        {
            public DoubleDataSource(int count) : base(count) { }

            public DoubleDataSource(double min, double max, int count) : base(min, max, count) { }

            protected override double GetNext(Randomizer randomizer)
            {
                return randomizer.NextDouble();
            }

            protected override double GetNext(Randomizer randomizer, double min, double max)
            {
                return randomizer.NextDouble(min, max);
            }

            protected override bool CanBeDistinct(double min, double max, int count)
            {
                return true;
            }
        }

        #endregion

        #region FloatDataSource

        class FloatDataSource : RandomDataSource<float>
        {
            public FloatDataSource(int count) : base(count) { }

            public FloatDataSource(float min, float max, int count) : base(min, max, count) { }

            protected override float GetNext(Randomizer randomizer)
            {
                return randomizer.NextFloat();
            }

            protected override float GetNext(Randomizer randomizer, float min, float max)
            {
                return randomizer.NextFloat(min, max);
            }

            protected override bool CanBeDistinct(float min, float max, int count)
            {
                return true;
            }
        }

        #endregion

        #region ByteDataSource

        class ByteDataSource : RandomDataSource<byte>
        {
            public ByteDataSource(int count) : base(count) { }

            public ByteDataSource(byte min, byte max, int count) : base(min, max, count) { }

            protected override byte GetNext(Randomizer randomizer)
            {
                return randomizer.NextByte();
            }

            protected override byte GetNext(Randomizer randomizer, byte min, byte max)
            {
                return randomizer.NextByte(min, max);
            }

            protected override bool CanBeDistinct(byte min, byte max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region SByteDataSource

        class SByteDataSource : RandomDataSource<sbyte>
        {
            public SByteDataSource(int count) : base(count) { }

            public SByteDataSource(sbyte min, sbyte max, int count) : base(min, max, count) { }

            protected override sbyte GetNext(Randomizer randomizer)
            {
                return randomizer.NextSByte();
            }

            protected override sbyte GetNext(Randomizer randomizer, sbyte min, sbyte max)
            {
                return randomizer.NextSByte(min, max);
            }

            protected override bool CanBeDistinct(sbyte min, sbyte max, int count)
            {
                return count <= max - min;
            }
        }

        #endregion

        #region EnumDataSource

        class EnumDataSource : RandomDataSource
        {
            private readonly int _count;

            private readonly List<object> previousValues = new List<object>();

            public EnumDataSource(int count) : base(typeof(Enum))
            {
                _count = count;
            }

            public override IEnumerable GetData(IParameterInfo parameter)
            {
                Guard.ArgumentValid(parameter.ParameterType.GetTypeInfo().IsEnum, "EnumDataSource requires an enum parameter", nameof(parameter));

                Randomizer randomizer = Randomizer.GetRandomizer(parameter.ParameterInfo);
                DataType = parameter.ParameterType;

                int valueCount = Enum.GetValues(DataType).Cast<int>().Distinct().Count();

                Guard.OperationValid(!(Distinct && _count > valueCount), $"The enum \"{DataType.Name}\" has {valueCount} values and the random value count is {_count} so the values cannot be distinct.");

                for (int i = 0; i < _count; i++)
                {
                    if (Distinct)
                    {
                        object next;

                        do
                        {
                            next = randomizer.NextEnum(parameter.ParameterType);
                        } while (previousValues.Contains(next));

                        previousValues.Add(next);

                        yield return next;
                    }
                    else
                        yield return randomizer.NextEnum(parameter.ParameterType);
                }
            }
        }

        #endregion

        #region DecimalDataSource

        class DecimalDataSource : RandomDataSource<decimal>
        {
            public DecimalDataSource(int count) : base(count) { }

            protected override decimal GetNext(Randomizer randomizer)
            {
                return randomizer.NextDecimal();
            }

            protected override decimal GetNext(Randomizer randomizer, decimal min, decimal max)
            {
                return randomizer.NextDecimal(min, max);
            }

            protected override bool CanBeDistinct(decimal min, decimal max, int count)
            {
                return true;
            }
        }

        #endregion

        #endregion
    }
}
