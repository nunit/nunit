// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.TestData.DatapointFixture
{
    public abstract class SquareRootTest
    {
        [Theory]
        public void SqrtTimesItselfGivesOriginal(double num)
        {
            Assume.That(num >= 0.0 && num < double.MaxValue);

            double sqrt = Math.Sqrt(num);

            Assert.That(sqrt >= 0.0);
            Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
        }
    }

    public class SquareRootTest_Field_Double : SquareRootTest
    {
        [Datapoint]
        public double zero = 0;

        [Datapoint]
        public double positive = 1;

        [Datapoint]
        public double negative = -1;

        [Datapoint]
        public double max = double.MaxValue;

        [Datapoint]
        public double infinity = double.PositiveInfinity;
    }

    public class SquareRootTest_Field_ArrayOfDouble : SquareRootTest
    {
        [Datapoints]
        public double[] values = new double[] { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
    }

    public class SquareRootTest_Field_IEnumerableOfDouble : SquareRootTest
    {
        [Datapoints]
        public IEnumerable<double> values = new List<double> { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
    }

    public class SquareRootTest_Property_IEnumerableOfDouble : SquareRootTest
    {
        [Datapoints]
        public IEnumerable<double> Values
        {
            get 
            {
                List<double> list = new List<double>();
                list.Add(0.0);
                list.Add(1.0);
                list.Add(-1.0);
                list.Add(double.MaxValue);
                list.Add(double.PositiveInfinity);
                return list;
            }
        }
    }

    public class SquareRootTest_Method_IEnumerableOfDouble : SquareRootTest
    {
        [Datapoints]
        public IEnumerable<double> GetValues()
        {
            List<double> list = new List<double>();
            list.Add(0.0);
            list.Add(1.0);
            list.Add(-1.0);
            list.Add(double.MaxValue);
            list.Add(double.PositiveInfinity);
            return list;
        }
    }

    public class SquareRootTest_Property_ArrayOfDouble : SquareRootTest
    {
        [Datapoints]
        public double[] Values
        {
            get { return new double[] { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity }; }
        }
    }

    public class SquareRootTest_Method_ArrayOfDouble : SquareRootTest
    {
        [Datapoints]
        public double[] GetValues()
        {
            return new double[] { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
        }
    }
 
    public class SquareRootTest_Iterator_IEnumerableOfDouble : SquareRootTest
    {
        [Datapoints]
        public IEnumerable<double> GetValues()
        {
            yield return 0.0;

            yield return 1.0;
            yield return -1.0;
            yield return double.MaxValue;
            yield return double.PositiveInfinity;
        }
    }

    public class InheritedDatapointSquareRoot : SquareRootTest
    {
        [Datapoint]
        public double zero = 0;

        [Datapoint]
        public double positive = 1;

        [Datapoint]
        public double negative = -1;
    }

    public class DatapointCanBeInherited : InheritedDatapointSquareRoot
    {
        [Datapoint]
        public double max = double.MaxValue;

        [Datapoint]
        public double infinity = double.PositiveInfinity;
    }

    public class InheritedDatapointsSquareRoot : SquareRootTest
    {
        [Datapoints]
        public double[] GetCommonValues()
        {
            return new double[] { 0.0, 1.0, -1.0 };
        }
    }

    public class DatapointsCanBeInherited : InheritedDatapointsSquareRoot
    {
        [Datapoints]
        public double[] GetValues()
        {
            return new double[] { double.MaxValue, double.PositiveInfinity };
        }
    }
}