// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture()]
    public class TypeAssertTests
    {
        [Test]
        public void ExactType()
        {
            Assert.That("Hello", Is.TypeOf(typeof(string)));
        }

        [Test]
        public void ExactTypeFails()
        {
            var expectedMessage =
                "  Expected: <System.Int32>" + Environment.NewLine +
                "  But was:  <System.String>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That("Hello", Is.TypeOf(typeof(int))));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsInstanceOf()
        {
            var ex = new ArgumentException();

            Assert.That(ex, Is.InstanceOf(typeof(Exception)));
            Assert.That(ex, Is.InstanceOf<Exception>());
        }

        [Test]
        public void IsInstanceOfFails()
        {
            var expectedMessage =
                "  Expected: instance of <System.Int32>" + Environment.NewLine +
                "  But was:  <System.String>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That("abc123", Is.InstanceOf(typeof(int))));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotInstanceOf()
        {
            Assert.That("abc123", Is.Not.InstanceOf(typeof(int)));
            Assert.That("abc123", Is.Not.TypeOf<int>());
        }

        [Test, SetUICulture("en-US")]
        public void IsNotInstanceOfFails()
        {
            var expectedMessage =
                "  Expected: not instance of <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: Value does not fall within the expected range.>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.IsNotInstanceOf(typeof(Exception), new ArgumentException()));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test()]
        public void IsAssignableFrom()
        {
            var array10 = new int[10];

            Assert.That(array10, Is.AssignableFrom(typeof(int[])));
            Assert.That(array10, Is.AssignableFrom<int>());
        }

        [Test]
        public void IsAssignableFromFails()
        {
            var array10 = new int[10];
            var array2 = new int[2, 2];

            var expectedMessage =
                "  Expected: assignable from <System.Int32[,]>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(array10, Is.AssignableFrom(array2.GetType())));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }

        [Test()]
        public void IsNotAssignableFrom()
        {
            var array10 = new int[10];

            Assert.That(array10, Is.Not.AssignableFrom(typeof(int[,])));
            Assert.That(array10, Is.Not.AssignableFrom<int[,]>());
        }

        [Test]
        public void IsNotAssignableFromFails()
        {
            var array10 = new int[10];
            var array2 = new int[2];

            var expectedMessage =
                "  Expected: not assignable from <System.Int32[]>" + Environment.NewLine +
                "  But was:  <System.Int32[]>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(array10, Is.Not.AssignableFrom(array2.GetType())));
            Assert.That(ex?.Message, Is.EqualTo(expectedMessage));
        }
    }
}
