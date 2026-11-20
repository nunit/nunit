// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
        private static TestCaseData[] GetUnionTestCases()
        {
            return
            [
                TestCaseData.Create<GeneratedUnionType>(new UnionMember()),
                TestCaseData.Create<CustomUnionType>(new UnionMember()),
            ];
        }

        [TestCaseSource(nameof(GetUnionTestCases))]
        public void CanUseWithUnions<TUnion>(TUnion u)
        {
            Assert.That(u, Is.TypeOf<UnionMember>());
            Assert.That(u, Is.Not.TypeOf<TUnion>());
            Assert.That(u, Is.Not.TypeOf<int>());

            Assert.That(u, Is.TypeOf(typeof(UnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(TUnion)));
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
        public void CanUseWithUnionsOfDerivedTypes<TUnion>(TUnion u)
        {
            Assert.That(u, Is.TypeOf<DerivedUnionMember>());
            Assert.That(u, Is.Not.TypeOf<UnionMember>());

            Assert.That(u, Is.TypeOf(typeof(DerivedUnionMember)));
            Assert.That(u, Is.Not.TypeOf(typeof(UnionMember)));
        }

        [TestCaseSource(nameof(GetUnionTestCases))]
        public void FailureMessagesUseContainedUnionType<T>(T u)
        {
            AssertFailureMessageUsesContainedUnionType(Is.TypeOf<int>(), u, typeof(T));
            AssertFailureMessageUsesContainedUnionType(Is.TypeOf(typeof(int)), u, typeof(T));
        }

        private static void AssertFailureMessageUsesContainedUnionType(IConstraint constraint, object? actual, Type unionType)
        {
            var result = constraint.ApplyTo(actual);
            Assert.That(result.IsSuccess, Is.False);

            var writer = new TextMessageWriter();
            result.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.Contain(typeof(UnionMember).Name));
            Assert.That(writer.ToString(), Does.Not.Contain(unionType.Name));
        }
    }
}
