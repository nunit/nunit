// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class NullConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new NullConstraint();
            expectedDescription = "null";
            stringRepresentation = "<null>";
        }
        
        object[] SuccessData = new object[] { null };

        object[] FailureData = new object[] { new object[] { "hello", "\"hello\"" } };
    }

    [TestFixture]
    public class TrueConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new TrueConstraint();
            expectedDescription = "True";
            stringRepresentation = "<true>";
        }
        
        object[] SuccessData = new object[] { true, 2+2==4 };
        
        object[] FailureData = new object[] { 
            new object[] { null, "null" }, new object[] { "hello", "\"hello\"" },
            new object[] { false, "False"}, new object[] { 2+2==5, "False" } };
    }

    [TestFixture]
    public class FalseConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new FalseConstraint();
            expectedDescription = "False";
            stringRepresentation = "<false>";
        }

        object[] SuccessData = new object[] { false, 2 + 2 == 5 };

        object[] FailureData = new object[] { 
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
            theConstraint = new NaNConstraint();
            expectedDescription = "NaN";
            stringRepresentation = "<nan>";
        }
        
        object[] SuccessData = new object[] { double.NaN, float.NaN };

        object[] FailureData = new object[] { 
            new TestCaseData( null, "null" ),
            new TestCaseData( "hello", "\"hello\"" ),
            new TestCaseData( 42, "42" ), 
            new TestCaseData( double.PositiveInfinity, "Infinity" ),
            new TestCaseData( double.NegativeInfinity, "-Infinity" ),
            new TestCaseData( float.PositiveInfinity, "Infinity" ),
            new TestCaseData( float.NegativeInfinity, "-Infinity" ) };
    }
}
