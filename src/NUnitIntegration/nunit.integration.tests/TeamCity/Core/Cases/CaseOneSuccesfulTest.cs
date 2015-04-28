using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseOneSuccesfulTest : CaseBase
    {
        public CaseOneSuccesfulTest()
            : base(CertType.TestFramework, "OneSuccesfulTest", "1 succesful test")
        {            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = GetTestMessages(messages).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 2, out result))
            {
                return result;
            }

            var testStarted1 = testMessages[0];
            var testFinished1 = testMessages[1];

            if (!CheckNameOfPair(testStarted1, testFinished1, out result))
            {
                return result;
            }

            return new ValidationResult(ValidationState.Valid);
        }
    }
}