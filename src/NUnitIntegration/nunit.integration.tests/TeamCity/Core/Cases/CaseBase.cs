using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Cases
{
    internal abstract class CaseBase : ICase
    {
        private readonly CertType _certType;
        private readonly string _caseId;
        private readonly string _description;

        protected CaseBase(
            CertType certType, 
            [NotNull] string caseId,
            [NotNull] string description)
        {
            Contract.Requires<ArgumentNullException>(caseId != null);
            Contract.Requires<ArgumentNullException>(description != null);

            _certType = certType;
            _caseId = caseId;
            _description = description;
        }

        /// <summary>
        /// Gets the cert type.
        /// </summary>
        public CertType CertType
        {
            get { return _certType; }
        }

        /// <summary>
        /// Gets the case id.
        /// </summary>
        public string CaseId
        {
            get
            {
                return _caseId;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        public ValidationResult Validate(IEnumerable<IServiceMessage> rawMessages)
        {
            Contract.Requires<ArgumentNullException>(rawMessages != null);

            var outputValidator = ServiceLocator.Root.GetService<IOutputValidator>();
            var validationResult = outputValidator.Validate(rawMessages);
            if (validationResult.State == ValidationState.NotValid || validationResult.State == ValidationState.Unknown)
            {
                return validationResult;
            }

            var validationCaseResult = ValidateCase(rawMessages);
            return new ValidationResult(validationCaseResult.State, validationResult.Details.Concat(validationCaseResult.Details).ToArray());
        }

        [NotNull]
        protected abstract ValidationResult ValidateCase([NotNull] IEnumerable<IServiceMessage> rawMessages);
    }
}
