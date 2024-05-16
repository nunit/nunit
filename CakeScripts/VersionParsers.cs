// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

public static class VersionParsers
{
    public static string ParseFileVersion(string version)
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

    public static string ParseAssemblyVersion(string version)
    {
        var dash = version.LastIndexOf('-');
        if (dash > 0)
        {
            return string.Concat(version.Substring(0, dash), ".0");
        }
        return version + ".0";
    }
}
