using System;
using System.Collections.Generic;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class OutputValidator : IOutputValidator
    {
        public ValidationResult Validate(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var isSuccess = true;
            var details = new List<string>();            
            var validatedMessages = new List<IServiceMessage>();
            var messageValidator = ServiceLocator.Root.GetService<IServiceMessageValidator>();
            foreach (var message in messages)
            {
                var messageValidationResult = messageValidator.Validate(message);
                switch (messageValidationResult.State)
                {
                    case ValidationState.Valid:
                        validatedMessages.Add(message);
                        break;

                    case ValidationState.HasWarning:
                        details.Add(string.Format("Message \"{0}\" has warning(s)", message.Name));
                        validatedMessages.Add(message);
                        break;

                    case ValidationState.Unknown:
                        details.Add(string.Format("Message \"{0}\" is unknown", message.Name));
                        break;

                    case ValidationState.NotValid:
                        details.Add(string.Format("Message \"{0}\" is not valid", message.Name));
                        isSuccess = false;
                        break;
                }

                foreach (var validationDetail in messageValidationResult.Details)
                {
                    details.Add(string.Format("\t{0}", validationDetail));
                }
            }

            if (!isSuccess)
            {
                return new ValidationResult(ValidationState.NotValid, details.ToString());
            }

            var structureValidationResult = ServiceLocator.Root.GetService<IServiceMessageStructureValidator>().Validate(validatedMessages);
            switch (structureValidationResult.State)
            {
                case ValidationState.HasWarning:
                    details.Add("Message structure validation has warning(s)");
                    break;

                case ValidationState.Unknown:
                    details.Add("Message structure validation result is unknown");
                    break;

                case ValidationState.NotValid:
                    details.Add("Message structure validation has errors");
                    isSuccess = false;
                    break;
            }

            foreach (var validationDetail in structureValidationResult.Details)
            {
                details.Add(string.Format("\t{0}", validationDetail));
            }

            if (!isSuccess)
            {
                return new ValidationResult(ValidationState.NotValid, string.Join(Environment.NewLine, details));
            }

            return new ValidationResult(ValidationState.Valid, string.Join(Environment.NewLine, details));
        }
    }
}
