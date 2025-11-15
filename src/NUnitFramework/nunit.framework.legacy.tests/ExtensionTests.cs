// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ExtensionTests
    {
        [Test]
        public void ThatWeCanUseLegacyAssertsInNUnitFrameworkNamespace()
        {
            // This is a simple smoke test to ensure the Assert.* legacy extension methods
            // are available on NUnit.Framework.Assert and execute correctly.
            int x = 42;
            Assert.AreEqual(42, x, "Some message");
        }
    }
}
