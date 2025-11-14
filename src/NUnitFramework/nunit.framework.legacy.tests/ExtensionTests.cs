// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ExtensionTests
    {
        [Test]
        public void ThatWeCanUseLegacyAssertsInNUnitFrameworkNamespace()
        {
            int x = 43;
            Assert.AreEqual(42, x, "Some message");
        }
    }
}
