using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ServiceMessageValidator : IServiceMessageValidator
    {
        public ValidationResult Validate(IServiceMessage message)
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            ValidationResult result;
            switch (message.Name)
            {
                case ServiceMessageConstants.TestSuiteStartedMessageName:
                    if (!HasAttributes(message, out result, "name"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestSuiteFinishedMessageName:
                    if (!HasAttributes(message, out result, "name"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestStartedMessageName:
                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestFinishedMessageName:
                    return new ValidationResult(ValidationState.Valid);

                default:
                    return new ValidationResult(ValidationState.Unknown);
            }
        }

        private bool HasAttributes(IServiceMessage message, out ValidationResult result, params string[] attributeNames)
        {
            if (!attributeNames.Any() && message.Attributes.Any())
            {
                result = new ValidationResult(ValidationState.NotValid, "Message should not have any attributes");
                return false;
            }

            var missingAttributes = new List<string>();
            foreach (var attributeName in attributeNames)
            {
                if (!message.Attributes.Contains(attributeName))
                {
                    missingAttributes.Add(attributeName);                    
                }
            }

            if (missingAttributes.Any())
            {
                result = new ValidationResult(ValidationState.NotValid, string.Format("Message have no attributes: {0}", string.Join(" ", missingAttributes)));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }
    }
}
