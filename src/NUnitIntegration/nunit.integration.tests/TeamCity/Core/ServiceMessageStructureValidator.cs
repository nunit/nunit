using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ServiceMessageStructureValidator : IServiceMessageStructureValidator
    {
        public ValidationResult Validate(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            var messageStack = new Stack<IServiceMessage>();
            foreach (var curMessage in messages)
            {
                string nameAttribute;
                var hasNameAttribute = curMessage.TryGetAttribute(ServiceMessageConstants.MessageAttributeName, out nameAttribute) && !string.IsNullOrEmpty(nameAttribute);
                var isStackPushMessage = 
                    curMessage.Name == ServiceMessageConstants.TestSuiteStartedMessageName
                    || curMessage.Name == ServiceMessageConstants.TestStartedMessageName;

                if (isStackPushMessage && hasNameAttribute)
                {
                    messageStack.Push(curMessage);
                    continue;
                }

                var isStackPopMessage =
                    curMessage.Name == ServiceMessageConstants.TestSuiteFinishedMessageName
                    || curMessage.Name == ServiceMessageConstants.TestFinishedMessageName;

                if (isStackPopMessage && hasNameAttribute)
                {
                    if (messageStack.Count == 0)
                    {
                        return new ValidationResult(ValidationState.NotValid);
                    }

                    var prevMessage = messageStack.Pop();

                    var prevName = prevMessage.Name.Replace("Started", string.Empty);
                    var curName = curMessage.Name.Replace("Finished", string.Empty);
                    if (prevName != curName)
                    {
                        return new ValidationResult(ValidationState.NotValid, string.Format("Message \"{0}\" has no corresponding message for start", curMessage));
                    }

                    var prevNameAttr = prevMessage.GetAttribute(ServiceMessageConstants.MessageAttributeName);
                    var curNameAttr = curMessage.GetAttribute(ServiceMessageConstants.MessageAttributeName);
                    if (prevNameAttr != curNameAttr)
                    {
                        return new ValidationResult(ValidationState.NotValid, string.Format("Message \"{0}\" has no corresponding message for start with valid name", curMessage.Name));
                    }
                }
            }

            if (messageStack.Count > 0)
            {
                return new ValidationResult(
                    ValidationState.NotValid, 
                    messageStack.Select(curMessage => string.Format("Message \"{0}\" has no corresponding message for finish", curMessage)).ToArray());
            }

            return new ValidationResult(ValidationState.Valid);
        }
    }
}
