// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class SameAsTest : ConstraintTestBase
    {
        private static readonly object obj1 = new object();
        private static readonly object obj2 = new object();

        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SameAsConstraint(obj1);
            ExpectedDescription = "same as <System.Object>";
            StringRepresentation = "<sameas System.Object>";
        }

        private static object[] SuccessData = new object[] { obj1 };
        private static object[] FailureData = new object[] { 
            new TestCaseData( obj2, "<System.Object>" ),
            new TestCaseData( 3, "3" ),
            new TestCaseData( "Hello", "\"Hello\"" ) };
    }
}
