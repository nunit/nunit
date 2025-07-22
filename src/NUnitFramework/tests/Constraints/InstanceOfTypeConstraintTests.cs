// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

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

        public InstanceOfTypeConstraintTests(InstanceOfTypeConstraint constraint)
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
            new TestCaseData(new B(), "<" + typeof(B).FullName + ">")
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
}
