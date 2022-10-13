// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Linq;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class ConditionAssertTests
    {
        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void IsTrueNullable()
        {
            bool? actual = true;
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsTrue(false));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [TestCase(false, "  But was:  False")]
        [TestCase(null,"  But was:  null")]
        public void IsTrueFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsTrue(actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsFalse()
        {
            Assert.IsFalse(false);
        }

        [Test]
        public void IsFalseNullable()
        {
            bool? actual = false;
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsFalse(true));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [TestCase(true,"  But was:  True")]
        [TestCase(null, "  But was:  null")]
        public void IsFalseFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsFalse(actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNull()
        {
            Assert.IsNull(null);
        }

        [Test]
        public void IsNullFails()
        {
            String s1 = "S1";
            var expectedMessage =
                "  Expected: null" + Environment.NewLine +
                "  But was:  \"S1\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNull(s1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotNull()
        {
            String s1 = "S1";
            Assert.IsNotNull(s1);
        }

        [Test]
        public void IsNotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotNull(null));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNaN()
        {
            Assert.IsNaN(double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                "  But was:  10.0d" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNaN(10.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty( "", "Failed on empty String" );
            Assert.IsEmpty(Array.Empty<int>(), "Failed on empty Array" );
            Assert.IsEmpty(Enumerable.Empty<int>(), "Failed on empty IEnumerable");

            Assert.IsEmpty( new ArrayList(), "Failed on empty ArrayList" );
            Assert.IsEmpty( new Hashtable(), "Failed on empty Hashtable" );
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  \"Hi!\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( "Hi!" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( (string)null ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( new[] { 1, 2, 3 } ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty((IEnumerable)new[] { 1, 2, 3 }));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmpty()
        {
            int[] array = new[] { 1, 2, 3 };

            Assert.IsNotEmpty( "Hi!", "Failed on String" );
            Assert.IsNotEmpty( array, "Failed on Array" );
            Assert.IsNotEmpty( (IEnumerable)array, "Failed on IEnumerable" );

            ArrayList list = new ArrayList(array);
            Hashtable hash = new Hashtable();
            hash.Add("array", array);

            Assert.IsNotEmpty(list, "Failed on ArrayList");
            Assert.IsNotEmpty(hash, "Failed on Hashtable");
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyString()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <string.Empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( "" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( Array.Empty<int>() ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty(Enumerable.Empty<int>()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty(new ArrayList()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty(new Hashtable()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}
