// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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

        [Test]
        public void UseDifferentComparisonTypes_ThrowsException()
        {
            var subStringConstraint = (SubstringConstraint)TheConstraint;

            Assert.Multiple(() =>
            {
                // Invoke Using method before IgnoreCase
                Assert.That(() => subStringConstraint.Using(StringComparison.CurrentCulture).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => subStringConstraint.Using(StringComparison.InvariantCulture).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => subStringConstraint.Using(StringComparison.InvariantCultureIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => subStringConstraint.Using(StringComparison.Ordinal).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => subStringConstraint.Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());

                // Invoke IgnoreCase before Using method
                Assert.That(() => ((SubstringConstraint)subStringConstraint.IgnoreCase).Using(StringComparison.CurrentCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)subStringConstraint.IgnoreCase).Using(StringComparison.InvariantCulture),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)subStringConstraint.IgnoreCase).Using(StringComparison.InvariantCultureIgnoreCase),
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)subStringConstraint.IgnoreCase).Using(StringComparison.Ordinal).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
                Assert.That(() => ((SubstringConstraint)subStringConstraint.IgnoreCase).Using(StringComparison.OrdinalIgnoreCase).IgnoreCase,
                    Throws.TypeOf<InvalidOperationException>());
            });
        }

        [Test]
        public void UseSameComparisonTypes_DoesNotThrowException()
        {
            var subStringConstraint = new SubstringConstraint("hello");
            Assert.DoesNotThrow(() =>
            {
                var newConstraint = subStringConstraint.Using(StringComparison.CurrentCultureIgnoreCase).IgnoreCase;
            });

            Assert.DoesNotThrow(() =>
            {
                var newConstraint = subStringConstraint.IgnoreCase;
                newConstraint.Using(StringComparison.CurrentCultureIgnoreCase);
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
    }
}
