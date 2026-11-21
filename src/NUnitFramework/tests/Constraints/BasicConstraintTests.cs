// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NullConstraintTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new NullConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "null";
            StringRepresentation = "<null>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object?[] SuccessData = new object?[] { null };
        private static readonly object[] FailureData = new object[] { new object[] { "hello", "\"hello\"" } };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void NonNullUnionCases()
        {
            UnionMember member = new();

            GeneratedUnionType generatedUnion = member;
            CustomUnionType customUnion = member;
            ExplicitIUnionType explicitUnion = member;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generatedUnion is not null);
#pragma warning disable NUnit2023 // Invalid NullConstraint usage
                Assert.That(generatedUnion, Is.Not.Null);
#pragma warning restore NUnit2023 // Invalid NullConstraint usage

                Assert.That(customUnion is not null);
                Assert.That(customUnion, Is.Not.Null);

                Assert.That(explicitUnion is not null);
                Assert.That(explicitUnion, Is.Not.Null);
            }
        }

        [Test]
        public void NullUnionCases()
        {
            GeneratedUnionType generatedUnion = default;
            ExplicitIUnionType explicitUnion = new(default(UnionMember));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generatedUnion is null);
#pragma warning disable NUnit2023 // Invalid NullConstraint usage
                Assert.That(generatedUnion, Is.Null);
#pragma warning restore NUnit2023 // Invalid NullConstraint usage

                Assert.That(explicitUnion is null);
                Assert.That(explicitUnion, Is.Null);
            }
        }
    }

    [TestFixture]
    public class TrueConstraintTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new TrueConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "True";
            StringRepresentation = "<true>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { true, 2 + 2 == 4 };
        private static readonly object[] FailureData = new object[]
        {
            new object?[] { null, "null" },
            new object[] { "hello", "\"hello\"" },
            new object[] { false, "False" },
            new object[] { 2 + 2 == 5, "False" }
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    [TestFixture]
    public class FalseConstraintTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new FalseConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "False";
            StringRepresentation = "<false>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { false, 2 + 2 == 5 };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(null, "null"),
            new TestCaseData("hello", "\"hello\""),
            new TestCaseData(true, "True"),
            new TestCaseData(2 + 2 == 4, "True")
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    [TestFixture]
    public class NaNConstraintTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new NaNConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "NaN";
            StringRepresentation = "<nan>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { double.NaN, float.NaN };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(null, "null"),
            new TestCaseData("hello", "\"hello\""),
            new TestCaseData(42, "42"),
            new TestCaseData(double.PositiveInfinity, double.PositiveInfinity.ToString()),
            new TestCaseData(double.NegativeInfinity, double.NegativeInfinity.ToString()),
            new TestCaseData(float.PositiveInfinity, double.PositiveInfinity.ToString()),
            new TestCaseData(float.NegativeInfinity, double.NegativeInfinity.ToString())
        };
#pragma warning restore IDE0052 // Remove unread private members
    }

    [TestFixture]
    public class MultipleOfConstraintTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new MultipleOfConstraint(3);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "MultipleOf(3)";
            StringRepresentation = "<multipleof>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 3, 9 };
        private static readonly object[] FailureData = new object[]
        {
            new object?[] { null, "null" },
            new object[] { "hello", "\"hello\"" },
            new object[] { false, "False" },
            new object[] { 2, "2" }
        };
#pragma warning restore IDE0052 // Remove unread private members
    }
}
