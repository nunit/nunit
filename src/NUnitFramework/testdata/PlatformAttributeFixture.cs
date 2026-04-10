// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData
{
    [Platform(Includes = [NUnitOS])]
    public class PlatformAttributeFixture
    {
        public const string NUnitOS = "NUnit:OS";
        public const string NUnitArchitecture = "NUnit:Architecture";

        [Test]
        public void NoTestLevelAttributeSpecified()
        {
        }

        [Test]
        [Platform(Includes = [NUnitOS, NUnitArchitecture])]
        public void WithDuplicateProperty()
        {
        }

        [Test]
        [Platform(Includes = [NUnitArchitecture])]
        public void WithoutDuplicateProperty()
        {
        }

        [Test]
        [Platform(Excludes = [NUnitArchitecture])]
        public void WithExcludesProperty()
        {
        }

        [Test]
        [Platform(Includes = [PlatformNames.Win])]
        public void WindowsOnlyTest()
        {
        }

        [Test]
        [Platform(Includes = [PlatformNames.UNIX])]
        public void UnixOnlyTest()
        {
        }

        [Test]
        [Platform(Excludes = [PlatformNames.MacOSX])]
        public void RunsOnAllButMacOSX()
        {
        }
    }
}
