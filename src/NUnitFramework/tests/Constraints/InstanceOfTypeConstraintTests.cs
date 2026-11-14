// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;
#if NET11_0_OR_GREATER
using NUnit.TestData;
#endif

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

#if NET11_0_OR_GREATER
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
        public void CanUseWithUnions<T>(T u)
        {
            Assert.That(u, Is.InstanceOf<T>());
            Assert.That(u, Is.InstanceOf<UnionMember>());
            Assert.That(u, Is.Not.InstanceOf<int>());

            Assert.That(u, Is.InstanceOf(typeof(T)));
            Assert.That(u, Is.InstanceOf(typeof(UnionMember)));
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
        public void CanUseWithUnionsOfDerivedTypes<T>(T u)
        {
            Assert.That(u, Is.InstanceOf<UnionMember>());
            Assert.That(u, Is.InstanceOf<DerivedUnionMember>());

            Assert.That(u, Is.InstanceOf(typeof(UnionMember)));
            Assert.That(u, Is.InstanceOf(typeof(DerivedUnionMember)));
        }
    }
#endif
}
