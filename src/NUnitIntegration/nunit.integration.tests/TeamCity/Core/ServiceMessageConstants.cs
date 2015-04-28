namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal static class ServiceMessageConstants
    {
        public const string ServiceMessageOpen = "##teamcity[";
        public const string ServiceMessageClose = "]";

        public const string TestSuiteStartedMessageName = "testSuiteStarted";
        public const string TestSuiteFinishedMessageName = "testSuiteFinished";
        public const string TestStartedMessageName = "testStarted";
        public const string TestFinishedMessageName = "testFinished";
        public const string TestFailedMessageName = "testFailed";
        public const string TestIgnoredMessageName = "testIgnored";
        public const string TestStdOutMessageName = "testStdOut";
        public const string TestStdErrMessageName = "testStdErr";

        public const string MessageAttributeName = "name";
        public const string MessageAttributeFlowId = "flowId";
    }
}