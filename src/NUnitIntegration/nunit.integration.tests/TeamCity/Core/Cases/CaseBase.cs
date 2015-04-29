// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
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
            var messagesStrs = Enumerable.Repeat("Captured messages:", 1).Concat(messages.Select(i => i.ToString()));
            if (validationResult.State == ValidationState.NotValid || validationResult.State == ValidationState.Unknown)
            {
                return new ValidationResult(validationResult.State, validationResult.Details.Combine(messagesStrs));
            }

            var validationCaseResult = ValidateCase(messages);
            return new ValidationResult(validationCaseResult.State, validationResult.Details.Combine(validationCaseResult.Details).Combine(messagesStrs));
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

        protected static bool CheckMessageType([NotNull] IServiceMessage message, string expectedMessageType, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Requires<ArgumentNullException>(expectedMessageType != null);

            if (message.Name != expectedMessageType)
            {
                result = new ValidationResult(
                    ValidationState.NotValid,
                    new Details(string.Format("Message types are not equal for message {0}: expected \"{1}\", actual \"{2}\"", message, expectedMessageType, message.Name)));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }

        protected static bool CheckCount([NotNull] IEnumerable<IServiceMessage> messages, int expectedCount, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(messages != null);

            var actualCount = messages.Count();
            if (messages.Count() != expectedCount)
            {
                result = new ValidationResult(
                    ValidationState.NotValid, 
                    new Details(string.Format("Invalid count of messages, expected {0}, actual {1}", expectedCount, actualCount)));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }

        protected static bool CheckPair([NotNull] IServiceMessage message1, [NotNull] IServiceMessage message2, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(message1 != null);
            Contract.Requires<ArgumentNullException>(message2 != null);

            if (!CheckAttr(message1, message2, ServiceMessageConstants.MessageAttributeName, out result))
            {
                return false;
            }

            if (!CheckAttr(message1, message2, ServiceMessageConstants.MessageAttributeFlowId, out result))
            {
                return false;
            }

            result = default(ValidationResult);
            return true;
        }

        private static bool CheckAttr([NotNull] IServiceMessage message1, [NotNull] IServiceMessage message2, [NotNull] string attributeName, out ValidationResult result)
        {
            Contract.Requires<ArgumentNullException>(message1 != null);
            Contract.Requires<ArgumentNullException>(message2 != null);
            Contract.Requires<ArgumentNullException>(attributeName != null);

            var atr1 = message1.GetAttr(attributeName);
            var atr2 = message2.GetAttr(attributeName);
            if (atr1 != atr2)
            {
                result = new ValidationResult(
                    ValidationState.NotValid,
                    new Details(string.Format("Attributes \"{0}\" are not equal: \"{1}\" and \"{2}\"", attributeName, atr1, atr2)));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }
    }
}
