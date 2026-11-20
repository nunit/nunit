// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixtureSource(nameof(GetInstanceOfTypeConstraints))]
    public class InstanceOfTypeConstraintTests : ConstraintTestBase
    {
        private static IEnumerable<TestFixtureData> GetInstanceOfTypeConstraints()
        {
            yield return new TestFixtureData(new InstanceOfTypeConstraint<D1>())
                .SetArgDisplayNames("generic");
            yield return new TestFixtureData(new InstanceOfTypeConstraint(typeof(D1)))
                .SetArgDisplayNames("non-generic");
        }

        public InstanceOfTypeConstraintTests(Constraint constraint)
        {
            TheConstraint = constraint;
        }

        protected override Constraint TheConstraint { get; }

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = $"instance of <{typeof(D1)}>";
            StringRepresentation = $"<instanceof {typeof(D1)}>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { new D1(), new D2() };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(new B(), $"<{typeof(B).FullName}>")
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
    public class InstanceOfTypeConstraintTests_GeneralCases
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
            Assert.That(u, Is.InstanceOf<UnionMember>());
            Assert.That(u, Is.Not.InstanceOf<TUnion>());
            Assert.That(u, Is.Not.InstanceOf<int>());

            Assert.That(u, Is.InstanceOf(typeof(UnionMember)));
            Assert.That(u, Is.Not.InstanceOf(typeof(TUnion)));
            Assert.That(u, Is.Not.InstanceOf(typeof(int)));
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
            Assert.That(u, Is.InstanceOf<UnionMember>());
            Assert.That(u, Is.InstanceOf<DerivedUnionMember>());

            Assert.That(u, Is.InstanceOf(typeof(UnionMember)));
            Assert.That(u, Is.InstanceOf(typeof(DerivedUnionMember)));
        }

        [TestCaseSource(nameof(GetUnionTestCases))]
        public void FailureMessagesUseContainedUnionType<T>(T u)
        {
            AssertFailureMessageUsesContainedUnionType(Is.InstanceOf<int>(), u, typeof(T));
            AssertFailureMessageUsesContainedUnionType(Is.InstanceOf(typeof(int)), u, typeof(T));
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
