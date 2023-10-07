// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// Summary description for ArrayEqualsFailureMessageFixture.
    /// </summary>
    // TODO: Exact tests of messages are fragile - revisit this
    [TestFixture]
    public class ArrayEqualsFailureMessageFixture
    {
        private static readonly string NL = Environment.NewLine;

        [Test]
        public void ArraysHaveDifferentRanks()
        {
            var expected = new[] { 1, 2, 3, 4 };
            var actual = new[,] { { 1, 2 }, { 3, 4 } };

            var expectedMessage =
                "  Expected is <System.Int32[4]>, actual is <System.Int32[2,2]>" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ExpectedArrayIsLonger()
        {
            var expected = new[] { 1, 2, 3, 4, 5 };
            var actual = new[] { 1, 2, 3 };

            var expectedMessage =
                "  Expected is <System.Int32[5]>, actual is <System.Int32[3]>" + NL +
                "  Values differ at index [3]" + NL +
                "  Missing:  < 4, 5 >";

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ActualArrayIsLonger()
        {
            var expected = new[] { 1, 2, 3 };
            var actual = new[] { 1, 2, 3, 4, 5, 6, 7 };

            var expectedMessage =
                "  Expected is <System.Int32[3]>, actual is <System.Int32[7]>" + NL +
                "  Values differ at index [3]" + NL +
                "  Extra:    < 4, 5, 6... >";

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FailureOnSingleDimensionedArrays()
        {
            var expected = new[] { 1, 2, 3 };
            var actual = new[] { 1, 5, 3 };

            var expectedMessage =
                "  Expected and actual are both <System.Int32[3]>" + NL +
                "  Values differ at index [1]" + NL +
                TextMessageWriter.Pfx_Expected + "2" + NL +
                TextMessageWriter.Pfx_Actual + "5" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DoubleDimensionedArrays()
        {
            var expected = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            var actual = new[,] { { 1, 3, 2 }, { 4, 0, 6 } };

            var expectedMessage =
                "  Expected and actual are both <System.Int32[2,3]>" + NL +
                "  Values differ at index [0,1]" + NL +
                TextMessageWriter.Pfx_Expected + "2" + NL +
                TextMessageWriter.Pfx_Actual + "3" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void TripleDimensionedArrays()
        {
            var expected = new[, ,] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } };
            var actual = new[, ,] { { { 1, 2 }, { 3, 4 } }, { { 0, 6 }, { 7, 8 } } };

            var expectedMessage =
                "  Expected and actual are both <System.Int32[2,2,2]>" + NL +
                "  Values differ at index [1,0,0]" + NL +
                TextMessageWriter.Pfx_Expected + "5" + NL +
                TextMessageWriter.Pfx_Actual + "0" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void FiveDimensionedArrays()
        {
            var expected = new int[2, 2, 2, 2, 2] { { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } };
            var actual = new int[2, 2, 2, 2, 2] { { { { { 1, 2 }, { 4, 3 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } }, { { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } }, { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } } } } };

            var expectedMessage =
                "  Expected and actual are both <System.Int32[2,2,2,2,2]>" + NL +
                "  Values differ at index [0,0,0,1,0]" + NL +
                TextMessageWriter.Pfx_Expected + "3" + NL +
                TextMessageWriter.Pfx_Actual + "4" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void JaggedArrays()
        {
            var expected = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6, 7 }, new[] { 8, 9 } };
            var actual = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 0, 7 }, new[] { 8, 9 } };

            var expectedMessage =
                "  Expected and actual are both <System.Int32[3][]>" + NL +
                "  Values differ at index [1]" + NL +
                "    Expected and actual are both <System.Int32[4]>" + NL +
                "    Values differ at index [2]" + NL +
                TextMessageWriter.Pfx_Expected + "6" + NL +
                TextMessageWriter.Pfx_Actual + "0" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void JaggedArrayComparedToSimpleArray()
        {
            var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var actual = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 0, 7 }, new[] { 8, 9 } };

            var expectedMessage =
                "  Expected is <System.Int32[9]>, actual is <System.Int32[3][]>" + NL +
                "  Values differ at index [0]" + NL +
                TextMessageWriter.Pfx_Expected + "1" + NL +
                TextMessageWriter.Pfx_Actual + "< 1, 2, 3 >" + NL;

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ArraysWithDifferentRanksAsCollection()
        {
            var expected = new[] { 1, 2, 3, 4 };
            var actual = new[,] { { 1, 0 }, { 3, 4 } };

            var expectedMessage =
                "  Expected is <System.Int32[4]>, actual is <System.Int32[2,2]>" + NL +
                "  Values differ at expected index [1], actual index [0,1]" + NL +
                TextMessageWriter.Pfx_Expected + "2" + NL +
                TextMessageWriter.Pfx_Actual + "0" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected).AsCollection));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ArraysWithDifferentDimensionsAsCollection()
        {
            var expected = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            var actual = new[,] { { 1, 2 }, { 3, 0 }, { 5, 6 } };

            var expectedMessage =
                "  Expected is <System.Int32[2,3]>, actual is <System.Int32[3,2]>" + NL +
                "  Values differ at expected index [1,0], actual index [1,1]" + NL +
                TextMessageWriter.Pfx_Expected + "4" + NL +
                TextMessageWriter.Pfx_Actual + "0" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected).AsCollection));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void SameLengthDifferentContent()
        {
            string[] actual = { "one", "two", "three" };
            string[] expected = { "one", "two", "ten" };

            var expectedMessage =
                "  Expected and actual are both <System.String[3]>" + NL +
                "  Values differ at index [2]" + NL +
                "  Expected string length 3 but was 5. Strings differ at index 1." + NL +
                "  Expected: \"ten\"" + NL +
                "  But was:  \"three\"" + NL +
                "  ------------^" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ArraysDeclaredAsDifferentTypes()
        {
            string[] actual = { "one", "two", "three" };
            object[] expected = { "one", "three", "two" };

            var expectedMessage =
                "  Expected is <System.Object[3]>, actual is <System.String[3]>" + NL +
                "  Values differ at index [1]" + NL +
                "  Expected string length 5 but was 3. Strings differ at index 1." + NL +
                "  Expected: \"three\"" + NL +
                "  But was:  \"two\"" + NL +
                "  ------------^" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ArrayAndCollection_Failure()
        {
            var expected = new[] { 1, 2, 3 };
            var actual = new List<int>(new[] { 1, 3 });

            var expectedMessage =
                "  Expected is <System.Int32[3]>, actual is <System.Collections.Generic.List`1[System.Int32]> with 2 elements" + NL +
                "  Values differ at index [1]" + NL +
                "  Expected: 2" + NL +
                "  But was:  3" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected).AsCollection));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DifferentArrayTypesEqualFails()
        {
            string[] array1 = { "one", "two", "three" };
            object[] array2 = { "one", "three", "two" };

            var expectedMessage =
                "  Expected is <System.String[3]>, actual is <System.Object[3]>" + NL +
                "  Values differ at index [1]" + NL +
                "  Expected string length 3 but was 5. Strings differ at index 1." + NL +
                "  Expected: \"two\"" + NL +
                "  But was:  \"three\"" + NL +
                "  ------------^" + NL;

            var ex = Assert.Throws<AssertionException>(() => Assert.That(array2, Is.EqualTo(array1)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
