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

namespace NUnit.ConsoleRunner.Utilities.Tests
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
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo("<filter></filter>"));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            Assert.That(filter.Xml.ChildNodes.Count, Is.EqualTo(0));
        }

        [Test]
        public void OneTestSelected()
        {
            builder.Tests.Add("My.Test.Name");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><tests><test>My.Test.Name</test></tests></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            Assert.That(filter.Xml.SelectSingleNode("tests/test").InnerText, Is.EqualTo("My.Test.Name"));
        }

        [Test]
        public void ThreeTestsSelected()
        {
            builder.Tests.Add("My.First.Test");
            builder.Tests.Add("My.Second.Test");
            builder.Tests.Add("My.Third.Test");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><tests><test>My.First.Test</test><test>My.Second.Test</test><test>My.Third.Test</test></tests></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
            Assert.That(filter.Xml.Name, Is.EqualTo("filter"));
            XmlNodeList testNodes = filter.Xml.SelectNodes("tests/test");
            Assert.That(testNodes.Count, Is.EqualTo(3));
            Assert.That(testNodes[0].InnerText, Is.EqualTo("My.First.Test"));
            Assert.That(testNodes[1].InnerText, Is.EqualTo("My.Second.Test"));
            Assert.That(testNodes[2].InnerText, Is.EqualTo("My.Third.Test"));
        }

        [Test]
        public void OneCategoryIncluded()
        {
            builder.Include.Add("Dummy");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><cat>Dummy</cat></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void ThreeCategoriesIncluded()
        {
            builder.Include.Add("Dummy");
            builder.Include.Add("Another");
            builder.Include.Add("StillAnother");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><cat>Dummy,Another,StillAnother</cat></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void OneCategoryExcluded()
        {
            builder.Exclude.Add("Dummy");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><not><cat>Dummy</cat></not></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void ThreeCategoriesExcluded()
        {
            builder.Exclude.Add("Dummy");
            builder.Exclude.Add("Another");
            builder.Exclude.Add("StillAnother");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><not><cat>Dummy,Another,StillAnother</cat></not></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void OneTestAndOneCategory()
        {
            builder.Tests.Add("My.Test.Name");
            builder.Include.Add("Dummy");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><tests><test>My.Test.Name</test></tests><cat>Dummy</cat></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }

        [Test]
        public void TwoCategoriesIncludedAndOneExcluded()
        {
            builder.Include.Add("Dummy");
            builder.Include.Add("Another");
            builder.Exclude.Add("Slow");
            TestFilter filter = builder.GetFilter();

            string expectedText = "<filter><cat>Dummy,Another</cat><not><cat>Slow</cat></not></filter>";
            Assert.That(filter.Text, Is.EqualTo(expectedText));
        }
    }
}
