// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.Xml;
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.Engine.Api.Tests
{
    public class TestFilterTests
    {
        [Test]
        public void EmptyFilter()
        {
            TestFilter filter = TestFilter.Empty;
            Assert.That(filter.Text, Is.EqualTo("<filter/>"));
        }

        [Test]
        public void FilterWithOneTest()
        {
            string text = "<filter><tests><test>My.Test.Name</test></tests></filter>";
            TestFilter filter = new TestFilter(text);
            Assert.That(filter.Text, Is.EqualTo(text));
        }

        [Test]
        public void FilterWithThreeTests()
        {
            string text = "<filter><tests><test>My.First.Test</test><test>My.Second.Test</test><test>My.Third.Test</test></tests></filter>";
            TestFilter filter = new TestFilter(text);
            Assert.That(filter.Text, Is.EqualTo(text));
        }
    }
}
