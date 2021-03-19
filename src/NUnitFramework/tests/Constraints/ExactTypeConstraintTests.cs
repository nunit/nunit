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
            ExpectedDescription = string.Format("<{0}>", typeof(D1));
            StringRepresentation = string.Format("<typeof {0}>", typeof(D1));
        }

        static object[] SuccessData = new object[] { new D1() };
        
        static object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<" + typeof(B).FullName + ">" ),
            new TestCaseData( new D2(), "<" + typeof(D2).FullName + ">" )
        };

        class B { }

        class D1 : B { }

        class D2 : D1 { }
    }
}
