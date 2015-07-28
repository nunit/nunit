// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
    // Filter XML formats
    //
    // Empty Filter:
    //    <filter/>
    //
    // Id Filter:
    //    <id>1</id>
    //    <id>1,2,3</id>
    // 
    // TestName filter
    //    <tests><test>xxxxxxx.xxx</test><test>yyyyyyy.yyy</test></tests>
    //
    // Name filter
    //    <name>xxxxx</name>
    //
    // Category filter 
    //    <cat>cat1</cat>
    //    <cat>cat1,cat2,cat3</cat>
    //
    // Property filter
    //    <prop>name=value</prop>
    //
    // And Filter
    //    <and><filter>...</filter><filter>...</filter></and>
    //    <filter><filter>...</filter><filter>...</filter></filter>
    //
    // Or Filter
    //    <or><filter>...</filter><filter>...</filter></or>

    public abstract class TestFilterTests
    {
        protected static readonly TestSuite _dummyFixture = TestBuilder.MakeFixture(typeof(DummyFixture));
        protected static readonly TestSuite _anotherFixture = TestBuilder.MakeFixture(typeof(AnotherFixture));
        protected static readonly TestSuite _yetAnotherFixture = TestBuilder.MakeFixture(typeof(YetAnotherFixture));
        protected static readonly TestSuite _topLevelSuite = new TestSuite("MySuite");

        static TestFilterTests()
        {
            _topLevelSuite.Add(_dummyFixture);
            _topLevelSuite.Add(_anotherFixture);
            _topLevelSuite.Add(_yetAnotherFixture);
        }

        #region Fixtures Used by Tests

        [Category("Dummy")]
        private class DummyFixture
        {
            [Test]
            public void Test() { }

        }

        [Category("Dummy")]
        private class FixtureWithExplicitTest
        {
            [Test, Explicit]
            public void ExplicitTest() { }

        }

        [Category("Another")]
        private class AnotherFixture
        {
            [Test]
            public void Test() { }
        }

        private class YetAnotherFixture
        {
        }

        #endregion
    }
}
