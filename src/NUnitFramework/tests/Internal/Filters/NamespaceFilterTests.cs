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

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture("NUnit.Framework.Internal.Filters", false, true)]
    [TestFixture("NUnit.Framework.*", true, true)]
    [TestFixture("NUnit.Framework", false, false)]
    public class NamespaceFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;
        private readonly bool _expected;

        public NamespaceFilterTests(string value, bool isRegex, bool expected)
        {
            _filter = new NamespaceFilter(value) { IsRegex = isRegex };
            _expected = expected;
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(_filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(_dummyFixture.Tests[0]), Is.EqualTo(_expected));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(_nestingFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_nestedFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_emptyNestedFixture), Is.EqualTo(_expected));

            Assert.That(_filter.Pass(_topLevelSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_dummyFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_dummyFixture.Tests[0]), Is.EqualTo(_expected));
        }

    }
}
