using System.Collections.Generic;

namespace NUnit.Integration.Tests.TeamCity
{
    internal static class CaseList
    {
        public static readonly IEnumerable<string> Cases = new[]
        {
            "OneSuccesfulTest", "TwoSuccesfulTests",
            "OneFailedTest", "TwoFailedTests", "OneIgnoredTest",
            "TwoIgnoredTests", "SuccesfulIgnoredFailedTests",
        };
    }
}
