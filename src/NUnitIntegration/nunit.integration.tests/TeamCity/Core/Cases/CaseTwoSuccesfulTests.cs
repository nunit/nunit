using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseTwoSuccesfulTests : CaseBase
    {
        public CaseTwoSuccesfulTests()
            : base(CertType.TestFramework, "TwoSuccesfulTests", "2 succesful tests")
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
            var testFinished1 = testMessages[1];
            var testStarted2 = testMessages[2];
            var testFinished2 = testMessages[3];

            if (!CheckNameOfPair(testStarted1, testFinished1, out result))
            {
                return result;
            }

            if (!CheckNameOfPair(testStarted2, testFinished2, out result))
            {
                return result;
            }
            
            return new ValidationResult(ValidationState.Valid);
        }
    }
}