using System.Diagnostics;

namespace AppxRunner
{
    [DebuggerDisplay("{PackagePath,nq} ({SourcePath,nq})")]
    internal readonly struct AppxLayoutFile
    {
        public string SourcePath { get; }
        public string PackagePath { get; }

        public AppxLayoutFile(string sourcePath, string packagePath)
        {
            SourcePath = sourcePath;
            PackagePath = packagePath;
        }
    }
}