// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests;

public class CakeTests
{
    [TestCase("1.2.3-beta.1")]
    [TestCase("1.2.3")]
    public void ThatWeCanParseVersionStrings(string versionString)
    {
        Assert.That(Version.TryParse(versionString, out _), Is.True);
    }
}

public class CakeCode
{
    public string ParseVersion(string version)
    {
        return version.Substring(0, version.LastIndexOf('-')) + ".0";
    }
}
