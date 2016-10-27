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
    //    <test>xxxxxxx.xxx</test>
    //
    // Name filter
    //    <name>xxxxx</name>
    //
    // Category filter 
    //    <cat>cat1</cat>
    //    <cat>cat1,cat2,cat3</cat>
    //
    // Property filter
    //    <prop name="xxxx">value</prop>
    //
    // And Filter
    //    <and><filter>...</filter><filter>...</filter></and>
    //    <filter><filter>...</filter><filter>...</filter></filter>
    //
    // Or Filter
    //    <or><filter>...</filter><filter>...</filter></or>

    public abstract class TestFilterTests
    {
        public const string DUMMY_CLASS = "NUnit.Framework.Internal.Filters.TestFilterTests+DummyFixture";

        protected readonly TestSuite _dummyFixture = TestBuilder.MakeFixture(typeof(DummyFixture));
        protected readonly TestSuite _anotherFixture = TestBuilder.MakeFixture(typeof(AnotherFixture));
        protected readonly TestSuite _yetAnotherFixture = TestBuilder.MakeFixture(typeof(YetAnotherFixture));
        protected readonly TestSuite _fixtureWithMultipleTests = TestBuilder.MakeFixture (typeof (FixtureWithMultipleTests));
        protected readonly TestSuite _topLevelSuite = new TestSuite("MySuite");

        [OneTimeSetUp]
        public void SetUpSuite()
        {
            _topLevelSuite.Add(_dummyFixture);
            _topLevelSuite.Add(_anotherFixture);
            _topLevelSuite.Add(_yetAnotherFixture);
            _topLevelSuite.Add(_fixtureWithMultipleTests);
        }

        #region Fixtures Used by Tests

        [Category("Dummy"), Property("Priority", "High"), Author("Charlie Poole")]
        private class DummyFixture
        {
            [Test]
            public void Test() { }

        }

        [Category("Another"), Property("Priority", "Low"), Author("Fred Smith")]
        private class AnotherFixture
        {
            [Test]
            public void Test() { }
        }

        private class YetAnotherFixture
        {
        }

        private class FixtureWithMultipleTests
        {
            [Test]
            public void Test1 () {}

            [Test, Category ("Dummy")]
            public void Test2 () {}
        }
        #endregion
    }
}
