// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class InstanceOfTypeConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new InstanceOfTypeConstraint(typeof(D1));

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

        private class B { }

        private class D1 : B { }

        private class D2 : D1 { }
    }
}
