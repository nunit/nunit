// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        public double Zero = 0;

        [Datapoint]
        public double Positive = 1;

        [Datapoint]
        public double Negative = -1;

        [Datapoint]
        public double Max = double.MaxValue;

        [Datapoint]
        public double Infinity = double.PositiveInfinity;
    }

    public class SquareRootTest_Field_ArrayOfDouble : SquareRootTest
    {
        [Datapoints]
        public double[] Values = new[] { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
    }

    public class SquareRootTest_Field_IEnumerableOfDouble : SquareRootTest
    {
        [Datapoints]
        public IEnumerable<double> Values = new List<double> { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
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
        public double[] Values =>
            new[]
            {
                0.0,
                1.0,
                -1.0,
                double.MaxValue,
                double.PositiveInfinity
            };
    }

    public class SquareRootTest_Method_ArrayOfDouble : SquareRootTest
    {
        [Datapoints]
        public double[] GetValues()
        {
            return new[] { 0.0, 1.0, -1.0, double.MaxValue, double.PositiveInfinity };
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
        public double Zero = 0;

        [Datapoint]
        public double Positive = 1;

        [Datapoint]
        public double Negative = -1;
    }

    public class DatapointCanBeInherited : InheritedDatapointSquareRoot
    {
        [Datapoint]
        public double Max = double.MaxValue;

        [Datapoint]
        public double Infinity = double.PositiveInfinity;
    }

    public class InheritedDatapointsSquareRoot : SquareRootTest
    {
        [Datapoints]
        public double[] GetCommonValues()
        {
            return new[] { 0.0, 1.0, -1.0 };
        }
    }

    public class DatapointsCanBeInherited : InheritedDatapointsSquareRoot
    {
        [Datapoints]
        public double[] GetValues()
        {
            return new[] { double.MaxValue, double.PositiveInfinity };
        }
    }
}
