﻿// ***********************************************************************
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

using System.Xml;
using NUnit.Framework;

namespace NUnit.Engine.Tests.Api
{
    public class TestFilterTests
    {
        [Test]
        public void EmptyFilter()
        {
            var filter = TestFilter.Empty;
            Assert.That(filter.Text, Is.EqualTo("<filter/>"));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            Assert.That(filter.Xml.ChildNodes.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterWithOneTest()
        {
            var text = "<filter><tests><test>My.Test.Name</test></tests></filter>";
            var filter = new TestFilter(text);
            Assert.That(filter.Text, Is.EqualTo(text));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            Assert.That(filter.Xml.SelectSingleNode("tests/test").InnerText, Is.EqualTo("My.Test.Name"));
        }

        [Test]
        public void FilterWithThreeTests()
        {
            var text = "<filter><tests><test>My.First.Test</test><test>My.Second.Test</test><test>My.Third.Test</test></tests></filter>";
            var filter = new TestFilter(text);
            Assert.That(filter.Text, Is.EqualTo(text));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            XmlNodeList testNodes = filter.Xml.SelectNodes("tests/test");
            Assert.That(testNodes.Count, Is.EqualTo(3));
            Assert.That(testNodes[0].InnerText, Is.EqualTo("My.First.Test"));
            Assert.That(testNodes[1].InnerText, Is.EqualTo("My.Second.Test"));
            Assert.That(testNodes[2].InnerText, Is.EqualTo("My.Third.Test"));
        }
    }
}
