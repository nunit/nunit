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
    public class EmptyFilterTests : TestFilterTests
    {
        [Test]
        public void IsEmpty()
        {
            Assert.That(TestFilter.Empty.IsEmpty);
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml("<filter/>");

            Assert.That(filter.IsEmpty);
        }

        [Test]
        public void MatchesAnything()
        {
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void PassesAnything()
        {
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void MatchesNothihngExplicitly()
        {
            Assert.False(TestFilter.Empty.IsExplicitMatch(_dummyFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_anotherFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_yetAnotherFixture));
        }
    }
}
