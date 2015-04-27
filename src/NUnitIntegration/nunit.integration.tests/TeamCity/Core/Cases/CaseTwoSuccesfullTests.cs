using System;
using System.Collections.Generic;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    /// <summary>
    /// The cmd line tool test two succesfull tests.
    /// </summary>
    internal class CaseTwoSuccesfullTests : ICase
    {
        /// <summary>
        /// Gets the cert type.
        /// </summary>
        public CertType CertType
        {
            get { return CertType.TestFramework; }
        }

        /// <summary>
        /// Gets the case id.
        /// </summary>
        public string CaseId 
        {
            get
            {
                return "TwoSuccesfullTests";
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description 
        {
            get
            {
                return "Two Succesfull Tests";
            }
        }

        public ValidationResult Validate(IEnumerable<string> output)
        {
            Contract.Requires<ArgumentNullException>(output != null);

            var outputValidator = ServiceLocator.Root.GetService<IOutputValidator>();
            var validationResult = outputValidator.Validate(output);

            return validationResult;
        }
    }
}