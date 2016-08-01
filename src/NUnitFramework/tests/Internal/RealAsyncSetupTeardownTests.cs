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

#if NET_4_0 || NET_4_5 || PORTABLE
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
#pragma warning disable 1998
    public class RealAsyncSetupTeardownTests
    {
        private object _initializedOnce;
        private object _initializedEveryTime;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _initializedOnce = new object();
        }

        [SetUp]
        public async Task Setup()
        {
            Assume.That(_initializedOnce, Is.Not.Null);
            _initializedEveryTime = new object();
        }


        [Test]
        public void TestCurrentFixtureInitialization()
        {
            Assert.That(_initializedOnce, Is.Not.Null);
            Assert.That(_initializedEveryTime, Is.Not.Null);

            _initializedEveryTime = null;
        }

        [TearDown]
        public async Task TearDown()
        {
            Assume.That(_initializedEveryTime, Is.Null);
            _initializedOnce = null;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            Assume.That(_initializedOnce, Is.Null);
        }

    }
#pragma warning restore 1998
}
#endif