// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class WhiteSpaceContraintTests : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new WhiteSpaceConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "white-space";
            StringRepresentation = "<white-space>";
        }

        private static readonly object[] SuccessData = new object[]
        {
            string.Empty,
            " ",
            "\f",
            "\n",
            "\r",
            "\t",
            "\v",
        };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("Hello", "\"Hello\""),
            new TestCaseData("Hello World", "\"Hello World\""),
        };

        [TestCaseSource(nameof(SuccessData))]
        public void TestIsWhiteSpace(string text)
        {
            Assert.That(text, Is.WhiteSpace);
        }

        [TestCaseSource(nameof(FailureData))]
        public void TestIsNotWhiteSpace(string text, string message)
        {
            Assert.That(text, Is.Not.WhiteSpace, message);
        }
    }
}
