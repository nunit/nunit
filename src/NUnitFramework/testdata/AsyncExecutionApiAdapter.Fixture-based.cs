// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class AsyncExecutionApiAdapter
    {
        public sealed class TaskReturningTestMethodFixture
        {
            private readonly Func<Task> _asyncUserCode;

            public TaskReturningTestMethodFixture(Func<Task> asyncUserCode)
            {
                _asyncUserCode = asyncUserCode;
            }

            [Test]
            public Task TestMethod() => _asyncUserCode.Invoke();
        }

        public sealed class TaskReturningSetUpFixture
        {
            private readonly Func<Task> _asyncUserCode;

            public TaskReturningSetUpFixture(Func<Task> asyncUserCode)
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
            private readonly Func<Task> _asyncUserCode;

            public TaskReturningTearDownFixture(Func<Task> asyncUserCode)
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
            private readonly Func<Task> _asyncUserCode;

            public TaskReturningOneTimeSetUpFixture(Func<Task> asyncUserCode)
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
            private readonly Func<Task> _asyncUserCode;

            public TaskReturningOneTimeTearDownFixture(Func<Task> asyncUserCode)
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
