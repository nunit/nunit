// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    [SetUpFixture]
    public static class NUnitTestsSetUpFixture
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the NUnit namespace as that is what we are testing here.
            // So we are setting a filter that doesn't match anything.
            StackFilter.DefaultFilter = new StackFilter("^$");
        }
    }
}
