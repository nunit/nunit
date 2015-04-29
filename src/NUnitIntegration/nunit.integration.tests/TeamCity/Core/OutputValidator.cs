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

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class OutputValidator : IOutputValidator
    {
        public ValidationResult Validate(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var isSuccess = true;
            var details = new Details();
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

                details = details.Combine(messageValidationResult.Details, "\t");
            }

            if (!isSuccess)
            {
                return new ValidationResult(ValidationState.NotValid, details);
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

            details = details.Combine(structureValidationResult.Details, "\t");
            return new ValidationResult(isSuccess ? ValidationState.Valid : ValidationState.NotValid, details);
        }
    }
}
