// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    internal class AssertTypesTests
    {
        [Test()]
        public void IsAssignableFrom()
        {
            var array10 = new int[10];

            ClassicAssert.IsAssignableFrom(typeof(int[]), array10);
            ClassicAssert.IsAssignableFrom(typeof(int[]), array10, "{0} type", "array");
            ClassicAssert.IsAssignableFrom<int[]>(array10);
            ClassicAssert.IsAssignableFrom<int[]>(array10, "{0} type", "array");
        }

        [Test]
        public void IsAssignableFromFails()
        {
            var array10 = new int[10];

            var expectedMessage =
                "  Expected: assignable from <System.Int32>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsAssignableFrom(typeof(int), array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsAssignableFrom(typeof(int), array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));

            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsAssignableFrom<int>(array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsAssignableFrom<int>(array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));
        }

        [Test()]
        public void IsNotAssignableFrom()
        {
            var array10 = new int[10];

            ClassicAssert.IsNotAssignableFrom(typeof(int[,]), array10);
            ClassicAssert.IsNotAssignableFrom(typeof(int[,]), array10, "{0} type", "multi-dimensional array");
            ClassicAssert.IsNotAssignableFrom<int[,]>(array10);
            ClassicAssert.IsNotAssignableFrom<int[,]>(array10, "{0} type", "multi-dimensional array");
        }

        [Test]
        public void IsNotAssignableFromFails()
        {
            var array10 = new int[10];

            var expectedMessage =
                "  Expected: not assignable from <System.Int32[]>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotAssignableFrom(typeof(int[]), array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotAssignableFrom(typeof(int[]), array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));

            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotAssignableFrom<int[]>(array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotAssignableFrom<int[]>(array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));
        }

        [Test]
        public void IsInstanceOf()
        {
            var array10 = new int[10];

            ClassicAssert.IsInstanceOf(typeof(int[]), array10);
            ClassicAssert.IsInstanceOf(typeof(int[]), array10, "{0} type", "array");
            ClassicAssert.IsInstanceOf<int[]>(array10);
            ClassicAssert.IsInstanceOf<int[]>(array10, "{0} type", "array");
        }

        [Test]
        public void IsInstanceOfFails()
        {
            var array10 = new int[10];

            var expectedMessage =
                "  Expected: instance of <System.Int32>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsInstanceOf(typeof(int), array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsInstanceOf(typeof(int), array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));

            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsInstanceOf<int>(array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsInstanceOf<int>(array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));
        }

        [Test()]
        public void IsNotInstanceOf()
        {
            var array10 = new int[10];

            ClassicAssert.IsNotInstanceOf(typeof(int[,]), array10);
            ClassicAssert.IsNotInstanceOf(typeof(int[,]), array10, "{0} type", "multi-dimensional array");
            ClassicAssert.IsNotInstanceOf<int[,]>(array10);
            ClassicAssert.IsNotInstanceOf<int[,]>(array10, "{0} type", "multi-dimensional array");
        }

        [Test]
        public void IsNotInstanceOfFails()
        {
            var array10 = new int[10];

            var expectedMessage =
                "  Expected: not instance of <System.Int32[]>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotInstanceOf(typeof(int[]), array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotInstanceOf(typeof(int[]), array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));

            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotInstanceOf<int[]>(array10));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
            ex = Assert.Throws<AssertionException>(() => ClassicAssert.IsNotInstanceOf<int[]>(array10, "{0} type", "array"));
            Assert.That(ex?.Message, Does.Contain("array type"));
        }
    }
}
