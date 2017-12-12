// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

#if PARALLEL
using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData.ParallelExecutionData
{
    [SetUpFixture]
    public class SetUpFixture1
    {
        [OneTimeSetUpAttribute]
        public void RunOneTimeSetUp()
        {
            Thread.Sleep(100);
        }

        [OneTimeTearDown]
        public void RunOneTimeTearDown()
        {
            Thread.Sleep(100);
        }
    }

    [SetUpFixture]
    public class SetUpFixture2
    {
        [OneTimeSetUpAttribute]
        public void RunOneTimeSetUp()
        {
            Thread.Sleep(100);
        }

        [OneTimeTearDown]
        public void RunOneTimeTearDown()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture1
    {
        [Test]
        public void TestFixture1_Test()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture2
    {
        [Test]
        public void TestFixture2_Test()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture3
    {
        [Test]
        public void TestFixture3_Test()
        {
            Thread.Sleep(100);
        }
    }

#if APARTMENT_STATE
    [Apartment(ApartmentState.STA)]
    public class STAFixture
    {
        [Test]
        public void STAFixture_Test()
        {
            Thread.Sleep(100);
        }
    }
#endif

    public class TestFixtureWithParallelParameterizedTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Parallelizable(ParallelScope.Children)]
        public void ParameterizedTest(int i)
        {
            Thread.Sleep(100);
        }
    }
}
#endif
