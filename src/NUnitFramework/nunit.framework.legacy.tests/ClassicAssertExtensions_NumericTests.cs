// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt


namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_NumericTests
    {
        [Test]
        public void Zero_Positive_Negative_Int()
        {
            Assert.Zero(0);
            Assert.NotZero(1);

            Assert.Positive(1);
            Assert.Negative(-1);
        }

        [Test]
        public void Zero_Positive_Negative_Double()
        {
            Assert.Zero(0.0);
            Assert.NotZero(0.1);

            Assert.Positive(0.1);
            Assert.Negative(-0.1);
        }
    }
}
