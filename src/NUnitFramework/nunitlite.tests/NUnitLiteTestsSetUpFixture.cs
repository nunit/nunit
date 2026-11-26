// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitLite.Tests
{
    [SetUpFixture]
    public static class NUnitLiteTestsSetUpFixture
    {
        private static IDisposable _stackFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the NUnitLite namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework.
            _stackFilter = StackFilter.SetNUnitFilter(@"^ +\w+:? NUnit\.Framework\.");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            _stackFilter.Dispose();
        }
    }
}
