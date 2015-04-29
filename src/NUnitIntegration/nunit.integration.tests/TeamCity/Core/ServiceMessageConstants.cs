// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
        public const string MessageAttributeCaptureStandardOutput = "captureStandardOutput";
        public const string MessageAttributeDuration = "duration";
        public const string MessageAttributeMessage = "message";
        public const string MessageAttributeDetails = "details";
        public const string MessageAttributeOut = "out";
    }
}