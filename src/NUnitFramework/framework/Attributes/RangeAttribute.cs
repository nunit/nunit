// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole
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

namespace NUnit.Framework
{
    /// <summary>
    /// RangeAttribute is used to supply a range of _values to an
    /// individual parameter of a parameterized test.
    /// </summary>
    public class RangeAttribute : ValuesAttribute
    {
        #region Ints

        /// <summary>
        /// Construct a range of ints using default step of 1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RangeAttribute(int from, int to) : this(from, to, from > to ? -1 : 1) { }

        /// <summary>
        /// Construct a range of ints specifying the step size 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        public RangeAttribute(int from, int to, int step)
        {
            Guard.ArgumentValid(step > 0 && to >= from || step < 0 && to <= from,
                "Step must be positive with to >= from or negative with to <= from", "step");

            int count = (to - from) / step + 1;
            this.data = new object[count];
            int index = 0;
            for (int val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Construct a range of unsigned ints using default step of 1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        [CLSCompliant(false)]
        public RangeAttribute(uint from, uint to) : this(from, to, 1u) { }

        /// <summary>
        /// Construct a range of unsigned ints specifying the step size 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        [CLSCompliant(false)]
        public RangeAttribute(uint from, uint to, uint step)
        {
            Guard.ArgumentValid(step > 0, "Step must be greater than zero", "step");
            Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", "to");

            uint count = (to - from) / step + 1;
            this.data = new object[count];
            uint index = 0;
            for (uint val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion

        #region Longs

        /// <summary>
        /// Construct a range of longs using a default step of 1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RangeAttribute(long from, long to) : this(from, to, from > to ? -1L : 1L) { }

        /// <summary>
        /// Construct a range of longs
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        public RangeAttribute(long from, long to, long step)
        {
            Guard.ArgumentValid(step > 0L && to >= from || step < 0L && to <= from,
                "Step must be positive with to >= from or negative with to <= from", "step");

            long count = (to - from) / step + 1;
            this.data = new object[count];
            int index = 0;
            for (long val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Construct a range of unsigned longs using default step of 1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        [CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to) : this(from, to, 1ul) { }

        /// <summary>
        /// Construct a range of unsigned longs specifying the step size 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        [CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to, ulong step)
        {
            Guard.ArgumentValid(step > 0, "Step must be greater than zero", "step");
            Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", "to");

            ulong count = (to - from) / step + 1;
            this.data = new object[count];
            ulong index = 0;
            for (ulong val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Construct a range of doubles
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        public RangeAttribute(double from, double to, double step)
        {
            Guard.ArgumentValid(step > 0.0D && to >= from || step < 0.0D && to <= from,
                "Step must be positive with to >= from or negative with to <= from", "step");

            double aStep = Math.Abs(step);
            double tol = aStep / 1000;
            int count = (int)(Math.Abs(to - from) / aStep + tol + 1);
            this.data = new object[count];
            int index = 0;
            for (double val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion

        #region Floats

        /// <summary>
        /// Construct a range of floats
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        public RangeAttribute(float from, float to, float step)
        {
            Guard.ArgumentValid(step > 0.0F && to >= from || step < 0.0F && to <= from,
                "Step must be positive with to >= from or negative with to <= from", "step");

            float aStep = Math.Abs(step);
            float tol = aStep / 1000;
            int count = (int)(Math.Abs(to - from) / aStep + tol + 1);
            this.data = new object[count];
            int index = 0;
            for (float val = from; index < count; val += step)
                this.data[index++] = val;
        }

        #endregion
    }
}
