using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

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
                    if (!HasAttributes(message, out result, "name"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestFinishedMessageName:
                    if (!HasAttributes(message, out result, "name", "duration"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestFailedMessageName:
                    if (!HasAttributes(message, out result, "name", "message"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestIgnoredMessageName:
                    if (!HasAttributes(message, out result, "name", "message"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestStdErrMessageName:
                    if (!HasAttributes(message, out result, "name", "out"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                case ServiceMessageConstants.TestStdOutMessageName:
                    if (!HasAttributes(message, out result, "name", "out"))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid);

                default:
                    return new ValidationResult(ValidationState.Unknown);
            }
        }

        private bool HasAttributes([NotNull] IServiceMessage message, out ValidationResult result, [NotNull] params string[] requiredAttributes)
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Requires<ArgumentNullException>(requiredAttributes != null);

            var missingAttributes = new List<string>();
            foreach (var attributeName in requiredAttributes)
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
