// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AssignableFromConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new AssignableFromConstraint(typeof(D1));
            ExpectedDescription = string.Format("assignable from <{0}>", typeof(D1));
            StringRepresentation = string.Format("<assignablefrom {0}>", typeof(D1));
        }

        static object[] SuccessData = new object[] { new D1(), new B() };

        static object[] FailureData = new object[] { 
            new TestCaseData( new D2(), "<" + typeof(D2).FullName + ">" ) };

        class B { }

        class D1 : B { }

        class D2 : D1 { }
    }
}