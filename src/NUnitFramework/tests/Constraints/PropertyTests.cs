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

// TODO: Remove conditional code
using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public class PropertyExistsTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyExistsConstraint("Length");
            expectedDescription = "property Length";
            stringRepresentation = "<propertyexists Length>";
        }

        static object[] SuccessData = new object[] { new int[0], "hello", typeof(Array) };

        static object[] FailureData = new object[] { 
            new TestCaseData( 42, "<System.Int32>" ),
            new TestCaseData( new List<int>(), "<System.Collections.Generic.List`1[System.Int32]>" ),
            new TestCaseData( typeof(Int32), "<System.Int32>" ) };

        public void NullDataThrowsArgumentNullException()
        {
            object value = null;
            Assert.Throws<ArgumentNullException>(() => theConstraint.ApplyTo(value));
        }
    }

    public class PropertyTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyConstraint("Length", new EqualConstraint(5));
            expectedDescription = "property Length equal to 5";
            stringRepresentation = "<property Length <equal 5>>";
        }

        static object[] SuccessData = new object[] { new int[5], "hello" };

        static object[] FailureData = new object[] { 
            new TestCaseData( new int[3], "3" ),
            new TestCaseData( "goodbye", "7" ) };

        [Test]
        public void NullDataThrowsArgumentNullException()
        {
            object value = null;
            Assert.Throws<ArgumentNullException>(() => theConstraint.ApplyTo(value));
        }
        [Test]
        public void InvalidDataThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => theConstraint.ApplyTo(42));
        }

        [Test]
        public void PropertyEqualToValueWithTolerance()
        {
            Constraint c = new EqualConstraint(105m).Within(0.1m);
            Assert.That(c.Description, Is.EqualTo("105m +/- 0.1m"));

            c = new PropertyConstraint("D", new EqualConstraint(105m).Within(0.1m));
            Assert.That(c.Description, Is.EqualTo("property D equal to 105m +/- 0.1m"));
        }
    }
}
