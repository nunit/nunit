// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    [SetUpFixture]
    public static class NUnitTestsSetUpFixture
    {
        private static StackFilter _savedFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            _savedFilter = StackFilter.DefaultFilter;

            // We don't want to filter out the NUnit namespace as that is what we are testing here.
            // So we are not using a pattern for NUnit frames.
            StackFilter.DefaultFilter = StackFilter.Create(nunitStackPattern: null);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            StackFilter.DefaultFilter = _savedFilter;
        }
    }
}
