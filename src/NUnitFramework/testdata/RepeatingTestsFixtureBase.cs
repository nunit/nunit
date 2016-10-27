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
using NUnit.Framework;

namespace NUnit.TestData.RepeatingTests
{
    [TestFixture]
    public class RepeatingTestsFixtureBase
    {
        private int fixtureSetupCount;
        private int fixtureTeardownCount;
        private int setupCount;
        private int teardownCount;
        protected int count;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            fixtureSetupCount++;
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            fixtureTeardownCount++;
        }

        [SetUp]
        public void SetUp()
        {
            setupCount++;
        }

        [TearDown]
        public void TearDown()
        {
            teardownCount++;
        }

        public int FixtureSetupCount
        {
            get { return fixtureSetupCount; }
        }
        public int FixtureTeardownCount
        {
            get { return fixtureTeardownCount; }
        }
        public int SetupCount
        {
            get { return setupCount; }
        }
        public int TeardownCount
        {
            get { return teardownCount; }
        }
        public int Count
        {
            get { return count; }
        }
    }
}
