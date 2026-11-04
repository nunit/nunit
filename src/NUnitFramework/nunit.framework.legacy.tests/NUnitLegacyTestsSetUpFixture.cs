// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Legacy.Tests
{
    [SetUpFixture]
    public static class NUnitLegacyTestsSetUpFixture
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the Legacy namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework, so we create a new StackFilter instance with a pattern that excludes Legacy.
            StackFilter.DefaultFilter = new StackFilter(@"^ +\w+:? NUnit\.Framework\.(?!Legacy\.)");
        }
    }
}
