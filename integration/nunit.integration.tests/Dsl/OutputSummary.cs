namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;

    internal class OutputSummary
    {
        private readonly IDictionary<string, string> _vals;

        public OutputSummary(IDictionary<string, string> vals)
        {
            _vals = vals;
        }

        public bool TryGetValue(string key, out string value)
        {
            return _vals.TryGetValue(key, out value);
        }
    }
}