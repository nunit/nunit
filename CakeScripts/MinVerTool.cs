// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics;

public enum AutoIncrement
{
    Major,
    Minor,
    Patch,
}

public static class MinVerTool
{
    public static string GetVersion(AutoIncrement autoIncrement = AutoIncrement.Minor, string tagPrefix = "v")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"minver --auto-increment {autoIncrement.ToString().ToLower()} --tag-prefix {tagPrefix}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start minver process");
        var output = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        return output;
    }
}
