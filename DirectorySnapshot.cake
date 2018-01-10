public sealed class DirectorySnapshot
{
    private readonly string directory;
    private readonly string filePattern;
    private readonly Dictionary<string, byte[]> original = new Dictionary<string, byte[]>();

    public static DirectorySnapshot Read(string directory, string filePattern)
    {
        return new DirectorySnapshot(directory, filePattern);
    }

    private DirectorySnapshot(string directory, string filePattern)
    {
        this.directory = directory;
        this.filePattern = filePattern;

        foreach (var file in GetFiles())
            original.Add(System.IO.Path.GetFileName(file), System.IO.File.ReadAllBytes(file));
    }

    private IReadOnlyCollection<string> GetFiles()
    {
        return System.IO.Directory.GetFiles(directory, filePattern);
    }

    public IReadOnlyCollection<string> GetChangedFiles()
    {
        return GetFiles().Where(IsChanged).ToList();
    }

    private bool IsChanged(string filePath)
    {
        if (original.TryGetValue(System.IO.Path.GetFileName(filePath), out var originalBytes))
        {
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                return !AreEqualLines(stream, originalBytes);
            }
        }
        else
        {
            return true;
        }
    }

    /// <summary>Determines whether the byte sequences are equal with tolerance for LF vs CRLF line endings.</summary>
    private static bool AreEqualLines(Stream stream, byte[] array)
    {
        for (var position = 0; position < array.Length; position++)
        {
            var next = stream.ReadByte();
            if (next != array[position])
            {
                if (next == '\r')
                {
                    if (array[position] == '\n')
                    {
                        if (stream.ReadByte() != '\n') return false;
                    }
                }
                else if (next == '\n')
                {
                    if (array[position] == '\r')
                    {
                        position++;
                        if (array.Length <= position || array[position] != '\n') return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return stream.ReadByte() == -1;
    }
}
