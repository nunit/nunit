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
    public class ClassNameFilterTests : TestFilterTests
    {
        private const string DUMMY_CLASS = "NUnit.Framework.Internal.Filters.TestFilterTests+DummyFixture";
        private const string ANOTHER_CLASS = "NUnit.Framework.Internal.Filters.TestFilterTests+AnotherFixture";

        private readonly TestFilter filter = new ClassNameFilter(DUMMY_CLASS);
        private readonly TestFilter filter2 = new ClassNameFilter(new string[] { DUMMY_CLASS, ANOTHER_CLASS });

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void Match_SingleName()
        {
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void Pass_SingleName()
        {
            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(filter.Pass(_anotherFixture));
        }

        public void ExplicitMatch_SingleName()
        {
            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void Match_MultipleNames()
        {
            Assert.That(filter2.Match(_dummyFixture));
            Assert.That(filter2.Match(_anotherFixture));
            Assert.False(filter2.Match(_yetAnotherFixture));
        }

        [Test]
        public void Pass_MultipleNames()
        {
            Assert.That(filter2.Pass(_topLevelSuite));
            Assert.That(filter2.Pass(_dummyFixture));
            Assert.That(filter2.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter2.Pass(_anotherFixture));
            Assert.That(filter2.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter2.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatch_MultipleNames()
        {
            Assert.That(filter2.IsExplicitMatch(_dummyFixture));
            Assert.That(filter2.IsExplicitMatch(_anotherFixture));
            Assert.That(filter2.IsExplicitMatch(_dummyFixture.Tests[0]));
        }

        [Test]
        public void AddNames()
        {
            var filter = new ClassNameFilter();
            filter.Add(DUMMY_CLASS);
            filter.Add(ANOTHER_CLASS);

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><class>" + DUMMY_CLASS + "</class></filter>");

            Assert.That(filter, Is.TypeOf<ClassNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }
    }
}
