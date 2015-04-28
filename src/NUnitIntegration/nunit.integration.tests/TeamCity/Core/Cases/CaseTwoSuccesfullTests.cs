using System;
using System.Collections.Generic;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseTwoSuccesfullTests : CaseBase
    {
        public CaseTwoSuccesfullTests()
            : base(CertType.TestFramework, "TwoSuccesfullTests", "Two Succesfull Tests")
        {
            
        }

        protected override ValidationResult ValidateCase(IEnumerable<IServiceMessage> rawMessages)
        {
            Contract.Requires<ArgumentNullException>(rawMessages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);
        
            return new ValidationResult(ValidationState.Valid);
        }
    }
}