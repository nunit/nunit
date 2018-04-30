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
using System.Reflection;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// RangeAttribute is used to supply a range of values to an
    /// individual parameter of a parameterized test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class RangeAttribute : DataAttribute, IParameterDataSource
    {
        private readonly object _from;
        private readonly object _to;
        private readonly object _step;

        #region Ints

        /// <summary>
        /// Construct a range of ints using default step of 1
        /// </summary>
        public RangeAttribute(int from, int to) : this(from, to, from > to ? -1 : 1) { }

        /// <summary>
        /// Construct a range of ints specifying the step size
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
        /// Construct a range of unsigned ints using default step of 1
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(uint from, uint to) : this(from, to, 1u) { }

        /// <summary>
        /// Construct a range of unsigned ints specifying the step size
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
        /// Construct a range of longs using a default step of 1
        /// </summary>
        public RangeAttribute(long from, long to) : this(from, to, from > to ? -1L : 1L) { }

        /// <summary>
        /// Construct a range of longs
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
        /// Construct a range of unsigned longs using default step of 1
        /// </summary>
        [CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to) : this(from, to, 1ul) { }

        /// <summary>
        /// Construct a range of unsigned longs specifying the step size
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
        /// Construct a range of doubles
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
        /// Construct a range of floats
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
        /// <param name="fixtureType">The point of context in the fixture’s inheritance hierarchy.</param>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
        {
            var from = ParamAttributeTypeConversions.Convert(_from, parameter.ParameterType);
            var to = ParamAttributeTypeConversions.Convert(_to, parameter.ParameterType);

            var valueGenerator = ValueGenerator.Create(parameter.ParameterType);

            ValueGenerator.Step step;
            if (!valueGenerator.TryCreateStep(_step, out step))
            {
                // ValueGenerator.CreateStep has the responsibility to enable Byte values to be incremented
                // by the Int32 value -1. Or perhaps in the future, DateTime values to be incremented by a TimeSpan.
                // It handles scenarios where the incrementing type is fundamentally different in its most natural form.

                // However, ParamAttributeTypeConversions has the responsibility to convert attribute arguments
                // that are only of a different type due to IL limitations or NUnit smoothing over overload differences.
                // See the XML docs for the ParamAttributeTypeConversions class.

                object stepValueToRequire;
                if (!ParamAttributeTypeConversions.TryConvert(_step, parameter.ParameterType, out stepValueToRequire))
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
