// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.DatapointFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    public class DatapointTests
    {
        private void RunTestOnFixture(Type fixtureType)
        {
            ITestResult result = TestBuilder.RunTestFixture(fixtureType);
            ResultSummary summary = new ResultSummary(result);
            Assert.That(summary.Passed, Is.EqualTo(2));
            Assert.That(summary.Inconclusive, Is.EqualTo(3));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void WorksOnField()
        {
            RunTestOnFixture(typeof(SquareRootTest_Field_Double));
        }

        [Test]
        public void WorksOnArray()
        {
            RunTestOnFixture(typeof(SquareRootTest_Field_ArrayOfDouble));
        }

        [Test]
        public void WorksOnPropertyReturningArray()
        {
            RunTestOnFixture(typeof(SquareRootTest_Property_ArrayOfDouble));
        }

        [Test]
        public void WorksOnMethodReturningArray()
        {
            RunTestOnFixture(typeof(SquareRootTest_Method_ArrayOfDouble));
        }

        [Test]
        public void WorksOnIEnumerableOfT()
        {
            RunTestOnFixture(typeof(SquareRootTest_Field_IEnumerableOfDouble));
        }

        [Test]
        public void WorksOnPropertyReturningIEnumerableOfT()
        {
            RunTestOnFixture(typeof(SquareRootTest_Property_IEnumerableOfDouble));
        }

        [Test]
        public void WorksOnMethodReturningIEnumerableOfT()
        {
            RunTestOnFixture(typeof(SquareRootTest_Method_IEnumerableOfDouble));
        }

        [Test]
        public void WorksOnEnumeratorReturningIEnumerableOfT()
        {
            RunTestOnFixture(typeof(SquareRootTest_Iterator_IEnumerableOfDouble));
        }

        [Test]
        public void WorksOnInheritedDataPoint()
        {
            RunTestOnFixture(typeof(DatapointCanBeInherited));
        }

        [Test]
        public void WorksOnInheritedDataPoints()
        {
            RunTestOnFixture(typeof(DatapointsCanBeInherited));
        }
    }
}