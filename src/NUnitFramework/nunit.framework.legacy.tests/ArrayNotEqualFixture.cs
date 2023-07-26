// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Classic.Tests
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

            Legacy.ClassicAssert.AreNotEqual(array1, array2);
            Legacy.ClassicAssert.AreNotEqual(array2, array1);
        }

        [Test]
        public void SameLengthDifferentContent()
        {
            string[] array1 = { "one", "two", "three" };
            string[] array2 = { "one", "two", "ten" };
            Legacy.ClassicAssert.AreNotEqual(array1, array2);
            Legacy.ClassicAssert.AreNotEqual(array2, array1);
        }

        [Test]
        public void ArraysDeclaredAsDifferentTypes()
        {
            string[] array1 = { "one", "two", "three" };
            object[] array2 = { "one", "three", "two" };
            Legacy.ClassicAssert.AreNotEqual(array1, array2);
        }
    }
}
