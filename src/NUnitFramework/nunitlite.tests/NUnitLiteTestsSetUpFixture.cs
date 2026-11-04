// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitLite.Tests
{
    [SetUpFixture]
    public static class NUnitLiteTestsSetUpFixture
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            // We don't want to filter out the NUnitLite namespace as that is what we are testing here.
            // We do want to filter out the rest of NUnit.Framework.
            StackFilter.DefaultFilter = new StackFilter(@"^ +\w+:? NUnit\.Framework\.");
        }
    }
}
