// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

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

        public ExactTypeConstraintTests(ExactTypeConstraint constraint)
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
            new TestCaseData(new B(), "<" + typeof(B).FullName + ">"),
            new TestCaseData(new D2(), "<" + typeof(D2).FullName + ">")
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
