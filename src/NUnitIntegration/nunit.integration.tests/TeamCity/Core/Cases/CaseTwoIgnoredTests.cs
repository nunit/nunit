using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseTwoIgnoredTests : CaseBase
    {
        public CaseTwoIgnoredTests()
            : base(CertType.TestFramework, "TwoIgnoredTests", "2 ignored tests")
        {            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = GetTestMessages(messages).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 4, out result))
            {
                return result;
            }

            var testStarted1 = testMessages[0];
            var testIgnored1 = testMessages[1];
            var testStarted2 = testMessages[2];
            var testIgnored2 = testMessages[3];

            if (!CheckMessageType(testStarted1, ServiceMessageConstants.TestStartedMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testIgnored1, ServiceMessageConstants.TestIgnoredMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testStarted2, ServiceMessageConstants.TestStartedMessageName, out result))
            {
                return result;
            }

            if (!CheckMessageType(testIgnored2, ServiceMessageConstants.TestIgnoredMessageName, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted1, testIgnored1, out result))
            {
                return result;
            }

            if (!CheckPair(testStarted2, testIgnored2, out result))
            {
                return result;
            }
            
            return new ValidationResult(ValidationState.Valid, new Details());
        }
    }
}