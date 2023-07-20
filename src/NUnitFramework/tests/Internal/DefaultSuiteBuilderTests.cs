// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
    }
}
