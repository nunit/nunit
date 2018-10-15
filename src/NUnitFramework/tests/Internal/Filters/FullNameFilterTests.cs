// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture(TestFilterTests.DUMMY_CLASS, false)]
    [TestFixture("Dummy", true)]
    public class FullNameFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public FullNameFilterTests(string value, bool isRegex)
        {
            _filter = new FullNameFilter(value) { IsRegex = isRegex };
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(_filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(_dummyFixture));
            Assert.False(_filter.Match(_anotherFixture));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(_topLevelSuite));
            Assert.That(_filter.Pass(_dummyFixture));
            Assert.That(_filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(_filter.Pass(_anotherFixture));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            Assert.That(_filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(_filter.IsExplicitMatch(_dummyFixture));
            Assert.False(_filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(_filter.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void UTF8CharacterMatchFromXML()
        {
            TestFilter filter = TestFilter.FromXml("<filter><test>NUnit.Framework.Internal.Filters.FullNameFilterTests.UTF8CharacterMatchFromXMLTestMethod(&quot;ABC\u0001&quot;)</test></filter>");
            var test = TestBuilder.MakeTestFromMethod(GetType(), nameof(UTF8CharacterMatchFromXMLTestMethod));

            Assert.IsTrue(test.HasChildren);
            Assert.IsTrue(test.Tests[0].Arguments[0] is string);
            Assert.IsTrue((string)test.Tests[0].Arguments[0] == "ABC\x01");
            Assert.IsTrue(filter.Match(test.Tests[0]));
        }

        [Test]
        public void ArgumentsMatch()
        {
            TestFilter filter = TestFilter.FromXml("<filter><test>NUnit.Framework.Internal.Filters.FullNameFilterTests.ArgumentsMatchTestMethod(1)</test></filter>");
            var test = TestBuilder.MakeTestFromMethod(GetType(), nameof(ArgumentsMatchTestMethod));

            Assert.IsTrue(test.HasChildren);
            Assert.IsTrue(test.Tests[0].Arguments[0] is int);
            Assert.IsTrue((int)test.Tests[0].Arguments[0] == 1);
            Assert.IsTrue(filter.Match(test.Tests[0]));
        }

        [TestCase("ABC\x01", ExpectedResult = "ABC%01")]
        public string UTF8CharacterMatchFromXMLTestMethod(string s)
        {
            return string.Empty;
        }

        [Test]
        public void ArgumentsMatchTestMethod([Values(1, 2)] int s)
        {
        }
    }
}
