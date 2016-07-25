using System;

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

        #region Initial Conditions

        [Test]
        public void Initially_CountIsZero()
        {
            Assert.That(_parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void Initially_ValuesAreNull()
        {
            Assert.Null(_parameters["ABC"]);
        }

        [Test]
        public void Initially_ExistsIsFalse()
        {
            Assert.False(_parameters.Exists("ABC"));
        }

        [Test]
        public void Initially_NamesAreEmpty()
        {
            Assert.That(_parameters.Names.Count, Is.EqualTo(0));
        }

        #endregion

        #region Add

        [Test]
        public void Add_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Add((string)null, "X"), Throws.ArgumentNullException);
        }

        [Test]
        public void Add_IncrementsCount()
        {
            _parameters.Add("Parm1", "5");
            _parameters.Add("Parm2", "five");
            Assert.That(_parameters.Count, Is.EqualTo(2));
        }

        [Test]
        public void Add_AccumlatesNames()
        {
            _parameters.Add("Parm1", "5");
            _parameters.Add("Parm2", "five");
            Assert.That(_parameters.Names, Is.EquivalentTo(new[] { "Parm1", "Parm2" }));
        }

        [Test]
        public void Add_ParameterExists()
        {
            _parameters.Add("SomeParm", "5");
            Assert.That(_parameters.Exists("SomeParm"));
        }

        #endregion

        #region this[string name]

        [Test]
        public void Indexer_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "5");
            Assert.That(_parameters["SomeParm"], Is.EqualTo("5"));
        }

        [Test]
        public void Indexer_NullKeyThrowsException()
        {
            Assert.That(() => _parameters[null], Throws.ArgumentNullException);
        }

        #endregion

        #region Get

        [Test]
        public void Get_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "5");
            Assert.That(_parameters.Get("SomeParm"), Is.EqualTo("5"));
        }

        [Test]
        public void Get_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null), Throws.ArgumentNullException);
        }

        #endregion

        #region Get String

        [Test]
        public void GetString_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(_parameters.Get("SomeParm", "JUNK"), Is.EqualTo("five"));
        }

        [Test]
        public void GetString_MissingValue()
        {
            Assert.That(_parameters.Get("XYZ", "JUNK"), Is.EqualTo("JUNK"));
        }

        [Test]
        public void GetString_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null, "JUNK"), Throws.ArgumentNullException);
        }

        #endregion

        #region Get Int

        [Test]
        public void GetInt_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "5");
            Assert.That(_parameters.Get("SomeParm", 99), Is.EqualTo(5));
        }

        [Test]
        public void GetInt_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", 99), Is.EqualTo(99));
        }

        [Test]
        public void GetInt_NullKeyThrowsException()
        {
            Assert.That(() => _parameters.Get(null, 99), Throws.ArgumentNullException);
        }

        [Test]
        public void GetInt_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "Not an int");
            Assert.That(() => _parameters.Get("SomeParm", 99), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get Double

        [Test]
        public void GetDouble_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "5.2");
            Assert.That(_parameters.Get("SomeParm", 99.9), Is.EqualTo(5.2));
        }

        [Test]
        public void GetDouble_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", 99.9), Is.EqualTo(99.9));
        }

        [Test]
        public void GetDouble_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", 99.9), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void GetDouble_BadFormatThrowsException()
        {
            _parameters.Add("SomeParm", "1,234");
            Assert.That(() => _parameters.Get("SomeParm", 99.9), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get Decimal

        [Test]
        public void GetDecimal_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "1.234");
            Assert.That(_parameters.Get("SomeParm", 99.9M), Is.EqualTo(1.234M));
        }

        [Test]
        public void GetDecimal_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", 99.9M), Is.EqualTo(99.9M));
        }

        [Test]
        public void GetDecimal_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", 99.9M), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void GetDecimal_BadFormatThrowsException()
        {
            _parameters.Add("SomeParm", "1,234");
            Assert.That(() => _parameters.Get("SomeParm", 99.9M), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get Boolean

        [Test]
        public void GetBoolean_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "true");
            Assert.That(_parameters.Get("SomeParm", false), Is.EqualTo(true));
        }

        [Test]
        public void GetBoolean_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", false), Is.EqualTo(false));
        }

        [Test]
        public void GetBoolean_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", false), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get Char

        [Test]
        public void GetChar_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "Z");
            Assert.That(_parameters.Get("SomeParm", 'A'), Is.EqualTo('Z'));
        }

        [Test]
        public void GetChar_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", 'A'), Is.EqualTo('A'));
        }

        [Test]
        public void GetChar_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", 'A'), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get DateTime

        private static readonly DateTime DEFAULT_DATE_TIME = new DateTime(2000, 1, 1, 0, 0, 0);
        [Test]
        public void GetDateTime_ValueIsCorrect()
        {
            _parameters.Add("SomeParm", "1-January-2016");
            Assert.That(_parameters.Get("SomeParm", DEFAULT_DATE_TIME), Is.EqualTo(new DateTime(2016,1,1,0,0,0)));
        }

        [Test]
        public void GetDateTime_MissingValue()
        {
            Assert.That(_parameters.Get("ABC", DEFAULT_DATE_TIME), Is.EqualTo(DEFAULT_DATE_TIME));
        }

        [Test]
        public void GetDateTime_BadValueThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", DEFAULT_DATE_TIME), Throws.TypeOf<FormatException>());
        }

        #endregion

        #region Get Unsupported Type

        [Test]
        public void GetUnsupportedType_ThrowsException()
        {
            _parameters.Add("SomeParm", "five");
            Assert.That(() => _parameters.Get("SomeParm", new UnsupportedDefaultType()), Throws.TypeOf<InvalidCastException>());
        }

        #endregion

        #region

        class UnsupportedDefaultType
        {
        }

        #endregion
    }
}
