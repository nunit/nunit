// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Supplies a range of values to an individual parameter of a parameterized test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class RangeAttribute : NUnitAttribute, IParameterDataSource
    {
        private readonly object _from;
        private readonly object _to;
        private readonly object _step;

        #region Ints

        /// <summary>
        /// Constructs a range of <see cref="int"/> values using the default step of 1.
        /// </summary>
        public RangeAttribute(int from, int to) : this(from, to, from > to ? -1 : 1) { }

        /// <summary>
        /// Constructs a range of <see cref="int"/> values with the specified step size.
        /// </summary>
        public RangeAttribute(int from, int to, int step)
        {
            Guard.ArgumentValid(step > 0 && to >= from || step < 0 && to <= from,
                "Step must be positive with to >= from or negative with to <= from", nameof(step));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Constructs a range of <see cref="uint"/> values using the default step of 1.
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(uint from, uint to) : this(from, to, 1u) { }

        /// <summary>
        /// Constructs a range of <see cref="uint"/> values with the specified step size.
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(uint from, uint to, uint step)
        {
            Guard.ArgumentValid(step > 0, "Step must be greater than zero", nameof(step));
            Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", nameof(to));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        #region Longs

        /// <summary>
        /// Constructs a range of <see cref="long"/> values using a default step of 1.
        /// </summary>
        public RangeAttribute(long from, long to) : this(from, to, from > to ? -1L : 1L) { }

        /// <summary>
        /// Constructs a range of <see cref="long"/> values with the specified step size.
        /// </summary>
        public RangeAttribute(long from, long to, long step)
        {
            Guard.ArgumentValid(step > 0L && to >= from || step < 0L && to <= from,
                "Step must be positive with to >= from or negative with to <= from", nameof(step));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Constructs a range of <see cref="ulong"/> values using the default step of 1.
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to) : this(from, to, 1ul) { }

        /// <summary>
        /// Constructs a range of <see cref="ulong"/> values with the specified step size.
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to, ulong step)
        {
            Guard.ArgumentValid(step > 0, "Step must be greater than zero", nameof(step));
            Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", nameof(to));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Constructs a range of <see cref="double"/> values with the specified step size.
        /// </summary>
        public RangeAttribute(double from, double to, double step)
        {
            Guard.ArgumentValid(step > 0.0D && to >= from || step < 0.0D && to <= from,
                "Step must be positive with to >= from or negative with to <= from", nameof(step));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        #region Floats

        /// <summary>
        /// Constructs a range of <see cref="float"/> values with the specified step size.
        /// </summary>
        public RangeAttribute(float from, float to, float step)
        {
            Guard.ArgumentValid(step > 0.0F && to >= from || step < 0.0F && to <= from,
                "Step must be positive with to >= from or negative with to <= from", nameof(step));

            _from = from;
            _to = to;
            _step = step;
        }

        #endregion

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(IParameterInfo parameter)
        {
            var from = ParamAttributeTypeConversions.Convert(_from, parameter.ParameterType);
            var to = ParamAttributeTypeConversions.Convert(_to, parameter.ParameterType);

            var valueGenerator = ValueGenerator.Create(parameter.ParameterType);

            if (!valueGenerator.TryCreateStep(_step, out var step))
            {
                // ValueGenerator.CreateStep has the responsibility to enable Byte values to be incremented
                // by the Int32 value -1. Or perhaps in the future, DateTime values to be incremented by a TimeSpan.
                // It handles scenarios where the incrementing type is fundamentally different in its most natural form.

                // However, ParamAttributeTypeConversions has the responsibility to convert attribute arguments
                // that are only of a different type due to IL limitations or NUnit smoothing over overload differences.
                // See the XML docs for the ParamAttributeTypeConversions class.

                if (!ParamAttributeTypeConversions.TryConvert(_step, parameter.ParameterType, out var stepValueToRequire))
                {
                    // This will cause CreateStep to throw the same exception as it would throw if TryConvert
                    // succeeded but the value generator still didn’t recognize the step value.
                    stepValueToRequire = _step;
                }

                step = valueGenerator.CreateStep(stepValueToRequire);
            }

            return valueGenerator.GenerateRange(from, to, step);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        public override string ToString()
        {
            return $"{MsgUtils.FormatValue(_from)} .. {MsgUtils.FormatValue(_step)} .. {MsgUtils.FormatValue(_to)}";
        }
    }
}
