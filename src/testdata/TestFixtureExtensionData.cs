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

#if !NUNITLITE
using System;
using NUnit.Framework;

// TODO: Figure out what this was for or delete it

namespace NUnit.TestData.TestFixtureExtensionData
{
    [TestFixture]
    public abstract class BaseTestFixture
    {
        public bool baseSetup = false;
        public bool baseTeardown = false;

        [SetUp]
        public void SetUp()
        { baseSetup = true; }

        [TearDown]
        public void TearDown()
        { baseTeardown = true; }
    }

    public class DerivedTestFixture : BaseTestFixture
    {
        [Test]
        public void Success()
        {
            Assert.IsTrue(true);
        }
    }

    public class SetUpDerivedTestFixture : BaseTestFixture
    {
        [SetUp]
        public void Init()
        {
            base.SetUp();
        }

        [Test]
        public void Success()
        {
            Assert.IsTrue(true);
        }
    }
}
#endif
