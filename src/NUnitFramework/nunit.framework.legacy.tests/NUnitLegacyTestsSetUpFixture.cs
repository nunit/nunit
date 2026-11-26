// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
{
    [SetUpFixture]
    public static class NUnitLegacyTestsSetUpFixture
    {
        private static IDisposable _stackFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the Legacy namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework, so we create a new StackFilter instance with a pattern that excludes Legacy.
            _stackFilter = StackFilter.SetNUnitFilter(@"^ +\w+:? NUnit\.Framework\.(?!Legacy\.)");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            _stackFilter.Dispose();
        }
    }
}
