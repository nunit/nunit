namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class TestOutputSummaryParser : IParser<OutputSummary>
    {
        private static readonly string[] Fields = { "Test Count", "Passed", "Failed", "Inconclusive", "Skipped" };
        private static readonly Regex SummaryRegex = new Regex(string.Join(@"(,|)\s*", Fields.Select(f => $"{f}:\\s*(?<{GetKey(f)}>\\d+)")), RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public OutputSummary Parse(string text)
        {
            var match = SummaryRegex.Match(text);
            if (!match.Success)
            {
                return new OutputSummary(new Dictionary<string, string>());
            }

            var vals = (from field in Fields
                let valMatch = match.Groups[GetKey(field)]
                where valMatch.Success
                select new { field, value = valMatch.Value }).ToDictionary(i => i.field, i => i.value);


            return new OutputSummary(vals);
        }

        private static string GetKey(string field)
        {
            return field.Replace(" ", string.Empty);
        }
    }
}
