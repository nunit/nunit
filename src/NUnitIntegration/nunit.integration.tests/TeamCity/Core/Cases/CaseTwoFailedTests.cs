using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseTwoFailedTests : CaseBase
    {
        public CaseTwoFailedTests()
            : base(CertType.TestFramework, "TwoFailedTests", "2 failed tests")
        {            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = GetTestMessages(messages).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 6, out result))
            {
                return result;
            }

            var testStarted1 = testMessages[0];
            var testFailed1 = testMessages[1];
            var testFinished1 = testMessages[2];
            var testStarted2 = testMessages[3];
            var testFailed2 = testMessages[4];
            var testFinished2 = testMessages[5];

            if (!CheckNameOfPair(testStarted1, testFinished1, out result))
            {
                return result;
            }

            if (!CheckNameOfPair(testStarted1, testFailed1, out result))
            {
                return result;
            }

            if (!CheckNameOfPair(testStarted2, testFinished2, out result))
            {
                return result;
            }

            if (!CheckNameOfPair(testStarted2, testFailed2, out result))
            {
                return result;
            }
            
            return new ValidationResult(ValidationState.Valid);
        }
    }
}