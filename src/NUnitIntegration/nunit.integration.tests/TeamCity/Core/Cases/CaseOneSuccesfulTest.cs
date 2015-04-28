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
            : base(CertType.TestFramework, "CaseOneSuccesfulTest", "One succesful test")
        {
            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var testMessages = messages.Where(i => i.Name == ServiceMessageConstants.TestStartedMessageName || i.Name == ServiceMessageConstants.TestFinishedMessageName).ToList();
            ValidationResult result;
            if (!CheckCount(testMessages, 2, out result))
            {
                return result;
            }
        
            return new ValidationResult(ValidationState.Valid);
        }
    }
}