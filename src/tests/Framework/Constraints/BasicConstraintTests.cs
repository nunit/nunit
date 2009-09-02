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
        
        object[] FailureData = new object[] { "hello" };

        string[] ActualValues = new string[] { "\"hello\"" };
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
        
        object[] FailureData = new object[] { null, "hello", false, 2+2==5 };

        string[] ActualValues = new string[] { "null", "\"hello\"", "False", "False" };
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

        object[] FailureData = new object[] { null, "hello", true, 2+2==4 };

        string[] ActualValues = new string[] { "null", "\"hello\"", "True", "True" };
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

        object[] FailureData = new object[] { null, "hello", 42, 
            double.PositiveInfinity, double.NegativeInfinity,
            float.PositiveInfinity, float.NegativeInfinity };

        string[] ActualValues = new string[] { "null", "\"hello\"", "42", 
            "Infinity", "-Infinity", "Infinity", "-Infinity" };
    }
}
