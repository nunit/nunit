// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using NUnit.Framework.Tests.TestUtilities.Collections;

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// Summary description for ArrayEqualTests.
    /// </summary>
    [TestFixture]
    public class ArrayEqualsFixture
    {
#pragma warning disable 183, 184 // error number varies in different runtimes
        // Used to detect runtimes where ArraySegments implement IEnumerable
        private static readonly bool ArraySegmentImplementsIEnumerable = new ArraySegment<int>() is IEnumerable;
#pragma warning restore 183, 184

        [Test]
        public void ArrayIsEqualToItself()
        {
            string[] array = { "one", "two", "three" };
            Assert.That(array, Is.SameAs(array));
            Assert.That(array, Is.EqualTo(array));
        }

        [Test]
        public void ArraysOfString()
        {
            string[] array1 = { "one", "two", "three" };
            string[] array2 = { "one", "two", "three" };
            Assert.That(array1 == array2, Is.False);
            Assert.Multiple(() =>
            {
                Assert.That(array2, Is.EqualTo(array1));
                Assert.That(array1, Is.EqualTo(array2));
            });
        }

        [Test]
        public void ArraysOfInt()
        {
            int[] a = { 1, 2, 3 };
            int[] b = { 1, 2, 3 };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArraysOfDouble()
        {
            double[] a = { 1.0, 2.0, 3.0 };
            double[] b = { 1.0, 2.0, 3.0 };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArraysOfDecimal()
        {
            decimal[] a = { 1.0m, 2.0m, 3.0m };
            decimal[] b = { 1.0m, 2.0m, 3.0m };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArrayOfIntAndArrayOfDouble()
        {
            int[] a = { 1, 2, 3 };
            double[] b = { 1.0, 2.0, 3.0 };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArraysDeclaredAsDifferentTypes()
        {
            string[] array1 = { "one", "two", "three" };
            object[] array2 = { "one", "two", "three" };
            Assert.That(array2, Is.EqualTo(array2), "String[] not equal to Object[]");
            Assert.That(array1, Is.EqualTo(array1), "Object[] not equal to String[]");
        }

        [Test]
        public void ArraysOfMixedTypes()
        {
            DateTime now = DateTime.Now;
            object[] array1 = { 1, 2.0f, 3.5d, 7.000m, "Hello", now };
            object[] array2 = { 1.0d, 2, 3.5, 7, "Hello", now };
            Assert.That(array2, Is.EqualTo(array1));
            Assert.That(array1, Is.EqualTo(array2));
        }

        [Test]
        public void DoubleDimensionedArrays()
        {
            int[,] a = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
            int[,] b = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void TripleDimensionedArrays()
        {
            int[,,] expected = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            int[,,] actual = { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void FiveDimensionedArrays()
        {
            int[,,,,] expected = new int[2, 2, 2, 2, 2] { { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } };
            int[,,,,] actual = new int[2, 2, 2, 2, 2] { { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } };

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ArraysOfArrays()
        {
            int[][] a = { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } };
            int[][] b = { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void JaggedArrays()
        {
            int[][] expected = { new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7 }, new[] { 8, 9 } };
            int[][] actual = { new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7 }, new[] { 8, 9 } };

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ArraysPassedAsObjects()
        {
            object a = new[] { 1, 2, 3 };
            object b = new[] { 1.0, 2.0, 3.0 };
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArrayAndCollection()
        {
            int[] a = { 1, 2, 3 };
            ICollection b = new SimpleObjectCollection(1, 2, 3);
            Assert.That(b, Is.EqualTo(a));
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void ArraysWithDifferentRanksComparedAsCollection()
        {
            int[] expected = { 1, 2, 3, 4 };
            int[,] actual = { { 1, 2 }, { 3, 4 } };

            Assert.That(actual, Is.Not.EqualTo(expected));
            Assert.That(actual, Is.EqualTo(expected).AsCollection);
        }

        [Test]
        public void ArraysWithDifferentDimensionsMatchedAsCollection()
        {
            int[,] expected = { { 1, 2, 3 }, { 4, 5, 6 } };
            int[,] actual = { { 1, 2 }, { 3, 4 }, { 5, 6 } };

            Assert.That(actual, Is.Not.EqualTo(expected));
            Assert.That(actual, Is.EqualTo(expected).AsCollection);
        }

        private static readonly int[] UnderlyingArray = { 1, 2, 3, 4, 5 };

        [Test]
        public void ArraySegmentAndArray()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new ArraySegment<int>(UnderlyingArray, 1, 3), Is.EqualTo(new[] { 2, 3, 4 }));
        }

        [Test]
        public void EmptyArraySegmentAndArray()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new ArraySegment<int>(), Is.Not.EqualTo(new[] { 2, 3, 4 }));
        }

        [Test]
        public void ArrayAndArraySegment()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new[] { 2, 3, 4 }, Is.EqualTo(new ArraySegment<int>(UnderlyingArray, 1, 3)));
        }

        [Test]
        public void ArrayAndEmptyArraySegment()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new[] { 2, 3, 4 }, Is.Not.EqualTo(new ArraySegment<int>()));
        }

        [Test]
        public void TwoArraySegments()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new ArraySegment<int>(UnderlyingArray, 1, 3), Is.EqualTo(new ArraySegment<int>(UnderlyingArray, 1, 3)));
        }

        [Test]
        public void TwoEmptyArraySegments()
        {
            Assume.That(ArraySegmentImplementsIEnumerable);
            Assert.That(new ArraySegment<int>(), Is.EqualTo(new ArraySegment<int>()));
        }
    }
}
