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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class MethodNameFilterTests : TestFilterTests
    {
        private TestFilter _filter1;
        private TestFilter _filter2;

        [SetUp]
        public void SetUp()
        {
            _filter1 = new MethodNameFilter("Test");
            _filter2 = new MethodNameFilter(new string[] { "Test1", "Test2" });
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(_filter1.IsEmpty);
            Assert.False(_filter2.IsEmpty);
        }

        [Test]
        public void Match_SingleName()
        {
            Assert.That(_filter1.Match(_dummyFixture.Tests[0]));
            Assert.That(_filter1.Match(_anotherFixture.Tests[0]));
        }

        [Test]
        public void Pass_SingleName()
        {
            Assert.That(_filter1.Pass(_topLevelSuite));
            Assert.That(_filter1.Pass(_dummyFixture));
            Assert.That(_filter1.Pass(_dummyFixture.Tests[0]));

            Assert.That(_filter1.Pass(_anotherFixture));
            Assert.That(_filter1.Pass(_anotherFixture.Tests[0]));
            Assert.False(_filter1.Pass(_yetAnotherFixture));
        }

        public void ExplicitMatch_SingleName()
        {
            Assert.That(_filter1.IsExplicitMatch(_topLevelSuite));
            Assert.That(_filter1.IsExplicitMatch(_dummyFixture));
            Assert.False(_filter1.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(_filter1.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void Match_MultipleNames()
        {
            var _filter2 = new MethodNameFilter(new string[] { "Test1", "Test2" });

            Assert.That(_filter2.Match(_fixtureWithMultipleTests.Tests[0]));
            Assert.That(_filter2.Match(_fixtureWithMultipleTests.Tests[1]));
            Assert.False(_filter2.Match(_dummyFixture.Tests[0]));
        }

        [Test]
        public void Pass_MultipleNames()
        {
            Assert.That(_filter2.Pass(_topLevelSuite));
            Assert.False(_filter2.Pass(_dummyFixture));
            Assert.False(_filter2.Pass(_anotherFixture));

            Assert.That(_filter2.Pass(_fixtureWithMultipleTests));
            Assert.That(_filter2.Pass(_fixtureWithMultipleTests.Tests[0]));
            Assert.That(_filter2.Pass(_fixtureWithMultipleTests.Tests[1]));
        }

        [Test]
        public void ExplicitMatch_MultipleNames()
        {
            Assert.That(_filter2.IsExplicitMatch(_topLevelSuite));
            Assert.False(_filter2.IsExplicitMatch(_dummyFixture));
            Assert.False(_filter2.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.False(_filter2.IsExplicitMatch(_anotherFixture));
            Assert.False(_filter2.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.That(_filter2.IsExplicitMatch(_fixtureWithMultipleTests));
            Assert.That(_filter2.IsExplicitMatch(_fixtureWithMultipleTests.Tests[0]));
            Assert.That(_filter2.IsExplicitMatch(_fixtureWithMultipleTests.Tests[1]));
        }

        [Test]
        public void AddNames()
        {
            var filter = new MethodNameFilter();
            filter.Add("Test1");
            filter.Add("Test2");

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.False(filter.Pass(_dummyFixture));
            Assert.False(filter.Pass(_anotherFixture));

            Assert.That(filter.Pass(_fixtureWithMultipleTests));
            Assert.That(filter.Pass(_fixtureWithMultipleTests.Tests[0]));
            Assert.That(filter.Pass(_fixtureWithMultipleTests.Tests[1]));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><method>Test</method></filter>");

            Assert.That(filter, Is.TypeOf<MethodNameFilter>());
            Assert.That(filter.Match(_dummyFixture.Tests[0]));
            Assert.That(filter.Match(_anotherFixture.Tests[0]));
            Assert.False(filter.Match(_fixtureWithMultipleTests.Tests[0]));
        }
    }
}
