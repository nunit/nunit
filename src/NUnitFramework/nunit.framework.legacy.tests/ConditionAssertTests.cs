// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Linq;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class ConditionAssertTests
    {
        [Test]
        public void IsTrue()
        {
            ClassicAssert.IsTrue(true);
        }

        [Test]
        public void IsTrueNullable()
        {
            bool? actual = true;
            ClassicAssert.IsTrue(actual);
        }

        [Test]
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsTrue(false));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [TestCase(false, "  But was:  False")]
        [TestCase(null, "  But was:  null")]
        public void IsTrueFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsTrue(actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsFalse()
        {
            ClassicAssert.IsFalse(false);
        }

        [Test]
        public void IsFalseNullable()
        {
            bool? actual = false;
            ClassicAssert.IsFalse(actual);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsFalse(true));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [TestCase(true, "  But was:  True")]
        [TestCase(null, "  But was:  null")]
        public void IsFalseFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsFalse(actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNull()
        {
            ClassicAssert.IsNull(null);
        }

        [Test]
        public void IsNullFails()
        {
            string s1 = "S1";
            var expectedMessage =
                "  Expected: null" + Environment.NewLine +
                "  But was:  \"S1\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNull(s1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotNull()
        {
            string s1 = "S1";
            ClassicAssert.IsNotNull(s1);
        }

        [Test]
        public void IsNotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotNull(null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNaN()
        {
            ClassicAssert.IsNaN(double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                "  But was:  10.0d" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNaN(10.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmpty()
        {
            ClassicAssert.IsEmpty(string.Empty, "Failed on empty String");
            ClassicAssert.IsEmpty(Array.Empty<int>(), "Failed on empty Array");
            ClassicAssert.IsEmpty(Enumerable.Empty<int>(), "Failed on empty IEnumerable");

            ClassicAssert.IsEmpty(new ArrayList(), "Failed on empty ArrayList");
            ClassicAssert.IsEmpty(new Hashtable(), "Failed on empty Hashtable");
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  \"Hi!\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty("Hi!"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(default));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(new[] { 1, 2, 3 }));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(new[] { 1, 2, 3 }));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmpty()
        {
            int[] array = new[] { 1, 2, 3 };

            ClassicAssert.IsNotEmpty("Hi!", "Failed on String");
            ClassicAssert.IsNotEmpty(array, "Failed on Array");
            IEnumerable enumerable = array;
            ClassicAssert.IsNotEmpty(enumerable, "Failed on IEnumerable");

            var list = new ArrayList(array);
            var hash = new Hashtable { { "array", array } };

            ClassicAssert.IsNotEmpty(list, "Failed on ArrayList");
            ClassicAssert.IsNotEmpty(hash, "Failed on Hashtable");
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyString()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <string.Empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(string.Empty));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Array.Empty<int>()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Enumerable.Empty<int>()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new ArrayList()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new Hashtable()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
