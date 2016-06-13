using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Framework.Tests
{
    public class TestParametersTests
    {
        private TestParameters _parameters;

        [SetUp]
        public void CreateTestParameters()
        {
            _parameters = new TestParameters();
        }

        [Test]
        public void InitiallyCountIsZero()
        {
            Assert.That(_parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void InitiallyValuesAreNull()
        {
            Assert.Null(_parameters["ABC"]);
            Assert.Null(_parameters["XYZ"]);
        }

        [Test]
        public void AddParameters_CountIsCorrect()
        {
            AddTwoItems();

            Assert.That(_parameters.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddParameters_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Add((string)null, "X"), Throws.ArgumentNullException);
        }

        [Test]
        public void IndexParameters_ValuesAreCorrect()
        {
            AddTwoItems();

            Assert.That(_parameters["Parm1"], Is.EqualTo("5"));
            Assert.That(_parameters["Parm2"], Is.EqualTo("five"));
        }

        [Test]
        public void IndexParameters_MissingValuesAreNull()
        {
            AddTwoItems();

            Assert.Null(_parameters["ABC"]);
            Assert.Null(_parameters["XYZ"]);
        }

        [Test]
        public void IndexParameters_NullKeyThrowsException()
        {
            Assert.That(() => _parameters[null], Throws.ArgumentNullException);
        }

        [Test]
        public void GetParameters_ValuesAreCorrect()
        {
            AddTwoItems();

            Assert.That(_parameters.Get("Parm1"), Is.EqualTo("5"));
            Assert.That(_parameters.Get("Parm2"), Is.EqualTo("five"));
        }

        [Test]
        public void GetParameters_MissingValuesAreNull()
        {
            AddTwoItems();

            Assert.Null(_parameters.Get("ABC"));
            Assert.Null(_parameters.Get("XYZ"));
        }

        [Test]
        public void GetParameters_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GetWithDefault_ValuesAreCorrect()
        {
            AddTwoItems();

            Assert.That(_parameters.Get("Parm1", 99), Is.EqualTo(5));
            Assert.That(_parameters.Get("Parm2", "JUNK"), Is.EqualTo("five"));
        }

        [Test]
        public void GetWithDefault_MissingValuesUseDefault()
        {
            AddTwoItems();

            Assert.That(_parameters.Get("ABC", 99), Is.EqualTo(99));
            Assert.That(_parameters.Get("XYZ", "JUNK"), Is.EqualTo("JUNK"));
        }

        private void AddTwoItems()
        {
            _parameters.Add("Parm1", "5");
            _parameters.Add("Parm2", "five");
        }
    }
}
