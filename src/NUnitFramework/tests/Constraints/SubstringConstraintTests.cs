// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class SubstringConstraintTests : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new SubstringConstraint("hello");

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String containing \"hello\"";
            StringRepresentation = "<substring \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "hello", "hello there", "I said hello", "say hello to fred" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("HELLO", "\"HELLO\""),
            new TestCaseData("What the hell?", "\"What the hell?\""),
            new TestCaseData(string.Empty, "<string.Empty>"),
            new TestCaseData(null, "null")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [TestCase(" ss ", "ß", StringComparison.CurrentCulture)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCulture)]
        [TestCase(" ss ", "s", StringComparison.CurrentCulture)]
        [TestCase(" SS ", "s", StringComparison.CurrentCulture)]
        [TestCase(" ss ", "ß", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" SS ", "ß", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" ss ", "s", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" SS ", "s", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" ss ", "ß", StringComparison.InvariantCulture)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCulture)]
        [TestCase(" ss ", "s", StringComparison.InvariantCulture)]
        [TestCase(" SS ", "s", StringComparison.InvariantCulture)]
        [TestCase(" ss ", "ß", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" SS ", "ß", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" ss ", "s", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" SS ", "s", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" ss ", "ß", StringComparison.Ordinal)]
        [TestCase(" SS ", "ß", StringComparison.Ordinal)]
        [TestCase(" ss ", "s", StringComparison.Ordinal)]
        [TestCase(" SS ", "s", StringComparison.Ordinal)]
        [TestCase(" ss ", "ß", StringComparison.OrdinalIgnoreCase)]
        [TestCase(" SS ", "ß", StringComparison.OrdinalIgnoreCase)]
        [TestCase(" ss ", "s", StringComparison.OrdinalIgnoreCase)]
        [TestCase(" SS ", "s", StringComparison.OrdinalIgnoreCase)]
        public void SpecifyComparisonType(string actual, string expected, StringComparison comparison)
        {
            // Get platform-specific StringComparison behavior
            var shouldSucceed = actual.IndexOf(expected, comparison) != -1;

            Constraint constraint = Contains.Substring(expected).Using(comparison);
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [TestCase("ss ", "ß")]
        [TestCase("ß ", "ß")]
        [TestCase("ss ", "s")]
        [TestCase("SS ", "s")]
        public void SpecifyCultureInfo(string actual, string expected)
        {
            var cultureInfo = new CultureInfo("de-DE");
            var shouldSucceed = actual.StartsWith(expected, false, cultureInfo);

            Constraint constraint = Contains.Substring(expected).Using(cultureInfo);
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [Test]
        public void MultipleUsingModifiers_ThrowsException()
        {
            var substringConstraint = new SubstringConstraint("hello");
            Assert.Multiple(() =>
            {
                Assert.That(() => substringConstraint.Using(StringComparison.Ordinal).Using(CultureInfo.InvariantCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => substringConstraint.Using(CultureInfo.InvariantCulture).Using(StringComparison.Ordinal),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => substringConstraint.Using(StringComparison.Ordinal).Using(StringComparison.OrdinalIgnoreCase),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => substringConstraint.Using(CultureInfo.InvariantCulture).Using(CultureInfo.CurrentCulture),
                    Throws.TypeOf<InvalidOperationException>());
            });
        }

        [Test]
        public void TestSubstringUsingDoesContains()
        {
            Assert.That("Hello NUnit!", Does.Contain("Hello"));
            Assert.That("Hello NUnit!", Does.Not.Contain("World"));

            Assert.That(() => Assert.That("Hello", Does.Not.Contain(null)),
                Throws.InvalidOperationException.With.Message.Contains("Substring"));
        }

        [Test]
        public void TestSubstringUsingContainsSubstring()
        {
            Assert.That("Hello NUnit!", Contains.Substring("Hello"));
            Assert.That("Hello NUnit!", !Contains.Substring("World"));
        }
    }

    [TestFixture, SetCulture("en-US")]
    public class SubstringConstraintTestsIgnoringCase : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new SubstringConstraint("hello").IgnoreCase;

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String containing \"hello\", ignoring case";
            StringRepresentation = "<substring \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "Hello", "HellO there", "I said HELLO", "say hello to fred" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("What the hell?", "\"What the hell?\""),
            new TestCaseData(string.Empty, "<string.Empty>"),
            new TestCaseData(null, "null")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [TestCase("ss ", "ß")]
        [TestCase("ß ", "ß")]
        [TestCase("ss ", "s")]
        [TestCase("SS ", "s")]
        public void SpecifyCultureInfo(string actual, string expected)
        {
            var cultureInfo = new CultureInfo("de-DE");
            var shouldSucceed = cultureInfo.CompareInfo.IndexOf(actual, expected, CompareOptions.IgnoreCase) != -1;

            Constraint constraint1 = Contains.Substring(expected).Using(cultureInfo).IgnoreCase;
            Constraint constraint2 = Contains.Substring(expected).IgnoreCase.Using(cultureInfo);
            if (!shouldSucceed)
            {
                constraint1 = new NotConstraint(constraint1);
                constraint2 = new NotConstraint(constraint2);
            }

            Assert.That(actual, constraint1);
            Assert.That(actual, constraint2);
        }
    }
}
