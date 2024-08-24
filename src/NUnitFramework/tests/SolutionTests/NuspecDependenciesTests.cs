// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NUnit.Framework.Tests.SolutionTests
{
    /// <summary>
    /// Checks that the csproj of the framework projects are reflected correctly in the nuspec files.
    /// </summary>
    internal class NuspecDependenciesTests
    {
        private Dictionary<string, List<string>> _nuspecPackages;
        private Dictionary<string, List<string>> _csprojPackages;
        private const string Root = "../../../../../../";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var nuspec = new NuspecReader("framework/nunit.nuspec");
            var csproj = new CsprojReader("framework/nunit.framework.csproj");
            _nuspecPackages = nuspec.ExtractNuspecPackages();
            _csprojPackages = csproj.ExtractCsprojPackages();
        }

        [Test]
        public void NUnitFrameworkNuspecContainsCorrectDependencies()
        {
            VerifyDependencies.ComparePackages(_csprojPackages, _nuspecPackages);
        }

        [Test]
        public void AllPackagesInNuspecIsInCsproj()
        {
            VerifyDependencies.CheckNuspecPackages(_nuspecPackages, _csprojPackages);
        }

        internal sealed class CsprojReader
        {
            private const string PathToFrameworkFolder = $"{Root}/src/NUnitFramework/";

            public const string NotSpecified = nameof(NotSpecified);

            private string Xml { get; }

            public CsprojReader(string nunitFrameworkCsproj)
            {
                var path = Path.GetFullPath(Path.Combine(PathToFrameworkFolder, nunitFrameworkCsproj));
                Assert.That(File.Exists(path), $"Csproj file at {path} not found.");
                Xml = File.ReadAllText(path);
            }

            // Function to extract packages from the .csproj file
            public Dictionary<string, List<string>> ExtractCsprojPackages()
            {
                var doc = XDocument.Parse(Xml);
                var groupedPackages = new Dictionary<string, List<string>>();

                foreach (var itemGroup in doc.Descendants("ItemGroup"))
                {
                    var condition = itemGroup.Attribute("Condition")?.Value;
                    string framework = NotSpecified;

                    if (!string.IsNullOrEmpty(condition) && condition.Contains("TargetFrameworkIdentifier"))
                    {
                        var split = condition.Split(new[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                        framework = split[1].Trim().Replace("'", string.Empty);
                    }

                    var packageReferences = itemGroup.Descendants("PackageReference")
                                                     .Select(pr => pr.Attribute("Include")?.Value)
                                                     .Where(include => !string.IsNullOrEmpty(include)) // Ensure it's non-null and non-empty
                                                     .Select(include => include!) // Use non-null assertion to ensure the result is a List<string>
                                                     .ToList();

                    if (!groupedPackages.TryGetValue(framework, out List<string>? referencesForFramework))
                    {
                        referencesForFramework = new List<string>();
                        groupedPackages[framework] = referencesForFramework;
                    }

                    referencesForFramework.AddRange(packageReferences);
                }

                return groupedPackages;
            }
        }

        internal sealed class NuspecReader
        {
            private const string PathToNuspecFolder = $"{Root}/nuget/";

            private string Xml { get; }

            public NuspecReader(string nuspecPath)
            {
                var path = Path.GetFullPath(Path.Combine(PathToNuspecFolder, nuspecPath));
                Assert.That(File.Exists(path), $"Nuspec file at {path} not found.");
                Xml = File.ReadAllText(path);
            }

            public Dictionary<string, List<string>> ExtractNuspecPackages()
            {
                var doc = XDocument.Parse(Xml);
                XNamespace ns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

                // Dictionary to hold dependencies grouped by targetFramework
                var groupedDependencies = new Dictionary<string, List<string>>();

                // Find the dependencies section
                var dependenciesSection = doc.Descendants(ns + "dependencies").FirstOrDefault();
                if (dependenciesSection is not null)
                {
                    // Iterate over all group elements within the dependencies block
                    foreach (var group in dependenciesSection.Elements(ns + "group"))
                    {
                        // Get the targetFramework attribute
                        var framework = group.Attribute("targetFramework")?.Value ?? CsprojReader.NotSpecified;

                        // Find all dependency elements in the current group
                        var dependencies = group.Elements(ns + "dependency")
                                                .Select(dep => dep.Attribute("id")?.Value)
                                                .Where(dep => !string.IsNullOrEmpty(dep))
                                                .Select(dep => dep!) // Ensure non-null values
                                                .ToList();

                        // Add dependencies to the respective framework group
                        if (!groupedDependencies.TryGetValue(framework, out List<string>? dependenciesForFramework))
                        {
                            dependenciesForFramework = new List<string>();
                            groupedDependencies[framework] = dependenciesForFramework;
                        }

                        dependenciesForFramework.AddRange(dependencies);
                    }
                }

                return groupedDependencies;
            }
        }

        public class VerifyDependencies
        {
            public static void ComparePackages(Dictionary<string, List<string>> csprojPackages, Dictionary<string, List<string>> nuspecPackages)
            {
                // Iterate through the frameworks in the csprojPackages dictionary
                foreach (var csprojFramework in csprojPackages.Keys)
                {
                    if (csprojFramework == CsprojReader.NotSpecified)
                    {
                        TestContext.Out.WriteLine("Checking for packages that should be in all frameworks in the .nuspec file");
                        // Check if the packages from the csproj are present in all nuspec framework
                        Assert.Multiple(() =>
                        {
                            foreach (var framework in nuspecPackages.Keys)
                            {
                                var missingPackages = csprojPackages[csprojFramework].Except(nuspecPackages[framework]).ToList();

                                // Assert that there are no missing packages in any nuspec framework
                                Assert.That(missingPackages, Is.Empty,
                                    $"Missing packages in framework '{framework}' in .nuspec: {string.Join(", ", missingPackages)}");
                            }
                        });
                    }
                    else
                    {
                        TestContext.Out.WriteLine($"Checking for packages that should be in corresponding '{csprojFramework}' in the .nuspec file");
                        // Handle specific framework case
                        var matchingNuspecFramework = nuspecPackages.Keys.FirstOrDefault(nuspecFramework =>
                            (csprojFramework == ".NETFramework" && nuspecFramework.StartsWith("net") &&
                             int.TryParse(nuspecFramework.Substring(3), out var version) && version >= 462) ||
                            (csprojFramework != ".NETFramework" && nuspecFramework == csprojFramework));

                        // Assert that the matching framework was found
                        Assert.That(matchingNuspecFramework, Is.Not.Null, $"Framework '{csprojFramework}' is in .csproj but not in .nuspec.");

                        // Find packages in csproj that are missing in nuspec
                        var missingPackages = csprojPackages[csprojFramework].Except(nuspecPackages[matchingNuspecFramework]).ToList();

                        // Assert that no packages are missing
                        Assert.That(missingPackages, Is.Empty, $"Missing packages for framework '{csprojFramework}' in .nuspec: {string.Join(", ", missingPackages)}");
                    }
                }
            }

            public static void CheckNuspecPackages(Dictionary<string, List<string>> nuspecPackages, Dictionary<string, List<string>> csprojPackages)
            {
                // Extract all packages from csproj
                var allCsprojPackages = csprojPackages.Values.SelectMany(x => x);

                // Extract all packages from nuspec
                var allNuspecPackages = nuspecPackages.Values.SelectMany(x => x).ToList();

                // Find packages in nuspec that are missing in csproj
                var missingPackages = allNuspecPackages.Except(allCsprojPackages).ToList();
                Assert.That(missingPackages, Is.Empty, $"Packages in .nuspec that are not in .csproj and should be deleted from nuspec: {string.Join(", ", missingPackages)}");
            }
        }
    }
}
