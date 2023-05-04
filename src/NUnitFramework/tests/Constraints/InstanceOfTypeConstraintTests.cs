// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class InstanceOfTypeConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new InstanceOfTypeConstraint(typeof(D1));
            ExpectedDescription = $"instance of <{typeof(D1)}>";
            StringRepresentation = $"<instanceof {typeof(D1)}>";
        }

        private static object[] SuccessData = new object[] { new D1(), new D2() };
        private static object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<" + typeof(B).FullName + ">" ) 
        };

        private class B { }

        private class D1 : B { }

        private class D2 : D1 { }
    }
}
