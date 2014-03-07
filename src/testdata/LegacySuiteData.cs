// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

#if !NUNITLITE
using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData.LegacySuiteData
{
    public class EmptySuite
    {
        [Suite]
        public static TestSuite MockSuite
        {
            get
            {
                TestSuite testSuite = new TestSuite("TestSuite");
                return testSuite;
            }
        }
    }

    public class LegacySuiteReturningFixtures
    {
        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                ArrayList suite = new ArrayList();
                suite.Add(new FixtureOne());
                suite.Add(new FixtureTwo());
                suite.Add(new FixtureThree());
                return suite;
            }
        }
    }

    public class LegacySuiteReturningTestSuite
    {
        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("MockSuite");
                suite.Add(new FixtureOne());
                suite.Add(new FixtureTwo());
                suite.Add(new FixtureThree());
                return suite;
            }
        }
    }

    public class FixtureOne
    {
        [Test]
        public void SomeTest() { }
    }

    public class FixtureTwo
    {
        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }

        [Test]
        public void Test3() { }

    }

    public class FixtureThree
    {
        [TestCase(42)]
        public void TestMethod(int arg)
        {
        }
    }

    public class LegacySuiteReturningTypes
    {
        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                ArrayList suite = new ArrayList();
                suite.Add(typeof(FixtureOne));
                suite.Add(typeof(FixtureTwo));
                suite.Add(typeof(FixtureThree));
                return suite;
            }
        }
    }

    public class LegacySuiteWithSetUpAndTearDown
    {
        static public int SetupCount = 0;
        static public int TeardownCount = 0;

        [Suite]
        public static TestSuite TheSuite
        {
            get { return new TestSuite("EmptySuite"); }
        }

        [OneTimeSetUp]
        public void SetUpMethod()
        {
            SetupCount++;
        }

        [OneTimeTearDown]
        public void TearDownMethod()
        {
            TeardownCount++;
        }
    }

    public class LegacySuiteWithInvalidPropertyType
    {
        [Suite]
        public static object TheSuite
        {
            get { return null; }
        }
    }
}
#endif
