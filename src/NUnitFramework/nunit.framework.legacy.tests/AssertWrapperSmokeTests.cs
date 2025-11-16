// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class AssertWrapperSmokeTests
    {
        [Test]
        public void Core_Asserts_Work()
        {
            Assert.AreEqual(5, 5);
            Assert.AreNotEqual(5, 6);
            Assert.IsTrue(true);
            Assert.IsFalse(false);
            Assert.Null(null);
            Assert.NotNull(new object());
            Assert.IsEmpty(string.Empty);
            Assert.IsNotEmpty("x");
            Assert.IsNaN(double.NaN);
        }

        [Test]
        public void Numeric_Asserts_Work()
        {
            Assert.Zero(0);
            Assert.NotZero(1);
            Assert.Positive(1);
            Assert.Negative(-1);

            Assert.Zero(0.0);
            Assert.NotZero(1.23);
            Assert.Positive(2.5);
            Assert.Negative(-2.5);
        }

        [Test]
        public void Comparison_Asserts_Work()
        {
            Assert.Greater(2, 1);
            Assert.GreaterOrEqual(2, 2);
            Assert.Less(1, 2);
            Assert.LessOrEqual(2, 2);

            Assert.Greater(2.5m, 2.4m);
            Assert.LessOrEqual(2.0, 2.0);
        }

        private class Base
        {
        }

        private class Derived : Base
        {
        }

        [Test]
        public void Type_Asserts_Work()
        {
            var obj = new Derived();
            Assert.IsInstanceOf<Derived>(obj);
            Assert.IsNotInstanceOf<Base>(new object());
            Assert.IsAssignableFrom<Derived>(new Base());
            Assert.IsNotAssignableFrom<Base>(new Derived());
        }
    }
}
