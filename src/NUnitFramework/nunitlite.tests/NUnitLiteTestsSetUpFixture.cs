// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitLite.Tests
{
    [SetUpFixture]
    public static class NUnitLiteTestsSetUpFixture
    {
        private static StackFilter _savedFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            _savedFilter = StackFilter.DefaultFilter;

            // We don't want to filter out the NUnitLite namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework.
            StackFilter.DefaultFilter = StackFilter.Create(@"^ +\w+:? NUnit\.Framework\.");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            StackFilter.DefaultFilter = _savedFilter;
        }
    }
}
