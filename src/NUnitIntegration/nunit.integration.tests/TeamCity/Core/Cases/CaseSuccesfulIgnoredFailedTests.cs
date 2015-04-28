using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseSuccesfulIgnoredFailedTests : CaseBase
    {
        public CaseSuccesfulIgnoredFailedTests()
            : base(CertType.TestFramework, "SuccesfulIgnoredFailedTests", "1 succesful 1 ignored 1 failed tests")
        {            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = GetTestMessages(messages).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 7, out result))
            {
                return result;
            }

            var testStarted1 = testMessages[0];
            var testFinished1 = testMessages[1];
            var testStarted2 = testMessages[2];
            var testIgnored2 = testMessages[3];
            var testStarted3 = testMessages[4];
            var testFailed3 = testMessages[5];
            var testFinished3 = testMessages[6];

            if (!CheckPair(testStarted1, testFinished1, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted2, testIgnored2, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted3, testFinished3, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted3, testFailed3, out result))
            {
                return result;
            }
            
            return new ValidationResult(ValidationState.Valid);
        }
    }
}