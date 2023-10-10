// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Internal
{
    // NOTE: Because this fixture tests the IImplyFixture interface, the attribute must be
    // present. Otherwise, if implied fixture recognition did not work, the tests would not run.
    [TestFixture]
    public static class DefaultSuiteBuilderTests
    {
        [Test]
        public static void CanBuildFromReturnsTrueForImpliedFixture()
        {
            Assert.That(new DefaultSuiteBuilder().CanBuildFrom(new TypeWrapper(typeof(ImpliedFixture))));
        }

        [Test]
        public static void BuildFromReturnsImpliedFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(typeof(ImpliedFixture))) as TestFixture;

            Assert.That(suite, Is.Not.Null, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(ImpliedFixture)));
        }

        [Test]
        public static void BuildFromAbstractBaseClassWithTestsReturnsFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(typeof(AbstractBaseClass))) as TestFixture;

            Assert.That(suite, Is.Not.Null, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(AbstractBaseClass)));
            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));

            Assert.That(suite.TestCaseCount, Is.EqualTo(1));

            ITest? testInDerivedClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.VirtualTestInBaseClass));
            Assert.That(testInDerivedClass, Is.Not.Null);
            Assert.That(testInDerivedClass.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public static void BuildFromBaseClassWithTestsReturnsFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(typeof(BaseClassWithTests))) as TestFixture;

            Assert.That(suite, Is.Not.Null, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(BaseClassWithTests)));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

            Assert.That(suite.TestCaseCount, Is.EqualTo(2));

            ITest? testInBaseClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.TestInBaseClass));
            Assert.That(testInBaseClass, Is.Not.Null);
            Assert.That(testInBaseClass.RunState, Is.EqualTo(RunState.Runnable));

            ITest? testInDerivedClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.VirtualTestInBaseClass));
            Assert.That(testInDerivedClass, Is.Not.Null);
            Assert.That(testInDerivedClass.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public static void BuildFromDerivedClassWithTestsReturnsFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(typeof(DerivedClassWithTestsInBaseClass))) as TestFixture;

            Assert.That(suite, Is.Not.Null, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(DerivedClassWithTestsInBaseClass)));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

            Assert.That(suite.TestCaseCount, Is.EqualTo(2));

            ITest? testInBaseClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.TestInBaseClass));
            Assert.That(testInBaseClass, Is.Not.Null);
            Assert.That(testInBaseClass.RunState, Is.EqualTo(RunState.Runnable));

            ITest? testInDerivedClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.VirtualTestInBaseClass));
            Assert.That(testInDerivedClass, Is.Not.Null);
            Assert.That(testInDerivedClass.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public static void BuildFromDerivedClassWithIgnoredTestsReturnsFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(new TypeWrapper(typeof(DerivedClassWithIgnoredTestsInBaseClass))) as TestFixture;

            Assert.That(suite, Is.Not.Null, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(DerivedClassWithIgnoredTestsInBaseClass)));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

            Assert.That(suite.TestCaseCount, Is.EqualTo(2));

            ITest? testInBaseClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.TestInBaseClass));
            Assert.That(testInBaseClass, Is.Not.Null);
            Assert.That(testInBaseClass.RunState, Is.EqualTo(RunState.Runnable));

            ITest? testInDerivedClass = suite.Tests.SingleOrDefault(x => x.Name == nameof(BaseClassWithTests.VirtualTestInBaseClass));
            Assert.That(testInDerivedClass, Is.Not.Null);
            Assert.That(testInDerivedClass.RunState, Is.EqualTo(RunState.Ignored));
        }
    }
}
