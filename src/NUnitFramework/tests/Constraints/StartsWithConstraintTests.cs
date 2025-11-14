// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class StartsWithConstraintTests : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new StartsWithConstraint("hello");

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String starting with \"hello\"";
            StringRepresentation = "<startswith \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "hello", "hello there" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("HELLO THERE", "\"HELLO THERE\""),
            new TestCaseData("I said hello", "\"I said hello\""),
            new TestCaseData("say hello to Fred", "\"say hello to Fred\""),
            new TestCaseData(string.Empty, "<string.Empty>"),
            new TestCaseData(null, "null")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void RespectsCulture()
        {
            var constraint = new StartsWithConstraint("r\u00E9sum\u00E9");

            var result = constraint.ApplyTo("re\u0301sume\u0301");
            Assert.That(result.IsSuccess, Is.True);
        }

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
            var shouldSucceed = actual.StartsWith(expected, comparison);

            Constraint constraint = Does.StartWith(expected).Using(comparison);
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [TestCase(" ss ", "ß", true)]
        [TestCase(" SS ", "ß", true)]
        [TestCase(" ss ", "s", true)]
        [TestCase(" SS ", "s", true)]
        [TestCase(" ss ", "ß", false)]
        [TestCase(" SS ", "ß", false)]
        [TestCase(" ss ", "s", false)]
        [TestCase(" SS ", "s", false)]
        public void SpecifyCultureInfo(string actual, string expected, bool ignoreCase)
        {
            var cultureInfo = new CultureInfo("fr-FR");
            // Get platform-specific StringComparison behavior
            var shouldSucceed = actual.StartsWith(expected, ignoreCase, cultureInfo);

            Constraint constraint = Does.StartWith(expected).Using(cultureInfo);
            if (ignoreCase)
                constraint = ((StartsWithConstraint)constraint).IgnoreCase;
            if (!shouldSucceed)
                constraint = new NotConstraint(constraint);

            Assert.That(actual, constraint);
        }

        [Test]
        public void UseDifferentComparisonTypes_ThrowsException()
        {
            var startsWithConstraint = (StartsWithConstraint)TheConstraint;

            Assert.Multiple(() =>
            {
                // Invoke Using method before IgnoreCase
                Assert.That(() => startsWithConstraint.Using(StringComparison.CurrentCulture).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => startsWithConstraint.Using(StringComparison.InvariantCulture).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => startsWithConstraint.Using(StringComparison.InvariantCultureIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => startsWithConstraint.Using(StringComparison.Ordinal).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => startsWithConstraint.Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());

                // Invoke IgnoreCase before Using method
                Assert.That(() => ((SubstringConstraint)startsWithConstraint.IgnoreCase).Using(StringComparison.CurrentCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)startsWithConstraint.IgnoreCase).Using(StringComparison.InvariantCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)startsWithConstraint.IgnoreCase).Using(StringComparison.InvariantCultureIgnoreCase),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)startsWithConstraint.IgnoreCase).Using(StringComparison.Ordinal).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)startsWithConstraint.IgnoreCase).Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
            });
        }

        [Test]
        public void UseSameComparisonTypes_DoesNotThrowException()
        {
            var startsWithConstraint = new StartsWithConstraint("hello");
            Assert.DoesNotThrow(() =>
            {
                var newConstraint = startsWithConstraint.Using(StringComparison.CurrentCultureIgnoreCase).IgnoreCase;
            });

            var stringConstraint = (StringConstraint)new StartsWithConstraint("hello");
            Assert.DoesNotThrow(() =>
            {
                var newConstraint = (StartsWithConstraint)stringConstraint.IgnoreCase;
                newConstraint = newConstraint.Using(StringComparison.CurrentCultureIgnoreCase);
            });
        }
    }

    [TestFixture]
    public class StartsWithConstraintTestsIgnoringCase : StringConstraintTests
    {
        protected override Constraint TheConstraint { get; } = new StartsWithConstraint("hello").IgnoreCase;

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "String starting with \"hello\", ignoring case";
            StringRepresentation = "<startswith \"hello\">";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { "Hello", "HELLO there" };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData("goodbye", "\"goodbye\""),
            new TestCaseData("What the hell?", "\"What the hell?\""),
            new TestCaseData("I said hello", "\"I said hello\""),
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
    }
}
