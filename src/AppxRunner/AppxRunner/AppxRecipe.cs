using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace AppxRunner
{
    internal sealed class AppxRecipe
    {
        public string PackageIdentityName { get; }
        public string PackageIdentityPublisher { get; }
        public string LayoutDir { get; }

        public bool RemoveNonLayoutFiles { get; }
        public IReadOnlyList<AppxLayoutFile> LayoutFiles { get; }

        public AppxRecipe(string packageIdentityName, string packageIdentityPublisher, string layoutDir, bool removeNonLayoutFiles, IReadOnlyList<AppxLayoutFile> layoutFiles)
        {
            PackageIdentityName = packageIdentityName;
            PackageIdentityPublisher = packageIdentityPublisher;
            LayoutDir = layoutDir;
            RemoveNonLayoutFiles = removeNonLayoutFiles;
            LayoutFiles = layoutFiles;
        }

        public static AppxRecipe Load(string path)
        {
            var project = new Project(path);

            return new AppxRecipe(
                packageIdentityName: project.GetPropertyValue("PackageIdentityName"),
                packageIdentityPublisher: project.GetPropertyValue("PackageIdentityPublisher"),
                layoutDir: project.GetPropertyValue("LayoutDir"),
                removeNonLayoutFiles: bool.Parse(DefaultIfNullOrEmpty(project.GetPropertyValue("RemoveNonLayoutFiles"), "false")),
                layoutFiles: project.AllEvaluatedItems
                    .Select(item => new AppxLayoutFile(item.EvaluatedInclude, item.GetMetadataValue("PackagePath")))
                    .Where(file => !string.IsNullOrWhiteSpace(file.PackagePath))
                    .ToList());
        }

        private static string DefaultIfNullOrEmpty(string value, string @default)
        {
            return string.IsNullOrEmpty(value) ? @default : value;
        }

        public void InitializeLayout()
        {
            if (RemoveNonLayoutFiles)
            {
                try
                {
                    Directory.Delete(LayoutDir, recursive: true);
                }
                catch (DirectoryNotFoundException)
                {
                }
            }

            Directory.CreateDirectory(LayoutDir);

            foreach (var file in LayoutFiles)
            {
                var destinationPath = Path.Combine(LayoutDir, file.PackagePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                File.Copy(file.SourcePath, destinationPath, overwrite: true);
            }
        }
    }
}
