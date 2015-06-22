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

using System.Collections;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class UniqueItemsTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new UniqueItemsConstraint();
            stringRepresentation = "<uniqueitems>";
            expectedDescription = "all items unique";
        }

        static object[] SuccessData = new object[] { new int[] { 1, 3, 17, -2, 34 }, new object[0] };
        static object[] FailureData = new object[] { new object[] { new int[] { 1, 3, 17, 3, 34 }, "< 1, 3, 17, 3, 34 >" } };

        [Test]
        [TestCaseSource( "IgnoreCaseData" )]
        public void HonorsIgnoreCase( IEnumerable actual )
        {
            Assert.That( new UniqueItemsConstraint().IgnoreCase.ApplyTo( actual ).IsSuccess, Is.False, "{0} should be unique ignoring case", actual );
        }

        private static readonly object[] IgnoreCaseData =
        {
            new object[] {new SimpleObjectCollection("x", "y", "z", "Z")},
            new object[] {new[] {'A', 'B', 'C', 'c'}},
            new object[] {new[] {"a", "b", "c", "C"}}
        };
    }
}
