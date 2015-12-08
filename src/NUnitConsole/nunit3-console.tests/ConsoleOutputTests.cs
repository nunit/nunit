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

namespace NUnit.ConsoleRunner.Tests
{
    [TestFixture, Explicit]
    public class ConsoleOutputTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Console.WriteLine("OneTimeSetUp: Console.WriteLine()");
            Console.Error.WriteLine("OneTimeSetUp: Console.Error.WriteLine()");
            TestContext.WriteLine("OneTimeSetUp: TestContext.WriteLine()");

        }

        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp: Console.WriteLine()");
            Console.Error.WriteLine("SetUp: Console.Error.WriteLine()");
            TestContext.WriteLine("SetUp: TestContext.WriteLine()");
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown: Console.WriteLine()");
            Console.Error.WriteLine("TearDown: Console.Error.WriteLine()");
            TestContext.WriteLine("TearDown: TestContext.WriteLine()");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Console.WriteLine("OneTimeTearDown: Console.WriteLine()");
            Console.Error.WriteLine("OneTimeTearDown: Console.Error.WriteLine()");
            TestContext.WriteLine("OneTimeTearDown: TestContext.WriteLine()");
        }

        [Test]
        public void Test()
        {
            Console.WriteLine("Test: Console.WriteLine()");
            Console.Error.WriteLine("Test: Console.Error.WriteLine()");
            TestContext.WriteLine("Test: TestContext.WriteLine()");
        }
    }
}
