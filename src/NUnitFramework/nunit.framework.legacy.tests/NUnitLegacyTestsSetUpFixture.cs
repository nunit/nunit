// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
{
    [SetUpFixture]
    public static class NUnitLegacyTestsSetUpFixture
    {
        private static StackFilter _savedFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            _savedFilter = StackFilter.DefaultFilter;

            // We don't want to filter out the Legacy namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework, so we create a new StackFilter instance with a pattern that excludes Legacy.
            StackFilter.DefaultFilter = StackFilter.Create(@"^ +\w+:? NUnit\.Framework\.(?!Legacy\.)");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            StackFilter.DefaultFilter = _savedFilter;
        }
    }
}
