// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ExactTypeConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ExactTypeConstraint(typeof(D1));
            ExpectedDescription = $"<{typeof(D1)}>";
            StringRepresentation = $"<typeof {typeof(D1)}>";
        }

        private static object[] SuccessData = new object[] { new D1() };
        private static object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<" + typeof(B).FullName + ">" ),
            new TestCaseData( new D2(), "<" + typeof(D2).FullName + ">" )
        };

        private class B { }

        private class D1 : B { }

        private class D2 : D1 { }
    }
}
