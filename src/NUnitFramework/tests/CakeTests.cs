// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests;

public class CakeTests
{
    [TestCase("1.2.3-beta.1", "1.2.3.1")]
    [TestCase("1.2.3", "1.2.3.0")]
    public void ThatWeCanParseFileVersionStrings(string versionString, string expected)
    {
        var cake = new CakeCode();
        var parsedVersion = cake.ParseFileVersion(versionString);
        Assert.That(parsedVersion, Is.EqualTo(expected));
    }

    [TestCase("1.2.3-beta.1", "1.2.3.0")]
    [TestCase("1.2.3", "1.2.3.0")]
    public void ThatWeCanParseAssemblyVersionStrings(string versionString, string expected)
    {
        var cake = new CakeCode();
        var parsedVersion = cake.ParseAssemblyVersion(versionString);
        Assert.That(parsedVersion, Is.EqualTo(expected));
    }
}

/// <summary>
/// This class is copied into the cake script. It is just here to be tested.
/// </summary>
public class CakeCode
{
    public string ParseFileVersion(string version)
    {
        var dash = version.LastIndexOf('-');
        if (dash > 0)
        {
            var fourthDigitValue = version.Substring(version.LastIndexOf('.') + 1);
            var fourthDigit = int.Parse(fourthDigitValue);
            return string.Concat(version.Substring(0, dash), $".{fourthDigit}");
        }
        return version + ".0";
    }

    public string ParseAssemblyVersion(string version)
    {
        var dash = version.LastIndexOf('-');
        if (dash > 0)
        {
            return string.Concat(version.Substring(0, dash), ".0");
        }
        return version + ".0";
    }
}
