// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace NUnit.TestData
{
    public sealed class ExecutionContextFlowFixture : IDisposable
    {
        // Constructor/Dispose and one-time setup/teardown may need to start flowing differently when we implement
        // instance-per-test-case.

#if !NETFRAMEWORK
        private static readonly AsyncLocal<bool> FromConstructor = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromOneTimeSetUp1 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromOneTimeSetUp2 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromSetUp1 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromSetUp2 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromTestMethod = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromTearDown1 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromTearDown2 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromOneTimeTearDown1 = new AsyncLocal<bool>();
        private static readonly AsyncLocal<bool> FromOneTimeTearDown2 = new AsyncLocal<bool>();
#endif

        public ExecutionContextFlowFixture()
        {
#if NETFRAMEWORK
            CallContext.LogicalSetData("Constructor", true);
#else
            FromConstructor.Value = true;
#endif
        }

        [OneTimeSetUp]
        public void OneTimeSetUp1()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);

            CallContext.LogicalSetData("OneTimeSetUp1", true);
#else
            Assert.That(FromConstructor.Value, Is.True);

            FromOneTimeSetUp1.Value = true;
#endif
        }

        [OneTimeSetUp]
        public void OneTimeSetUp2()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);

            CallContext.LogicalSetData("OneTimeSetUp2", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);

            FromOneTimeSetUp2.Value = true;
#endif
        }

        [SetUp]
        public void SetUp1()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);

            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("SetUp1", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);

            Assert.That(FromSetUp1.Value, Is.False);
            Assert.That(FromSetUp2.Value, Is.False);
            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            FromSetUp1.Value = true;
#endif
        }

        [SetUp]
        public void SetUp2()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.True);

            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("SetUp2", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);
            Assert.That(FromSetUp1.Value, Is.True);

            Assert.That(FromSetUp2.Value, Is.False);
            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            FromSetUp2.Value = true;
#endif
        }

        [Test]
        public void TestMethod([Range(1, 2)] int testCase)
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.True);

            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("TestMethod", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);
            Assert.That(FromSetUp1.Value, Is.True);
            Assert.That(FromSetUp2.Value, Is.True);

            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            FromTestMethod.Value = true;
#endif
        }

        [TearDown]
        public void TearDown2()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.True);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.True);

            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("TearDown2", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);
            Assert.That(FromSetUp1.Value, Is.True);
            Assert.That(FromSetUp2.Value, Is.True);
            Assert.That(FromTestMethod.Value, Is.True);
            Assert.That(FromTearDown1.Value, Is.True);

            Assert.That(FromTearDown2.Value, Is.False);

            FromTearDown2.Value = true;
#endif
        }

        [TearDown]
        public void TearDown1()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.True);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.True);

            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("TearDown1", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);
            Assert.That(FromSetUp1.Value, Is.True);
            Assert.That(FromSetUp2.Value, Is.True);
            Assert.That(FromTestMethod.Value, Is.True);

            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            FromTearDown1.Value = true;
#endif
        }

        [OneTimeTearDown]
        public void OneTimeTearDown2()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);

            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            Assert.That(CallContext.LogicalGetData("OneTimeTearDown1"), Is.True);

            CallContext.LogicalSetData("OneTimeTearDown2", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);

            Assert.That(FromSetUp1.Value, Is.False);
            Assert.That(FromSetUp2.Value, Is.False);
            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            Assert.That(FromOneTimeTearDown1.Value, Is.True);

            FromOneTimeTearDown2.Value = true;
#endif
        }

        [OneTimeTearDown]
        public void OneTimeTearDown1()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);

            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            CallContext.LogicalSetData("OneTimeTearDown1", true);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);

            Assert.That(FromSetUp1.Value, Is.False);
            Assert.That(FromSetUp2.Value, Is.False);
            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            FromOneTimeTearDown1.Value = true;
#endif
        }

        public void Dispose()
        {
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("Constructor"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeSetUp2"), Is.True);

            Assert.That(CallContext.LogicalGetData("SetUp1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("SetUp2"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TestMethod"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown1"), Is.Null);
            Assert.That(CallContext.LogicalGetData("TearDown2"), Is.Null);

            Assert.That(CallContext.LogicalGetData("OneTimeTearDown1"), Is.True);
            Assert.That(CallContext.LogicalGetData("OneTimeTearDown2"), Is.True);
#else
            Assert.That(FromConstructor.Value, Is.True);
            Assert.That(FromOneTimeSetUp1.Value, Is.True);
            Assert.That(FromOneTimeSetUp2.Value, Is.True);

            Assert.That(FromSetUp1.Value, Is.False);
            Assert.That(FromSetUp2.Value, Is.False);
            Assert.That(FromTestMethod.Value, Is.False);
            Assert.That(FromTearDown1.Value, Is.False);
            Assert.That(FromTearDown2.Value, Is.False);

            Assert.That(FromOneTimeTearDown1.Value, Is.True);
            Assert.That(FromOneTimeTearDown2.Value, Is.True);
#endif
        }
    }
}
