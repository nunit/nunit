// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtainingn
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
using NUnit.Framework;

namespace NUnit.Framework.TestHarness.Tests
{
    public class TestFilterBuilderTests
    {
        TestFilterBuilder builder;

        [SetUp]
        public void CreateBuilder()
        {
            this.builder = new TestFilterBuilder();
        }

        [Test]
        public void EmptyFilter()
        {
            string filterText = builder.GetFilterText();

            Assert.That(filterText, Is.EqualTo("<filter></filter>"));
        }

        [Test]
        public void FilterWithOneTest()
        {
            builder.Tests.Add("My.Test.Name");
            string filterText = builder.GetFilterText();

            string expectedText = "<filter><tests><test>My.Test.Name</test></tests></filter>";
            Assert.That(filterText, Is.EqualTo(expectedText));
        }

        [Test]
        public void FilterWithThreeTests()
        {
            builder.Tests.Add("My.First.Test");
            builder.Tests.Add("My.Second.Test");
            builder.Tests.Add("My.Third.Test");
            string filterText = builder.GetFilterText();

            string expectedText = "<filter><tests><test>My.First.Test</test><test>My.Second.Test</test><test>My.Third.Test</test></tests></filter>";
            Assert.That(filterText, Is.EqualTo(expectedText));
        }
    }
}
