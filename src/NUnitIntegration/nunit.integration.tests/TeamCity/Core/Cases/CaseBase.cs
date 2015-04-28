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

        [NotNull]
        protected static IEnumerable<IServiceMessage> GetTestMessages([NotNull] IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<IEnumerable<IServiceMessage>>() != null);

            return messages.Where(
                i => 
                i.Name != ServiceMessageConstants.TestSuiteStartedMessageName 
                && i.Name != ServiceMessageConstants.TestSuiteFinishedMessageName
                && i.Name != ServiceMessageConstants.TestStdErrMessageName
                && i.Name != ServiceMessageConstants.TestStdOutMessageName).ToList();
        }

        [NotNull]
        protected string GetAtr(IServiceMessage message, string name, [NotNull] string defaultValue = "")
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(defaultValue != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string val;
            if (message.TryGetAttribute(name, out val))
            {
                return val;
            }

            return defaultValue;
        }

        protected bool CheckCount([NotNull] IEnumerable<IServiceMessage> messages, int expectedCount, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(messages != null);

            var actualCount = messages.Count();
            if (messages.Count() != expectedCount)
            {
                result = new ValidationResult(ValidationState.NotValid, string.Format("Invalid count of messages, expected {0}, actual {1}", expectedCount, actualCount));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }

        protected bool CheckNameOfPair([NotNull] IServiceMessage message1, [NotNull] IServiceMessage message2, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(message1 != null);
            Contract.Requires<ArgumentNullException>(message2 != null);

            var name1 = GetAtr(message1, "name");
            var name2 = GetAtr(message2, "name");
            if (name1 != name2)
            {
                result = new ValidationResult(ValidationState.NotValid, string.Format("Tests' name are not equal: {0} and {1}", name1, name2));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }
    }
}
