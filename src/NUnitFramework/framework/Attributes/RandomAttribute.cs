// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// RandomAttribute is used to supply a set of random _values
    /// to a single parameter of a parameterized test.
    /// </summary>
    public class RandomAttribute : ValuesAttribute, IParameterDataSource
    {
        private enum SampleType
        {
            Auto,
            IntRange,
            DoubleRange,
        }

        private SampleType _sampleType;

        private int _count;
        private int _min, _max;
        private double _dmin, _dmax;

        /// <summary>
        /// Construct a set of Enums if the type is an Enum otherwise
        /// Construct a set of doubles from 0.0 to 1.0,
        /// specifying only the count.
        /// </summary>
        /// <param name="count"></param>
        public RandomAttribute(int count)
        {
            _count = count;
            _sampleType = SampleType.Auto;
        }

        /// <summary>
        /// Construct a set of doubles from min to max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="count"></param>
        public RandomAttribute(double min, double max, int count)
        {
            _count = count;
            _dmin = min;
            _dmax = max;
            _sampleType = SampleType.DoubleRange;
        }

        /// <summary>
        /// Construct a set of ints from min to max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="count"></param>
        public RandomAttribute(int min, int max, int count)
        {
            _count = count;
            _min = min;
            _max = max;
            _sampleType = SampleType.IntRange;
        }

        /// <summary>
        /// Get the collection of _values to be used as arguments.
        /// </summary>
        public new IEnumerable GetData(ParameterInfo parameter)
        {
            // Since a separate Randomizer is used for each parameter,
            // we can't fill in the data in the constructor of the
            // attribute. Only now, when GetData is called, do we have
            // sufficient information to create the values in a 
            // repeatable manner.
            Randomizer r = Randomizer.GetRandomizer(parameter);
            IList values;

            switch (_sampleType)
            {
                default:
                case SampleType.Auto:
                    if (parameter.ParameterType.IsEnum)
                        values = r.GetEnums(_count, parameter.ParameterType);
                    else
                        values = r.GetDoubles(_count);
                    break;
                case SampleType.IntRange:
                    values = r.GetInts(_min, _max, _count);
                    break;
                case SampleType.DoubleRange:
                    values = r.GetDoubles(_dmin, _dmax, _count);
                    break;
            }

            // Copy the random _values into the data array
            // and call the base class which may need to
            // convert them to another type.
            this.data = new object[values.Count];
            for (int i = 0; i < values.Count; i++)
                this.data[i] = values[i];

            return base.GetData(parameter);
        }
    }
}
