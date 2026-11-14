// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;
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

#if NET11_0_OR_GREATER
    [TestFixture]
    public class ExactTypeConstraintTests_GeneralCases
    {
        private static TestCaseData[] GetUnionTestCases()
        {
            return
            [
                TestCaseData.Create<GeneratedUnionType>(new UnionMember()),
                TestCaseData.Create<CustomUnionType>(new UnionMember()),
            ];
        }

        [TestCaseSource(nameof(GetUnionTestCases))]
        public void CanUseWithUnions<T>(T u)
        {
            Assert.That(u, Is.TypeOf<T>());
            Assert.That(u, Is.TypeOf<UnionMember>());
            Assert.That(u, Is.Not.TypeOf<int>());

            Assert.That(u, Is.TypeOf(typeof(T)));
            Assert.That(u, Is.TypeOf(typeof(UnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(int)));
        }

        private static TestCaseData[] GetDerivedUnionTestCases()
        {
            return
            [
                TestCaseData.Create<GeneratedUnionType>(new DerivedUnionMember()),
                TestCaseData.Create<CustomUnionType>(new DerivedUnionMember()),
            ];
        }

        [TestCaseSource(nameof(GetDerivedUnionTestCases))]
        public void CanUseWithUnionsOfDerivedTypes<T>(T u)
        {
            Assert.That(u, Is.TypeOf<DerivedUnionMember>());
            Assert.That(u, Is.Not.TypeOf<UnionMember>());

            Assert.That(u, Is.TypeOf(typeof(DerivedUnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(UnionMember)));
        }
    }
#endif
}
