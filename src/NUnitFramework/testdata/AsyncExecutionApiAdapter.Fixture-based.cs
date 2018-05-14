// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if ASYNC
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class AsyncExecutionApiAdapter
    {
        public sealed class TaskReturningTestMethodFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningTestMethodFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public Task TestMethod() => _asyncUserCode.Invoke();
        }

        public sealed class TaskReturningSetUpFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningSetUpFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [SetUp]
            public Task SetUp() => _asyncUserCode.Invoke();

            [Test]
            public void DummyTest() { }
        }

        public sealed class TaskReturningTearDownFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningTearDownFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public void DummyTest() { }

            [TearDown]
            public Task TearDown() => _asyncUserCode.Invoke();
        }

        public sealed class TaskReturningOneTimeSetUpFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningOneTimeSetUpFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [OneTimeSetUp]
            public Task OneTimeSetUp() => _asyncUserCode.Invoke();

            [Test]
            public void DummyTest() { }
        }

        public sealed class TaskReturningOneTimeTearDownFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningOneTimeTearDownFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public void DummyTest() { }

            [OneTimeTearDown]
            public Task OneTimeTearDown() => _asyncUserCode.Invoke();
        }
    }
}
#endif
