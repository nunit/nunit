// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    /// <summary>
    /// Summary description for ArrayNotEqualFixture.
    /// </summary>
    [TestFixture]
    public class ArrayNotEqualFixture
    {
        [Test]
        public void DifferentLengthArrays()
        {
            string[] array1 = { "one", "two", "three" };
            string[] array2 = { "one", "two", "three", "four", "five" };

            ClassicAssert.AreNotEqual(array1, array2);
            ClassicAssert.AreNotEqual(array2, array1);
        }

        [Test]
        public void SameLengthDifferentContent()
        {
            string[] array1 = { "one", "two", "three" };
            string[] array2 = { "one", "two", "ten" };
            ClassicAssert.AreNotEqual(array1, array2);
            ClassicAssert.AreNotEqual(array2, array1);
        }

        [Test]
        public void ArraysDeclaredAsDifferentTypes()
        {
            string[] array1 = { "one", "two", "three" };
            object[] array2 = { "one", "three", "two" };
            ClassicAssert.AreNotEqual(array1, array2);
        }
    }
}
