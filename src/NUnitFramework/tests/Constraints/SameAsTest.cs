// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class SameAsTest : ConstraintTestBase
    {
        private static readonly object obj1 = new object();
        private static readonly object obj2 = new object();

        protected override Constraint TheConstraint { get; } = new SameAsConstraint(obj1);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "same as <System.Object>";
            StringRepresentation = "<sameas System.Object>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { obj1 };
        private static readonly object[] FailureData = new object[]
        { 
            new TestCaseData( obj2, "<System.Object>" ),
            new TestCaseData( 3, "3" ),
            new TestCaseData( "Hello", "\"Hello\"" )
        };
#pragma warning restore IDE0052 // Remove unread private members
    }
}
