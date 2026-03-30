// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.TestData.TestUtilities;
using NUnit.Framework.Internal;

namespace NUnit.TestData
{
    [ObservablePlatform(Includes = [PlatformNames.DotNET])]
    public class PlatformAttributeFixture
    {
        [Test]
        public void NoTestLevelAttributeSpecified()
        {
        }

        [Test]
        [ObservablePlatform(Includes = [PlatformNames.DotNET, PlatformNames.X64BitOS])]
        public void WithDuplicateProperty()
        {
        }

        [Test]
        [ObservablePlatform(Includes = [PlatformNames.X64BitOS])]
        public void WithoutDuplicateProperty()
        {
        }
    }
}
