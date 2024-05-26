// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

public static class VersionParsers
{
    public static string ParseAssemblyVersion(string version)
    {
        var dash = version.LastIndexOf('-');
        if (dash > 0)
        {
            return string.Concat(version.AsSpan(0, dash), ".0");
        }
        return version + ".0";
    }
}
