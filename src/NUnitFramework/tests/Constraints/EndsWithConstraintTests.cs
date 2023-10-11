// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
    }
}
