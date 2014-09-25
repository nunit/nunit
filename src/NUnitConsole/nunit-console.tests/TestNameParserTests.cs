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
using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class TestNameParserTests
    {
        [TestCase("Test.Namespace.Fixture.Method")]
        [TestCase("Test.Namespace.Fixture.Method,")]
        [TestCase("  Test.Namespace.Fixture.Method  ")]
        [TestCase("  Test.Namespace.Fixture.Method  ,")]
        [TestCase("Test.Namespace.Fixture.Method()")]
        [TestCase("Test.Namespace.Fixture.Method(\"string,argument\")")]
        [TestCase("Test.Namespace.Fixture.Method(1,2,3)")]
        [TestCase("Test.Namespace.Fixture.Method<int,int>()")]
        [TestCase("Test.Namespace.Fixture.Method(\")\")")]
        public void SingleName(string name)
        {
            string[] names = TestNameParser.Parse(name);
            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(name.Trim(new char[] { ' ', ',' }), names[0]);
        }

        [TestCase("Test.Namespace.Fixture.Method1", "Test.Namespace.Fixture.Method2")]
        [TestCase("Test.Namespace.Fixture.Method1", "Test.Namespace.Fixture.Method2,")] // <= trailing comma
        [TestCase("Test.Namespace.Fixture.Method1(1,2)", "Test.Namespace.Fixture.Method2(3,4)")]
        [TestCase("Test.Namespace.Fixture.Method1(\"(\")", "Test.Namespace.Fixture.Method2(\"<\")")]
        public void TwoNames(string name1, string name2)
        {
            char[] delims = new char[] { ' ', ',' };
            string[] names = TestNameParser.Parse(name1 + "," + name2);
            Assert.AreEqual(2, names.Length);
            Assert.AreEqual(name1.Trim(delims), names[0]);
            Assert.AreEqual(name2.Trim(delims), names[1]);
        }
    }
}
