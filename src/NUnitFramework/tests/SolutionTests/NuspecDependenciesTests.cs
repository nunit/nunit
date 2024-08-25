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
    [TestFixture("framework/nunit.nuspec", "NUnitFramework/framework/nunit.framework.csproj")]
    [TestFixture("nunitlite/nunitlite.nuspec", "NUnitFramework/nunitlite/nunitlite.csproj")]
    internal sealed class NuspecDependenciesTests
    {
        private readonly string _nuspecPath;
        private readonly string _csprojPath;

        private Dictionary<string, List<string>> _nuspecPackages;
        private Dictionary<string, List<string>> _csprojPackages;

        private const string Root = "../../../../../../";
        private const string NotSpecified = nameof(NotSpecified);

        public NuspecDependenciesTests(string nuspecPath, string csProjPath)
        {
            _nuspecPath = Path.GetFullPath($"{Root}/nuget/{nuspecPath}");
            _csprojPath = Path.GetFullPath($"{Root}/src/{csProjPath}");
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _nuspecPackages = NuspecReader.ExtractNuspecPackages(_nuspecPath);
            _csprojPackages = CsprojReader.ExtractCsprojPackages(_csprojPath);
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

        private static class CsprojReader
        {
            // Function to extract packages from the .csproj file
            public static Dictionary<string, List<string>> ExtractCsprojPackages(string path)
            {
                Assert.That(path, Does.Exist, $"Csproj file at {path} not found.");
                string xml = File.ReadAllText(path);

                var doc = XDocument.Parse(xml);
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

                    if (itemGroup.Descendants("ProjectReference")
                                 .Select(pr => pr.Attribute("Include")?.Value)
                                 .Where(include => !string.IsNullOrEmpty(include)) // Ensure it's non-null and non-empty
                                 .Select(include => include!) // Use non-null assertion to ensure the result is a List<string>
                                 .Any(include => include.EndsWith("nunit.framework.csproj")))
                    {
                        packageReferences.Add("NUnit");
                    }

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

        private static class NuspecReader
        {
            public static Dictionary<string, List<string>> ExtractNuspecPackages(string path)
            {
                Assert.That(path, Does.Exist, $"Nuspec file at {path} not found.");
                string xml = File.ReadAllText(path);

                var doc = XDocument.Parse(xml);
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
                        var framework = group.Attribute("targetFramework")?.Value ?? NotSpecified;

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

        private sealed class VerifyDependencies
        {
            public static void ComparePackages(Dictionary<string, List<string>> csprojPackages, Dictionary<string, List<string>> nuspecPackages)
            {
                // Iterate through the frameworks in the csprojPackages dictionary
                foreach (var csprojFramework in csprojPackages.Keys)
                {
                    if (csprojFramework == NotSpecified)
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
