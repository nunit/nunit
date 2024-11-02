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
            ClassicAssert.IsTrue(true, "Really {0}", true);
        }

        [Test]
        public void IsTrueNullable()
        {
            bool? actual = true;
            ClassicAssert.IsTrue(actual);
            ClassicAssert.IsTrue(actual, "Really {0}", true);
        }

        [Test]
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsTrue(false));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsTrue(false, "Really {0}", true));
            Assert.That(ex?.Message, Does.Contain("Really True"));
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
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsTrue(actual, "Really {0}", true));
            Assert.That(ex?.Message, Does.Contain("Really True"));
        }

        [Test]
        public void True()
        {
            ClassicAssert.True(true);
            ClassicAssert.True(true, "Really {0}", true);
        }

        [Test]
        public void TrueNullable()
        {
            bool? actual = true;
            ClassicAssert.True(actual);
            ClassicAssert.True(actual, "Really {0}", true);
        }

        [Test]
        public void TrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.True(false));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.True(false, "Really {0}", true));
            Assert.That(ex?.Message, Does.Contain("Really True"));
        }

        [TestCase(false, "  But was:  False")]
        [TestCase(null, "  But was:  null")]
        public void TrueFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.True(actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.True(actual, "Really {0}", true));
            Assert.That(ex?.Message, Does.Contain("Really True"));
        }

        [Test]
        public void IsFalse()
        {
            ClassicAssert.IsFalse(false);
            ClassicAssert.IsFalse(false, "Really {0}", false);
        }

        [Test]
        public void IsFalseNullable()
        {
            bool? actual = false;
            ClassicAssert.IsFalse(actual);
            ClassicAssert.IsFalse(actual, "Really {0}", false);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsFalse(true));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsFalse(true, "Really {0}", false));
            Assert.That(ex?.Message, Does.Contain("Really False"));
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
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsFalse(actual, "Really {0}", false));
            Assert.That(ex?.Message, Does.Contain("Really False"));
        }

        [Test]
        public void False()
        {
            ClassicAssert.False(false);
            ClassicAssert.False(false, "Really {0}", false);
        }

        [Test]
        public void FalseNullable()
        {
            bool? actual = false;
            ClassicAssert.False(actual);
            ClassicAssert.False(actual, "Really {0}", false);
        }

        [Test]
        public void FalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.False(true));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.False(true, "Really {0}", false));
            Assert.That(ex?.Message, Does.Contain("Really False"));
        }

        [TestCase(true, "  But was:  True")]
        [TestCase(null, "  But was:  null")]
        public void FalseFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.False(actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.False(actual, "Really {0}", false));
            Assert.That(ex?.Message, Does.Contain("Really False"));
        }

        [Test]
        public void IsNull()
        {
            ClassicAssert.IsNull(null);
            ClassicAssert.IsNull(null, "Realy {0}", null);
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
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNull(s1, "Really {0}", "null"));
            Assert.That(ex?.Message, Does.Contain("Really null"));
        }

        [Test]
        public void Null()
        {
            ClassicAssert.Null(null);
            ClassicAssert.Null(null, "Realy {0}", null);
        }

        [Test]
        public void NullFails()
        {
            string s1 = "S1";
            var expectedMessage =
                "  Expected: null" + Environment.NewLine +
                "  But was:  \"S1\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.Null(s1));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.Null(s1, "Really {0}", "null"));
            Assert.That(ex?.Message, Does.Contain("Really null"));
        }

        [Test]
        public void IsNotNull()
        {
            string s1 = "S1";
            ClassicAssert.IsNotNull(s1);
            ClassicAssert.IsNotNull(s1, "Really {0}", "null");
        }

        [Test]
        public void IsNotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotNull(null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotNull(null, "Really {0}", "null"));
            Assert.That(ex?.Message, Does.Contain("Really null"));
        }

        [Test]
        public void NotNull()
        {
            string s1 = "S1";
            ClassicAssert.NotNull(s1);
            ClassicAssert.NotNull(s1, "Really {0}", "null");
        }

        [Test]
        public void NotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.NotNull(null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.NotNull(null, "Really {0}", "null"));
            Assert.That(ex?.Message, Does.Contain("Really null"));
        }

        [Test]
        public void IsNaN()
        {
            ClassicAssert.IsNaN(double.NaN);
            ClassicAssert.IsNaN(double.NaN, "Really {0}", double.NaN);
        }

        [Test]
        public void IsNullableNaN()
        {
            double? nan = double.NaN;
            ClassicAssert.IsNaN(nan);
            ClassicAssert.IsNaN(nan, "Really {0}", double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                "  But was:  10.0d" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNaN(10.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNaN(10.0, "Really {0}", double.NaN));
            Assert.That(ex?.Message, Does.Contain("Really NaN"));
        }

        [TestCase(null, "  But was:  null")]
        [TestCase(10.0, "  But was:  10.0d")]
        public void IsNullableNaNFails(double? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNaN(actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNaN(actual, "Really {0}", double.NaN));
            Assert.That(ex?.Message, Does.Contain("Really NaN"));
        }

        [Test]
        public void IsEmpty()
        {
            ClassicAssert.IsEmpty(string.Empty);
            ClassicAssert.IsEmpty(string.Empty, "Failed on empty String");

            ClassicAssert.IsEmpty(Array.Empty<int>(), "Failed on empty Array");
            ClassicAssert.IsEmpty(Enumerable.Empty<int>(), "Failed on empty IEnumerable");

            ClassicAssert.IsEmpty(new ArrayList());
            ClassicAssert.IsEmpty(new Hashtable());
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  \"Hi!\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty("Hi!"));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty("Hi!", "is {0}empty", "not "));
            Assert.That(ex?.Message, Does.Contain("is not empty"));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(default));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(default, "is {0}empty", "not "));
            Assert.That(ex?.Message, Does.Contain("is not empty"));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(new[] { 1, 2, 3 }));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsEmpty(new[] { 1, 2, 3 }, "is {0}empty", "not "));
            Assert.That(ex?.Message, Does.Contain("is not empty"));
        }

        [Test]
        public void IsNotEmpty()
        {
            int[] array = new[] { 1, 2, 3 };

            ClassicAssert.IsNotEmpty("Hi!");
            ClassicAssert.IsNotEmpty("Hi!", "Failed on String");

            ClassicAssert.IsNotEmpty(array);
            ClassicAssert.IsNotEmpty(array, "Failed on Array");

            IEnumerable enumerable = array;
            ClassicAssert.IsNotEmpty(enumerable);
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
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(string.Empty, "is {0}empty", string.Empty));
            Assert.That(ex?.Message, Does.Contain("is empty"));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Array.Empty<int>()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Array.Empty<int>(), "is {0}empty", string.Empty));
            Assert.That(ex?.Message, Does.Contain("is empty"));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Enumerable.Empty<int>()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(Enumerable.Empty<int>(), "is {0}empty", string.Empty));
            Assert.That(ex?.Message, Does.Contain("is empty"));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new Hashtable()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new Hashtable(), "is {0}empty", string.Empty));
            Assert.That(ex?.Message, Does.Contain("is empty"));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new Hashtable()));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotEmpty(new Hashtable(), "is {0}empty", string.Empty));
            Assert.That(ex?.Message, Does.Contain("is empty"));
        }
    }
}
