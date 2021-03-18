// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    using Builders;

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

            Assert.NotNull(suite, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(ImpliedFixture)));
        }
    }
}
