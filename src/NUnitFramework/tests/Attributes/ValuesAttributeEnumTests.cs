// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Attributes
{
    public enum EnumValues
    {
        One,
        Two,
        Three,
        Four,
        Five
    }

    [TestFixture]
    public class ValuesAttributeEnumTests
    {
        private int _countEnums;
        private int _countBools;
        private int _countNullableEnums;
        private int _countNullableBools;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _countEnums = 0;
            _countBools = 0;
            _countNullableEnums = 0;
            _countNullableBools = 0;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Assert.That(_countEnums, Is.EqualTo(5), "The TestEnumValues method should have been called 5 times");
            Assert.That(_countBools, Is.EqualTo(2), "The TestBoolValues method should have been called twice");
            Assert.That(_countNullableEnums, Is.EqualTo(6), "The TestNullableEnum method should have been called 6 times");
            Assert.That(_countNullableBools, Is.EqualTo(3), "The TestNullableBool method should have been called thrice");
        }

        [Test]
        public void TestEnumValues([Values]EnumValues value)
        {
            _countEnums++;
        }

        [Test]
        public void TestBoolValues([Values]bool value)
        {
            _countBools++;
        }

        [Test]
        public void TestNullableEnum([Values]EnumValues? enumValue)
        {
            /* runs with null and all enum values in no particular order */
            ++_countNullableEnums;
        }

        [Test]
        public void TestNullableBool([Values] bool? testInput)
        {
            /* runs with null, true, false in no particular order */
            ++_countNullableBools;
        }
    }
}
