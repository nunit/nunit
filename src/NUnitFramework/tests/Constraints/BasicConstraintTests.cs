// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class NullConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new NullConstraint();
            ExpectedDescription = "null";
            StringRepresentation = "<null>";
        }

        private static object[] SuccessData = new object[] { null };
        private static object[] FailureData = new object[] { new object[] { "hello", "\"hello\"" } };
    }

    [TestFixture]
    public class TrueConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new TrueConstraint();
            ExpectedDescription = "True";
            StringRepresentation = "<true>";
        }

        private static object[] SuccessData = new object[] { true, 2+2==4 };
        private static object[] FailureData = new object[] { 
            new object[] { null, "null" }, new object[] { "hello", "\"hello\"" },
            new object[] { false, "False"}, new object[] { 2+2==5, "False" } };
    }

    [TestFixture]
    public class FalseConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new FalseConstraint();
            ExpectedDescription = "False";
            StringRepresentation = "<false>";
        }

        private static object[] SuccessData = new object[] { false, 2 + 2 == 5 };
        private static object[] FailureData = new object[] { 
            new TestCaseData( null, "null" ),
            new TestCaseData( "hello", "\"hello\"" ),
            new TestCaseData( true, "True" ),
            new TestCaseData( 2+2==4, "True" )};
    }

    [TestFixture]
    public class NaNConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new NaNConstraint();
            ExpectedDescription = "NaN";
            StringRepresentation = "<nan>";
        }

        private static object[] SuccessData = new object[] { double.NaN, float.NaN };
        private static object[] FailureData = new object[] { 
            new TestCaseData( null, "null" ),
            new TestCaseData( "hello", "\"hello\"" ),
            new TestCaseData( 42, "42" ), 
            new TestCaseData( double.PositiveInfinity, double.PositiveInfinity.ToString() ),
            new TestCaseData( double.NegativeInfinity, double.NegativeInfinity.ToString() ),
            new TestCaseData( float.PositiveInfinity, double.PositiveInfinity.ToString() ),
            new TestCaseData( float.NegativeInfinity, double.NegativeInfinity.ToString() ) };
    }
}
