// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NUnit.Framework.Tests.SolutionTests
{
    /// <summary>
    /// Checks that the all targets of the framework projects are included in the nuspec files.
    /// </summary>
    [TestFixture("framework/nunit.nuspec", new string[] { "nunit.framework", "nunit.framework.legacy" }, new string[] { ".dll", ".pdb", ".xml" })]
    [TestFixture("nunitlite/nunitlite.nuspec", new string[] { "nunitlite" }, new string[] { ".dll", ".pdb" })]
    internal sealed class NuspecTargetInclusionsTests
    {
        private readonly string _nuspecPath;
        private readonly string _propsPath;
        private readonly string[] _files;
        private readonly string[] _extensions;
        private string[] _csprojTargetFrameworks;

        private const string Root = "../../../../../../";
        private const string NotSpecified = nameof(NotSpecified);

        public NuspecTargetInclusionsTests(string nuspecPath, string[] files, string[] extensions)
        {
            _nuspecPath = Path.GetFullPath($"{Root}/nuget/{nuspecPath}");
            _propsPath = Path.GetFullPath($"{Root}/src/NUnitFramework/Directory.Build.props");
            _files = files;
            _extensions = extensions;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _csprojTargetFrameworks = PackageVersionReader.ExtractTargetFrameworks(_propsPath);
        }

        [Test]
        public void AllRequiredFilesAreIncludedInNuspec()
        {
            List<FileReference> nuspecIncludedLibFiles = NuspecReader.ExtractIncludedLibFiles(_nuspecPath);
            List<FileReference> requiredIncludedLibFiles = GetRequiredLibFiles();

            Assert.That(nuspecIncludedLibFiles, Is.EquivalentTo(requiredIncludedLibFiles));
        }

        private List<FileReference> GetRequiredLibFiles()
        {
            var requiredIncludedLibFiles = new List<FileReference>(_csprojTargetFrameworks.Length * _files.Length * _extensions.Length);
            foreach (var targetFramework in _csprojTargetFrameworks)
            {
                foreach (var file in _files)
                {
                    foreach (var extension in _extensions)
                    {
                        requiredIncludedLibFiles.Add(new FileReference(
                            $"bin{Path.AltDirectorySeparatorChar}{targetFramework}{Path.AltDirectorySeparatorChar}{file}{extension}",
                            $"lib{Path.AltDirectorySeparatorChar}{targetFramework}"));
                    }
                }
            }
            return requiredIncludedLibFiles;
        }

        private sealed record class FileReference(string Src, string Target)
        {
            public override string ToString()
            {
                return $"""
                    <file src="{Src}" target="{Target}" />
                    """;
            }
        }

        private static class PackageVersionReader
        {
            // Function to extract packages from the .csproj file
            public static string[] ExtractTargetFrameworks(string path)
            {
                Assert.That(path, Does.Exist, $"props file at {path} not found.");
                string xml = File.ReadAllText(path);

                var doc = XDocument.Parse(xml);

                foreach (var itemGroup in doc.Descendants("PropertyGroup"))
                {
                    XElement? packageReferences = itemGroup.Descendants("NUnitLibraryFrameworks").LastOrDefault();

                    if (packageReferences is not null)
                    {
                        return packageReferences.Value.Split(';');
                    }
                }

                throw new Exception("No 'NUnitLibraryFrameworks' property found in Directory.Build.props");
            }
        }

        private static class NuspecReader
        {
            public static List<FileReference> ExtractIncludedLibFiles(string path)
            {
                Assert.That(path, Does.Exist, $"Nuspec file at {path} not found.");
                string xml = File.ReadAllText(path);

                var doc = XDocument.Parse(xml);
                XNamespace ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

                // List to include all lib files in the nuspec
                var includedLibFiles = new List<FileReference>();

                // Find the dependencies section
                var filesSection = doc.Descendants(ns + "files").FirstOrDefault();
                if (filesSection is not null)
                {
                    // Iterate over all files
                    foreach (var group in filesSection.Descendants())
                    {
                        // Get the target attribute
                        var target = group.Attribute("target")?.Value;

                        if (target?.StartsWith("lib") is true)
                        {
                            // Get the src attribute
                            var src = group.Attribute("src")?.Value;
                            if (src is not null)
                            {
                                includedLibFiles.Add(new FileReference(
                                    src.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                                    target.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)));
                            }
                        }
                    }
                }

                return includedLibFiles;
            }
        }
    }
}
