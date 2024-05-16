// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace CakeScripts.Tests;

public class CakeTests
{
    [TestCase("1.2.3-beta.1", "1.2.3.1")]
    [TestCase("1.2.3", "1.2.3.0")]
    public void ThatWeCanParseFileVersionStrings(string versionString, string expected)
    {
        
        var parsedVersion = VersionParsers.ParseFileVersion(versionString);
        Assert.That(parsedVersion, Is.EqualTo(expected));
    }

    [TestCase("1.2.3-beta.1", "1.2.3.0")]
    [TestCase("1.2.3", "1.2.3.0")]
    public void ThatWeCanParseAssemblyVersionStrings(string versionString, string expected)
    {
        var parsedVersion = VersionParsers.ParseAssemblyVersion(versionString);
        Assert.That(parsedVersion, Is.EqualTo(expected));
    }
}
