// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Linq;

namespace NUnit.Framework.Classic.Tests
{
    [TestFixture]
    public class ConditionAssertTests
    {
        [Test]
        public void IsTrue()
        {
            Legacy.ClassicAssert.IsTrue(true);
        }

        [Test]
        public void IsTrueNullable()
        {
            bool? actual = true;
            Legacy.ClassicAssert.IsTrue(actual);
        }

        [Test]
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsTrue(false));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [TestCase(false, "  But was:  False")]
        [TestCase(null, "  But was:  null")]
        public void IsTrueFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsTrue(actual));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsFalse()
        {
            Legacy.ClassicAssert.IsFalse(false);
        }

        [Test]
        public void IsFalseNullable()
        {
            bool? actual = false;
            Legacy.ClassicAssert.IsFalse(actual);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsFalse(true));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [TestCase(true, "  But was:  True")]
        [TestCase(null, "  But was:  null")]
        public void IsFalseFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsFalse(actual));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNull()
        {
            Legacy.ClassicAssert.IsNull(null);
        }

        [Test]
        public void IsNullFails()
        {
            string s1 = "S1";
            var expectedMessage =
                "  Expected: null" + Environment.NewLine +
                "  But was:  \"S1\"" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNull(s1));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotNull()
        {
            string s1 = "S1";
            Legacy.ClassicAssert.IsNotNull(s1);
        }

        [Test]
        public void IsNotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotNull(null));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNaN()
        {
            Legacy.ClassicAssert.IsNaN(double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                "  But was:  10.0d" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNaN(10.0));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmpty()
        {
            Legacy.ClassicAssert.IsEmpty(string.Empty, "Failed on empty String");
            Legacy.ClassicAssert.IsEmpty(Array.Empty<int>(), "Failed on empty Array");
            Legacy.ClassicAssert.IsEmpty(Enumerable.Empty<int>(), "Failed on empty IEnumerable");

            Legacy.ClassicAssert.IsEmpty(new ArrayList(), "Failed on empty ArrayList");
            Legacy.ClassicAssert.IsEmpty(new Hashtable(), "Failed on empty Hashtable");
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  \"Hi!\"" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsEmpty("Hi!"));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsEmpty(default));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsEmpty(new[] { 1, 2, 3 }));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsEmpty(new[] { 1, 2, 3 }));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmpty()
        {
            int[] array = new[] { 1, 2, 3 };

            Legacy.ClassicAssert.IsNotEmpty("Hi!", "Failed on String");
            Legacy.ClassicAssert.IsNotEmpty(array, "Failed on Array");
            IEnumerable enumerable = array;
            Legacy.ClassicAssert.IsNotEmpty(enumerable, "Failed on IEnumerable");

            var list = new ArrayList(array);
            var hash = new Hashtable { { "array", array } };

            Legacy.ClassicAssert.IsNotEmpty(list, "Failed on ArrayList");
            Legacy.ClassicAssert.IsNotEmpty(hash, "Failed on Hashtable");
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyString()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <string.Empty>" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotEmpty(string.Empty));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotEmpty(Array.Empty<int>()));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotEmpty(Enumerable.Empty<int>()));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotEmpty(new ArrayList()));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Framework.Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.IsNotEmpty(new Hashtable()));
            Framework.Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
