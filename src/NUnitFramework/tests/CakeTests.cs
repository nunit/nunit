// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests;

public class CakeTests
{
    [TestCase("1.2.3-beta.1", "1.2.3.0")]
    [TestCase("1.2.3", "1.2.3.0")]
    public void ThatWeCanParseVersionStrings(string versionString, string expected)
    {
        var cake = new CakeCode();
        var parsedVersion = cake.ParseVersion(versionString);
        Assert.That(parsedVersion, Is.EqualTo(expected));
    }
}

public class CakeCode
{
    public string ParseVersion(string version)
    {
        return version.Substring(0, version.LastIndexOf('-')) + ".0";
    }
}
