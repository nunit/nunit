using System.Collections.Generic;

namespace NUnit.Integration.Tests.TeamCity
{
    internal static class CaseLists
    {
        public static readonly IEnumerable<string> GeneralCases = new[]
        {
            "OneSuccesfulTest",
            "TwoSuccesfulTests",
            "OneFailedTest",
            "TwoFailedTests",
            "OneIgnoredTest",
            "TwoIgnoredTests",
            "SuccesfulIgnoredFailedTests",
        };

        public static readonly IEnumerable<string> MultithreadingCases = new[]
        {
            "MultithreadingTests",
        };
    }
}
