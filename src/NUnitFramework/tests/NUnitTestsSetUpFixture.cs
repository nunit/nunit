// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    [SetUpFixture]
    public static class NUnitTestsSetUpFixture
    {
        private static IDisposable _stackFilter;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the NUnit namespace as that is what we are testing here.
            // So we are not using a pattern for NUnit frames.
            _stackFilter = StackFilter.SetNUnitFilter(nunitStackPattern: null);
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            // Restore the filter for other assemblies that may run in the same process.
            _stackFilter.Dispose();
        }
    }
}
