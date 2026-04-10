// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public record TestResultsSummary(int Total, int Passed, int Failed, int Skipped);

public record FrameworkTestResult(string Framework, int Total, int Passed, int Failed, int Skipped);

public record DetailedTestResultsSummary(
    int Total,
    int Passed,
    int Failed,
    int Skipped,
    List<FrameworkTestResult> ByFramework);

public static class TestResultsParser
{
    // Regex to extract framework from paths like bin/Release/net8.0/ or bin\Release\net462\
    private static readonly Regex FrameworkPattern = new Regex(
        @"[/\\](net\d+\.\d+|net\d+|netcoreapp\d+\.\d+|netstandard\d+\.\d+)[/\\]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses TRX files in the specified directory and returns aggregated test results.
    /// </summary>
    /// <param name="resultsDirectory">Directory containing .trx files</param>
    /// <returns>Aggregated test results summary</returns>
    public static TestResultsSummary ParseTrxFiles(string resultsDirectory)
    {
        var detailed = ParseTrxFilesDetailed(resultsDirectory);
        return new TestResultsSummary(detailed.Total, detailed.Passed, detailed.Failed, detailed.Skipped);
    }

    /// <summary>
    /// Parses TRX files and returns detailed results including per-framework breakdown.
    /// </summary>
    public static DetailedTestResultsSummary ParseTrxFilesDetailed(string resultsDirectory)
    {
        int totalTests = 0, totalPassed = 0, totalFailed = 0, totalSkipped = 0;
        var frameworkResults = new Dictionary<string, (int Total, int Passed, int Failed, int Skipped)>();

        var trxFiles = System.IO.Directory.GetFiles(resultsDirectory, "*.trx", System.IO.SearchOption.AllDirectories);

        foreach (var trxFile in trxFiles)
        {
            var (total, passed, failed, skipped, framework) = ParseTrxFileWithFramework(trxFile);
            totalTests += total;
            totalPassed += passed;
            totalFailed += failed;
            totalSkipped += skipped;

            if (!string.IsNullOrEmpty(framework))
            {
                if (frameworkResults.TryGetValue(framework, out var existing))
                {
                    frameworkResults[framework] = (
                        existing.Total + total,
                        existing.Passed + passed,
                        existing.Failed + failed,
                        existing.Skipped + skipped);
                }
                else
                {
                    frameworkResults[framework] = (total, passed, failed, skipped);
                }
            }
        }

        var byFramework = frameworkResults
            .OrderBy(kv => kv.Key)
            .Select(kv => new FrameworkTestResult(kv.Key, kv.Value.Total, kv.Value.Passed, kv.Value.Failed, kv.Value.Skipped))
            .ToList();

        return new DetailedTestResultsSummary(totalTests, totalPassed, totalFailed, totalSkipped, byFramework);
    }

    /// <summary>
    /// Parses a single TRX file and returns the test counts plus detected framework.
    /// </summary>
    private static (int Total, int Passed, int Failed, int Skipped, string Framework) ParseTrxFileWithFramework(string trxPath)
    {
        var xml = XDocument.Load(trxPath);
        var ns = xml.Root?.GetDefaultNamespace() ?? XNamespace.None;
        var counters = xml.Descendants(ns + "Counters").FirstOrDefault();

        if (counters == null)
            return (0, 0, 0, 0, "");

        var total = int.Parse(counters.Attribute("total")?.Value ?? "0");
        var passed = int.Parse(counters.Attribute("passed")?.Value ?? "0");
        var failed = int.Parse(counters.Attribute("failed")?.Value ?? "0");
        var executed = int.Parse(counters.Attribute("executed")?.Value ?? "0");
        var skipped = total - executed;

        // Try to extract framework from test assembly paths in TestDefinitions
        var framework = ExtractFramework(xml, ns);

        return (total, passed, failed, skipped, framework);
    }

    /// <summary>
    /// Extracts the target framework from test definition paths in the TRX file.
    /// </summary>
    private static string ExtractFramework(XDocument xml, XNamespace ns)
    {
        // Look for codeBase attribute in UnitTest elements which contains the assembly path
        var codeBases = xml.Descendants(ns + "UnitTest")
            .Select(ut => ut.Attribute("storage")?.Value)
            .Where(s => !string.IsNullOrEmpty(s));

        foreach (var codeBase in codeBases)
        {
            var match = FrameworkPattern.Match(codeBase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToLowerInvariant();
            }
        }

        // Also try TestMethod codeBase
        var testMethods = xml.Descendants(ns + "TestMethod")
            .Select(tm => tm.Attribute("codeBase")?.Value)
            .Where(s => !string.IsNullOrEmpty(s));

        foreach (var codeBase in testMethods)
        {
            var match = FrameworkPattern.Match(codeBase);
            if (match.Success)
            {
                return match.Groups[1].Value.ToLowerInvariant();
            }
        }

        return "";
    }
}
