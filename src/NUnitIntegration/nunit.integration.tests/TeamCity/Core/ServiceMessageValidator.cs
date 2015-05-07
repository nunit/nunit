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
using System.Globalization;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

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
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestSuiteFinishedMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestStartedMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName))
                    {
                        return result;
                    }

                    if (!CheckAttr(
                            message, 
                            ServiceMessageConstants.MessageAttributeCaptureStandardOutput, 
                            out result, 
                            val =>
                            {
                                if (StringComparer.InvariantCultureIgnoreCase.Compare(val, "true") != 0 
                                    && StringComparer.InvariantCultureIgnoreCase.Compare(val, "false") != 0)
                                {
                                    return "value should be \"true\" or \"false\"";
                                }

                                return string.Empty;
                            }))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestFinishedMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName, ServiceMessageConstants.MessageAttributeDuration))
                    {
                        return result;
                    }

                    if (!CheckAttr(
                            message,
                            ServiceMessageConstants.MessageAttributeDuration,
                            out result,
                            val =>
                            {
                                double doubleVal;
                                if (!double.TryParse(val, NumberStyles.Number, CultureInfo.InvariantCulture, out doubleVal))
                                {
                                    return "value should be numeric";
                                }

                                return string.Empty;
                            }))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestFailedMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName, ServiceMessageConstants.MessageAttributeMessage, ServiceMessageConstants.MessageAttributeDetails))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestIgnoredMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName, ServiceMessageConstants.MessageAttributeMessage))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestStdErrMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName, ServiceMessageConstants.MessageAttributeOut))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                case ServiceMessageConstants.TestStdOutMessageName:
                    if (!HasAttributes(message, out result, ServiceMessageConstants.MessageAttributeName, ServiceMessageConstants.MessageAttributeOut))
                    {
                        return result;
                    }

                    return new ValidationResult(ValidationState.Valid, new Details());

                default:
                    return new ValidationResult(ValidationState.Unknown, new Details());
            }
        }

        private static bool HasAttributes([NotNull] IServiceMessage message, out ValidationResult result, [NotNull] params string[] requiredAttributes)
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
                result = new ValidationResult(ValidationState.NotValid, new Details(string.Format("Message {0} has no attributes: {1}", message, string.Join(" ", missingAttributes))));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }

        private static bool CheckAttr([NotNull] IServiceMessage message, [NotNull] string attrName, out ValidationResult result, [NotNull] Func<string, string> attrValueChecker, bool required = true)
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Requires<ArgumentNullException>(attrName != null);
            Contract.Requires<ArgumentNullException>(attrValueChecker != null);

            var attrValue = message.GetAttribute(attrName);
            var hasAttr = !string.IsNullOrWhiteSpace(attrValue);
            if (!hasAttr && required)
            {
                result = new ValidationResult(ValidationState.NotValid, new Details(string.Format("Message {0} has no required attribute: \"{1}\"", message, attrName)));
                return false;
            }

            if (!hasAttr)
            {
                result = default(ValidationResult);
                return true;
            }


            var errorInfo = attrValueChecker(attrValue);
            if (!string.IsNullOrWhiteSpace(errorInfo))
            {
                result = new ValidationResult(ValidationState.NotValid, new Details(string.Format("Message {0} has invalid attribute \"{1}\": {2}", message, attrName, errorInfo)));
                return false;
            }

            result = default(ValidationResult);
            return true;
        }
    }
}
