// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            public void DummyTest()
            {
            }
        }

        public sealed class TaskReturningTearDownFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningTearDownFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public void DummyTest()
            {
            }

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
            public void DummyTest()
            {
            }
        }

        public sealed class TaskReturningOneTimeTearDownFixture
        {
            private readonly AsyncTestDelegate _asyncUserCode;

            public TaskReturningOneTimeTearDownFixture(AsyncTestDelegate asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public void DummyTest()
            {
            }

            [OneTimeTearDown]
            public Task OneTimeTearDown() => _asyncUserCode.Invoke();
        }
    }
}
