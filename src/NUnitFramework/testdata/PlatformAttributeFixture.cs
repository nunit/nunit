// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData
{
    [Platform(Includes = [PlatformNames.Win])]
    public class PlatformAttributeFixture
    {
        [Test]
        public void NoTestLevelAttributeSpecified()
        {
        }

        [Test]
        [Platform(Includes = [PlatformNames.Win, PlatformNames.X64BitOS])]
        public void WithDuplicateProperty()
        {
        }

        [Test]
        [Platform(Includes = [PlatformNames.X64BitOS])]
        public void WithoutDuplicateProperty()
        {
        }
    }
}
