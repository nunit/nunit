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
            _parameters.Add("IntParm", "5");
            _parameters.Add("StringParm", "five");
        }

        #region Add(string name, string value)

        [Test]
        public void Add_CountIsCorrect()
        {
            Assert.That(_parameters.Count, Is.EqualTo(2));
        }

        [Test]
        public void Add_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Add((string)null, "X"), Throws.ArgumentNullException);
        }

        #endregion

        #region this[string name]

        [Test]
        public void Index_ValuesAreCorrect()
        {
            Assert.That(_parameters["IntParm"], Is.EqualTo("5"));
            Assert.That(_parameters["StringParm"], Is.EqualTo("five"));
        }

        [Test]
        public void Index_MissingValuesAreNull()
        {
            Assert.Null(_parameters["ABC"]);
            Assert.Null(_parameters["XYZ"]);
        }

        [Test]
        public void Index_NullKeyThrowsException()
        {
            Assert.That(() => _parameters[null], Throws.ArgumentNullException);
        }

        #endregion

        #region Get(string name)

        [Test]
        public void Get_ValuesAreCorrect()
        {
            Assert.That(_parameters.Get("IntParm"), Is.EqualTo("5"));
            Assert.That(_parameters.Get("StringParm"), Is.EqualTo("five"));
        }

        [Test]
        public void Get_MissingValuesAreNull()
        {
            Assert.Null(_parameters.Get("ABC"));
            Assert.Null(_parameters.Get("XYZ"));
        }

        [Test]
        public void Get_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null), Throws.ArgumentNullException);
        }

        #endregion

        #region Get(string name, string defaultValue)

        [Test]
        public void GetWithDefault_ValuesAreCorrect()
        {
            Assert.That(_parameters.Get("StringParm", "JUNK"), Is.EqualTo("five"));
        }

        [Test]
        public void GetWithDefault_MissingValuesUseDefault()
        {
            Assert.That(_parameters.Get("XYZ", "JUNK"), Is.EqualTo("JUNK"));
        }

        [Test]
        public void GetWithDefault_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null, "JUNK"), Throws.ArgumentNullException);
        }

        #endregion

        #region GetInt(string name)

        [Test]
        public void GetInt_ValuesAreCorrect()
        {
            Assert.That(_parameters.GetInt("IntParm"), Is.EqualTo(5));
        }

        [Test]
        public void GetInt_MissingValuesUseZero()
        {
            Assert.That(_parameters.GetInt("ABC"), Is.EqualTo(0));
        }

        [Test]
        public void GetInt_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.GetInt(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GetInt_NonIntValueThrowsException()
        {
            Assert.That(() => _parameters.GetInt("StringParm"), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region GetInt(string name, int defaultValue)

        [Test]
        public void GetIntWithDefault_ValuesAreCorrect()
        {
            Assert.That(_parameters.GetInt("IntParm", 99), Is.EqualTo(5));
        }

        [Test]
        public void GetIntWithDefault_MissingValuesUseDefault()
        {
            Assert.That(_parameters.GetInt("ABC", 99), Is.EqualTo(99));
        }

        [Test]
        public void GetIntWithDefault_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.GetInt(null, 99), Throws.ArgumentNullException);
        }

        [Test]
        public void GetIntWithDefault_NonIntValueThrowsException()
        {
            Assert.That(() => _parameters.GetInt("StringParm", 99), Throws.TypeOf<FormatException>());
        }

        #endregion
    }
}
