// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixtureSource(nameof(GetExactTypeConstraints))]
    public class ExactTypeConstraintTests : ConstraintTestBase
    {
        private static IEnumerable<TestFixtureData> GetExactTypeConstraints()
        {
            yield return new TestFixtureData(new ExactTypeConstraint<D1>())
                .SetArgDisplayNames("generic");
            yield return new TestFixtureData(new ExactTypeConstraint(typeof(D1)))
                .SetArgDisplayNames("non-generic");
        }

        public ExactTypeConstraintTests(Constraint constraint)
        {
            TheConstraint = constraint;
        }

        protected override Constraint TheConstraint { get; }

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = $"<{typeof(D1)}>";
            StringRepresentation = $"<typeof {typeof(D1)}>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { new D1() };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(new B(), $"<{typeof(B).FullName}>"),
            new TestCaseData(new D2(), $"<{typeof(D2).FullName}>")
        };
#pragma warning restore IDE0052 // Remove unread private members

        private class B
        {
        }

        private class D1 : B
        {
        }

        private class D2 : D1
        {
        }
    }

    [TestFixture]
    public class ExactTypeConstraintTests_GeneralCases
    {
        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.InitializedWithUnionMemberTestCases))]
        public void CanUseWithUnions<TUnion>(TUnion u)
        {
            Assert.That(u, Is.TypeOf<UnionMember>());
            Assert.That(u, Is.Not.TypeOf<TUnion>());
            Assert.That(u, Is.Not.TypeOf<int>());

            Assert.That(u, Is.TypeOf(typeof(UnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(TUnion)));
            Assert.That(u, Is.Not.TypeOf(typeof(int)));
        }

        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.InitializedWithDerivedUnionMemberTestCases))]
        public void CanUseWithUnionsOfDerivedTypes<TUnion>(TUnion u)
        {
            Assert.That(u, Is.TypeOf<DerivedUnionMember>());
            Assert.That(u, Is.Not.TypeOf<UnionMember>());

            Assert.That(u, Is.TypeOf(typeof(DerivedUnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(UnionMember)));
        }

        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.InitializedWithInt32TestCases))]
        public void CanUseWithUnionsValueType<TUnion>(TUnion u)
        {
            Assert.That(u, Is.Not.TypeOf<UnionMember>());
            Assert.That(u, Is.Not.TypeOf<TUnion>());
            Assert.That(u, Is.TypeOf<int>());

            Assert.That(u, Is.Not.TypeOf(typeof(UnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(TUnion)));
            Assert.That(u, Is.TypeOf(typeof(int)));
        }

        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.InitializedWithDerivedUnionMemberTestCases))]
        public void FailureMessagesUseContainedUnionType<TUnion>(TUnion u)
        {
            ConstraintAssert.MatchesFailureMessage(
                u,
                Is.TypeOf<int>(),
                Does.Contain(typeof(UnionMember).Name).And.Not.Contain(typeof(TUnion).Name));

            ConstraintAssert.MatchesFailureMessage(
                u,
                Is.TypeOf(typeof(int)),
                Does.Contain(typeof(UnionMember).Name).And.Not.Contain(typeof(TUnion).Name));
        }
    }
}
