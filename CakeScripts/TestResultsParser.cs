// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Xml.Linq;

public record TestResultsSummary(int Total, int Passed, int Failed, int Skipped);

public static class TestResultsParser
{
    /// <summary>
    /// Parses TRX files in the specified directory and returns aggregated test results.
    /// </summary>
    /// <param name="resultsDirectory">Directory containing .trx files</param>
    /// <returns>Aggregated test results summary</returns>
    public static TestResultsSummary ParseTrxFiles(string resultsDirectory)
    {
        int totalTests = 0, totalPassed = 0, totalFailed = 0, totalSkipped = 0;

        var trxFiles = System.IO.Directory.GetFiles(resultsDirectory, "*.trx", System.IO.SearchOption.AllDirectories);

        foreach (var trxFile in trxFiles)
        {
            var (total, passed, failed, skipped) = ParseTrxFile(trxFile);
            totalTests += total;
            totalPassed += passed;
            totalFailed += failed;
            totalSkipped += skipped;
        }

        return new TestResultsSummary(totalTests, totalPassed, totalFailed, totalSkipped);
    }

    /// <summary>
    /// Parses a single TRX file and returns the test counts.
    /// </summary>
    private static (int Total, int Passed, int Failed, int Skipped) ParseTrxFile(string trxPath)
    {
        var xml = XDocument.Load(trxPath);
        var ns = xml.Root?.GetDefaultNamespace() ?? XNamespace.None;
        var counters = xml.Descendants(ns + "Counters").FirstOrDefault();

        if (counters == null)
            return (0, 0, 0, 0);

        var total = int.Parse(counters.Attribute("total")?.Value ?? "0");
        var passed = int.Parse(counters.Attribute("passed")?.Value ?? "0");
        var failed = int.Parse(counters.Attribute("failed")?.Value ?? "0");
        var executed = int.Parse(counters.Attribute("executed")?.Value ?? "0");
        var skipped = total - executed;

        return (total, passed, failed, skipped);
    }
}
