// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class AssignableFromConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new AssignableFromConstraint(typeof(D1));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = $"assignable from <{typeof(D1)}>";
            StringRepresentation = $"<assignablefrom {typeof(D1)}>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { new D1(), new B() };
        private static readonly object[] FailureData = new object[]
        {
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
