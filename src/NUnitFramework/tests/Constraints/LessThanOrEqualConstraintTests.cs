// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class LessThanOrEqualConstraintTests : ComparisonConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = comparisonConstraint = new LessThanOrEqualConstraint(5);
            expectedDescription = "less than or equal to 5";
            stringRepresentation = "<lessthanorequal 5>";
        }

        static object[] SuccessData = new object[] { 4, 5 };

        static object[] FailureData = new object[] { new object[] { 6, "6" } };

        [Test]
        public void CanCompareIComparables()
        {
            ClassWithIComparable expected = new ClassWithIComparable(42);
            ClassWithIComparable actual = new ClassWithIComparable(0);
            Assert.That(actual, Is.LessThanOrEqualTo(expected));
        }

        [Test]
        public void CanCompareIComparablesOfT()
        {
            ClassWithIComparableOfT expected = new ClassWithIComparableOfT(42);
            ClassWithIComparableOfT actual = new ClassWithIComparableOfT(0);
            Assert.That(actual, Is.LessThanOrEqualTo(expected));
        }

        [TestCase(4.0, 5.0, 0.05)]
        [TestCase(4.9999, 5.0, 0.05)]
        [TestCase(5.0001, 5.0, 0.05)]
        [TestCase(5.05, 5.0, 0.05)]
        [TestCase(190, 200, 5)]
        [TestCase(198, 200, 5)]
        [TestCase(202, 200, 5)]
        [TestCase(205, 200, 5)]
        public void SimpleTolerance(object actual, object expected, object tolerance)
        {
            Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance));
        }

        [TestCase(5.1, 5.0, 0.05)]
        [TestCase(206, 200, 5)]
        [TestCase(210, 200, 5)]
        public void SimpleTolerance_Failure(object actual, object expected, object tolerance)
        {
            var ex = Assert.Throws<AssertionException>(
                () => Assert.That(actual, Is.LessThanOrEqualTo(expected).Within(tolerance)),
                "Assertion should have failed");

            Assert.That(ex.Message, Contains.Substring("Expected: less than or equal to " + expected.ToString()));
        }
    }
}