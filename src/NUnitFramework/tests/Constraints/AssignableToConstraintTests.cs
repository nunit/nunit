// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AssignableToConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new AssignableToConstraint(typeof(D1));
            ExpectedDescription = $"assignable to <{typeof(D1)}>";
            StringRepresentation = $"<assignableto {typeof(D1)}>";
        }

        private static object[] SuccessData = new object[] { new D1(), new D2() };
        private static object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<" + typeof(B).FullName + ">" ) };

        private class B { }

        private class D1 : B { }

        private class D2 : D1 { }
    }
}