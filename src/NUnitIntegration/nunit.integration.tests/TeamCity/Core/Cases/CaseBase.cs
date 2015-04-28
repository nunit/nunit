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

        public ValidationResult Validate(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);

            var outputValidator = ServiceLocator.Root.GetService<IOutputValidator>();
            var validationResult = outputValidator.Validate(messages);
            var messagesStrs = Enumerable.Repeat("Messages:", 1).Concat(messages.Select(i => i.ToString()));
            if (validationResult.State == ValidationState.NotValid || validationResult.State == ValidationState.Unknown)
            {
                return new ValidationResult(validationResult.State, validationResult.Details.Concat(messagesStrs).ToArray());
            }

            var validationCaseResult = ValidateCase(messages);
            return new ValidationResult(validationCaseResult.State, validationResult.Details.Concat(validationCaseResult.Details).Concat(messagesStrs).ToArray());
        }

        [NotNull]
        protected abstract ValidationResult ValidateCase([NotNull] IEnumerable<IServiceMessage> messages);

        protected bool CheckCount(IEnumerable<IServiceMessage> messages, int expectedCount, out ValidationResult result)
        {
            var actualCount = messages.Count();
            if (messages.Count() != expectedCount)
            {
                result = new ValidationResult(ValidationState.NotValid, string.Format("Invalid count of messages, expected {0}, actual {1}", expectedCount, actualCount));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }
    }
}
