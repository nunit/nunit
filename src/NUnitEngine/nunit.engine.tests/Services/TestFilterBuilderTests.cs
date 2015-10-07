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

namespace NUnit.Engine.Services.Tests
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
        }

        [Test]
        public void OneTestSelected()
        {
            builder.AddTest("My.Test.Name");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><test>My.Test.Name</test></filter>"));
        }

        [Test]
        public void ThreeTestsSelected()
        {
            builder.AddTest("My.First.Test");
            builder.AddTest("My.Second.Test");
            builder.AddTest("My.Third.Test");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><or><test>My.First.Test</test><test>My.Second.Test</test><test>My.Third.Test</test></or></filter>"));
        }

        [Test]
        public void OneCategoryIncluded()
        {
            builder.IncludeCategory("Dummy");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo("<filter><cat>Dummy</cat></filter>"));
        }

        [Test]
        public void ThreeCategoriesIncluded()
        {
            builder.IncludeCategory("Dummy");
            builder.IncludeCategory("Another");
            builder.IncludeCategory("StillAnother");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><cat>Dummy,Another,StillAnother</cat></filter>"));
        }

        [Test]
        public void OneCategoryExcluded()
        {
            builder.ExcludeCategory("Dummy");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo("<filter><not><cat>Dummy</cat></not></filter>"));
        }

        [Test]
        public void ThreeCategoriesExcluded()
        {
            builder.ExcludeCategory("Dummy");
            builder.ExcludeCategory("Another");
            builder.ExcludeCategory("StillAnother");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><not><cat>Dummy,Another,StillAnother</cat></not></filter>"));
        }

        [Test]
        public void OneTestAndOneCategory()
        {
            builder.AddTest("My.Test.Name");
            builder.IncludeCategory("Dummy");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><test>My.Test.Name</test><cat>Dummy</cat></filter>"));
        }

        [Test]
        public void TwoCategoriesIncludedAndOneExcluded()
        {
            builder.IncludeCategory("Dummy");
            builder.IncludeCategory("Another");
            builder.ExcludeCategory("Slow");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><cat>Dummy,Another</cat><not><cat>Slow</cat></not></filter>"));
        }

        [Test]
        public void WhereClause()
        {
            builder.SelectWhere("cat==Dummy");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo("<filter><cat>Dummy</cat></filter>"));
        }

        [Test]
        public void OneTestOneCategoryAndWhereClause()
        {
            builder.AddTest("My.Test.Name");
            builder.IncludeCategory("Dummy");
            builder.SelectWhere("cat != Slow");
            TestFilter filter = builder.GetFilter();

            Assert.That(filter.Text, Is.EqualTo(
                "<filter><test>My.Test.Name</test><cat>Dummy</cat><not><cat>Slow</cat></not></filter>"));
        }
    }
}
