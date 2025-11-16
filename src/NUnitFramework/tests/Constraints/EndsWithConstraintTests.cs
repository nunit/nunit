// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class EndsWithConstraintTests : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new EndsWithConstraint("hello");

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String ending with \"hello\"";
            StringRepresentation = "<endswith \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "hello", "I said hello" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("hello there", "\"hello there\""),
            new TestCaseData("say hello to Fred", "\"say hello to Fred\""),
            new TestCaseData(string.Empty, "<string.Empty>"),
            new TestCaseData(null, "null")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void RespectsCulture()
        {
            var constraint = new EndsWithConstraint("r\u00E9sum\u00E9");

            var result = constraint.ApplyTo("re\u0301sume\u0301");
            Assert.That(result.IsSuccess, Is.True);
        }

        [TestCase(" ss", "ß", StringComparison.CurrentCulture)]
        [TestCase(" ss", "s", StringComparison.CurrentCulture)]
        [TestCase(" SS", "s", StringComparison.CurrentCulture)]
        [TestCase(" ss", "ß", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" ss", "s", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" SS", "s", StringComparison.CurrentCultureIgnoreCase)]
        [TestCase(" ss", "ß", StringComparison.InvariantCulture)]
        [TestCase(" ss", "s", StringComparison.InvariantCulture)]
        [TestCase(" SS", "s", StringComparison.InvariantCulture)]
        [TestCase(" ss", "ß", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" ss", "s", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" SS", "s", StringComparison.InvariantCultureIgnoreCase)]
        [TestCase(" ss", "ß", StringComparison.Ordinal)]
        [TestCase(" ss", "s", StringComparison.Ordinal)]
        [TestCase(" SS", "s", StringComparison.Ordinal)]
        [TestCase(" ss", "ß", StringComparison.OrdinalIgnoreCase)]
        [TestCase(" ss", "s", StringComparison.OrdinalIgnoreCase)]
        [TestCase(" SS", "s", StringComparison.OrdinalIgnoreCase)]
        public void SpecifyComparisonType(string actual, string expected, StringComparison comparison)
        {
            // Get platform-specific StringComparison behavior
            var shouldSucceed = actual.EndsWith(expected, comparison);

            Constraint constraint = Does.EndWith(expected).Using(comparison);
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [TestCase(" ss", "ß")]
        [TestCase(" ß", "ß")]
        [TestCase(" ss", "s")]
        [TestCase(" SS", "s")]
        public void SpecifyCultureInfo(string actual, string expected)
        {
            var cultureInfo = new CultureInfo("de-DE");
            // Get platform-specific StringComparison behavior
            var shouldSucceed = actual.EndsWith(expected, false, cultureInfo);

            Constraint constraint = Does.EndWith(expected).Using(cultureInfo);
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [Test]
        public void MultipleUsingModifiers_ThrowsException()
        {
            var endsWithConstraint = new EndsWithConstraint("hello");
            Assert.Multiple(() =>
            {
                Assert.That(() => endsWithConstraint.Using(StringComparison.Ordinal).Using(CultureInfo.InvariantCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => endsWithConstraint.Using(CultureInfo.InvariantCulture).Using(StringComparison.Ordinal),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => endsWithConstraint.Using(StringComparison.Ordinal).Using(StringComparison.OrdinalIgnoreCase),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => endsWithConstraint.Using(CultureInfo.InvariantCulture).Using(CultureInfo.CurrentCulture),
                    Throws.TypeOf<InvalidOperationException>());
            });
        }
    }

    [TestFixture]
    public class EndsWithConstraintTestsIgnoringCase : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new EndsWithConstraint("hello").IgnoreCase;

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String ending with \"hello\", ignoring case";
            StringRepresentation = "<endswith \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "HELLO", "I said Hello" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("What the hell?", "\"What the hell?\""),
            new TestCaseData("hello there", "\"hello there\""),
            new TestCaseData("say hello to Fred", "\"say hello to Fred\""),
            new TestCaseData(string.Empty, "<string.Empty>"),
            new TestCaseData(null, "null")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void RespectsCulture()
        {
            var constraint = new EndsWithConstraint("r\u00E9sum\u00E9").IgnoreCase;

            var result = constraint.ApplyTo("re\u0301sume\u0301");
            Assert.That(result.IsSuccess, Is.True);
        }

        [TestCase("ss", "ß")]
        [TestCase("ß", "ß")]
        [TestCase("ss", "s")]
        [TestCase("SS", "s")]
        public void SpecifyCultureInfo(string actual, string expected)
        {
            var cultureInfo = new CultureInfo("de-DE");
            // Get platform-specific StringComparison behavior
            var shouldSucceed = actual.EndsWith(expected, true, cultureInfo);

            Constraint constraint1 = Does.EndWith(expected).Using(cultureInfo).IgnoreCase;
            Constraint constraint2 = Does.EndWith(expected).IgnoreCase.Using(cultureInfo);

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
