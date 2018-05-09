// ***********************************************************************
// Copyright (c) 2008-2018 Charlie Poole, Rob Prouse
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
using System.Globalization;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// RandomAttribute is used to supply a set of random values
    /// to a single parameter of a parameterized test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RandomAttribute : DataAttribute, IParameterDataSource
    {
        private RandomDataSource _source;
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
        /// Construct a set of ints within a specified range
        /// </summary>
        public RandomAttribute(int min, int max, int count)
        {
            _source = new IntDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of unsigned ints within a specified range
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(uint min, uint max, int count)
        {
            _source = new UIntDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of longs within a specified range
        /// </summary>
        public RandomAttribute(long min, long max, int count)
        {
            _source = new LongDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of unsigned longs within a specified range
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(ulong min, ulong max, int count)
        {
            _source = new ULongDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of shorts within a specified range
        /// </summary>
        public RandomAttribute(short min, short max, int count)
        {
            _source = new ShortDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of unsigned shorts within a specified range
        /// </summary>
        [CLSCompliant(false)]
        public RandomAttribute(ushort min, ushort max, int count)
        {
            _source = new UShortDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of doubles within a specified range
        /// </summary>
        public RandomAttribute(double min, double max, int count)
        {
            _source = new DoubleDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of floats within a specified range
        /// </summary>
        public RandomAttribute(float min, float max, int count)
        {
            _source = new FloatDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of bytes within a specified range
        /// </summary>
        public RandomAttribute(byte min, byte max, int count)
        {
            _source = new ByteDataSource(min, max, count);
        }

        /// <summary>
        /// Construct a set of sbytes within a specified range
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
        /// <param name="fixtureType">The point of context in the fixture’s inheritance hierarchy.</param>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
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

            return _source.GetData(fixtureType, parameter);
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
            public Type DataType { get; protected set; }
            public bool Distinct { get; set; }

            public abstract IEnumerable GetData(Type fixtureType, ParameterInfo parameter);
        }

        abstract class RandomDataSource<T> : RandomDataSource
        {
            private readonly T _min;
            private readonly T _max;
            private readonly int _count;
            private readonly bool _inRange;

            private readonly List<T> previousValues = new List<T>();

            protected Randomizer _randomizer;

            protected RandomDataSource(int count)
            {
                _count = count;
                _inRange = false;

                DataType = typeof(T);
            }

            protected RandomDataSource(T min, T max, int count)
            {
                _min = min;
                _max = max;
                _count = count;
                _inRange = true;

                DataType = typeof(T);
            }

            public override IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
            {
                //Guard.ArgumentValid(parameter.ParameterType == typeof(T), "Parameter type must be " + typeof(T).Name, "parameter");

                _randomizer = Randomizer.GetRandomizer(parameter);

                Guard.OperationValid(!(Distinct && _inRange && !CanBeDistinct(_min, _max, _count)), $"The range of values is [{_min}, {_max}[ and the random value count is {_count} so the values cannot be distinct.");
                

                for (int i = 0; i < _count; i++)
                {
                    if (Distinct)
                    {
                        T next;

                        do
                        {
                            next = _inRange
                                ? GetNext(_min, _max)
                                : GetNext();
                        } while (previousValues.Contains(next));

                        previousValues.Add(next);
                        
                        yield return next;
                    }
                    else
                        yield return _inRange
                            ? GetNext(_min, _max)
                            : GetNext();
                }
            }

            protected abstract T GetNext();
            protected abstract T GetNext(T min, T max);
            protected abstract bool CanBeDistinct(T min, T max, int count);
        }

        #endregion

        #region RandomDataConverter

        class RandomDataConverter : RandomDataSource
        {
            readonly IParameterDataSource _source;

            public RandomDataConverter(IParameterDataSource source)
            {
                _source = source;
            }

            public override IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
            {
                Type parmType = parameter.ParameterType;

                foreach (object obj in _source.GetData(fixtureType, parameter))
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

            protected override int GetNext()
            {
                return _randomizer.Next();
            }

            protected override int GetNext(int min, int max)
            {
                return _randomizer.Next(min, max);
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

            protected override uint GetNext()
            {
                return _randomizer.NextUInt();
            }

            protected override uint GetNext(uint min, uint max)
            {
                return _randomizer.NextUInt(min, max);
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

            protected override long GetNext()
            {
                return _randomizer.NextLong();
            }

            protected override long GetNext(long min, long max)
            {
                return _randomizer.NextLong(min, max);
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

            protected override ulong GetNext()
            {
                return _randomizer.NextULong();
            }

            protected override ulong GetNext(ulong min, ulong max)
            {
                return _randomizer.NextULong(min, max);
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

            protected override short GetNext()
            {
                return _randomizer.NextShort();
            }

            protected override short GetNext(short min, short max)
            {
                return _randomizer.NextShort(min, max);
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

            protected override ushort GetNext()
            {
                return _randomizer.NextUShort();
            }

            protected override ushort GetNext(ushort min, ushort max)
            {
                return _randomizer.NextUShort(min, max);
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

            protected override double GetNext()
            {
                return _randomizer.NextDouble();
            }

            protected override double GetNext(double min, double max)
            {
                return _randomizer.NextDouble(min, max);
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

            protected override float GetNext()
            {
                return _randomizer.NextFloat();
            }

            protected override float GetNext(float min, float max)
            {
                return _randomizer.NextFloat(min, max);
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

            protected override byte GetNext()
            {
                return _randomizer.NextByte();
            }

            protected override byte GetNext(byte min, byte max)
            {
                return _randomizer.NextByte(min, max);
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

            protected override sbyte GetNext()
            {
                return _randomizer.NextSByte();
            }

            protected override sbyte GetNext(sbyte min, sbyte max)
            {
                return _randomizer.NextSByte(min, max);
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

            public EnumDataSource(int count)
            {
                _count = count;
                DataType = typeof(Enum);
            }

            public override IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
            {
                Guard.ArgumentValid(parameter.ParameterType.GetTypeInfo().IsEnum, "EnumDataSource requires an enum parameter", nameof(parameter));

                Randomizer randomizer = Randomizer.GetRandomizer(parameter);
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

        // Currently, Randomizer doesn't implement methods for decimal
        // so we use random Ulongs and convert them. This doesn't cover
        // the full range of decimal, so it's temporary.
        class DecimalDataSource : RandomDataSource<decimal>
        {
            public DecimalDataSource(int count) : base(count) { }

            public DecimalDataSource(decimal min, decimal max, int count) : base(min, max, count) { }

            protected override decimal GetNext()
            {
                return _randomizer.NextDecimal();
            }

            protected override decimal GetNext(decimal min, decimal max)
            {
                return _randomizer.NextDecimal(min, max);
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
