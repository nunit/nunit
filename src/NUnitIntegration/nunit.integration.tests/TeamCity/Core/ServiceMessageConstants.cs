namespace JetBrains.TeamCityCert.Tools
{
    internal static class ServiceMessageConstants
    {
        public const string ServiceMessageOpen = "##teamcity[";
        public const string ServiceMessageClose = "]";

        public const string TestSuiteStartedMessageName = "testSuiteStarted";
        public const string TestSuiteFinishedMessageName = "testSuiteFinished";
        public const string TestStartedMessageName = "testStarted";
        public const string TestFinishedMessageName = "testFinished";

        public const string MessageAttributeName = "name";
    }
}