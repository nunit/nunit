public static T WithRawArgument<T>(this T settings, string rawArgument) where T : Cake.Core.Tooling.ToolSettings
{
    if (settings == null) throw new ArgumentNullException(nameof(settings));

    if (!string.IsNullOrEmpty(rawArgument))
    {
        var previousCustomizer = settings.ArgumentCustomization;
        if (previousCustomizer != null)
            settings.ArgumentCustomization = builder => previousCustomizer.Invoke(builder).Append(rawArgument);
        else
            settings.ArgumentCustomization = builder => builder.Append(rawArgument);
    }

    return settings;
}

void DeleteDirectoryRobust(params string[] directories)
{
    if (!directories.Any()) return;

    Information("Deleting directories:");

    foreach (var directory in directories)
    {
        for (var attempt = 1;; attempt++)
        {
            Information(directory);
            try
            {
                System.IO.Directory.Delete(directory, recursive: true);
                break;
            }
            catch (DirectoryNotFoundException)
            {
                break;
            }
            catch (IOException ex) when (attempt < 3 && (WinErrorCode)ex.HResult == WinErrorCode.DirNotEmpty)
            {
                Information("Another process added files to the directory while its contents were being deleted. Retrying...");
            }
        }
    }
}

private enum WinErrorCode : ushort
{
    DirNotEmpty = 145
}

public sealed class TempDirectory : IDisposable
{
    public DirectoryPath Path { get; }

    public TempDirectory()
    {
        Path = new DirectoryPath(System.IO.Path.GetTempPath()).Combine(System.IO.Path.GetRandomFileName());
        System.IO.Directory.CreateDirectory(Path.FullPath);
    }

    public void Dispose()
    {
        System.IO.Directory.Delete(Path.FullPath, recursive: true);
    }
}
