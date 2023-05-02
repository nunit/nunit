// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData.TestMethodSignatureFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class TestExpectedResult
    {
        [Test(ExpectedResult = 4)]
        public int CanExpectInt()
        {
            return 4;
        }

        [Test(ExpectedResult = 4)]
        public double CanExpectDouble()
        {
            return 4.0;
        }

        [Test(ExpectedResult = "Hello")]
        public string CanExpectString()
        {
            return "Hello";
        }

        [Test(ExpectedResult = null)]
        public object? ResultCanBeNull()
        {
            return null;
        }

        [Test]
        public void ExpectedResultNotAllowedOnParameterizedMethod()
        {
            TestAssert.IsNotRunnable(typeof(TestMethodSignatureFixture), nameof(TestMethodSignatureFixture.TestCasesWithReturnValueAndArgs_WithExpectedResult));
        }

        [Test(ExpectedResult = 42), Description("A description")]
        public int ExpectedResultDoesNotBlockApplyToTestAttributes()
        {
            Assert.That(TestContext.CurrentContext.Test.Properties.Get("Description"), Is.EqualTo("A description"));
            return 42;
        }
    }
}
