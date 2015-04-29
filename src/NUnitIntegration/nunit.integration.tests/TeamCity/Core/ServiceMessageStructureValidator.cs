using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ServiceMessageStructureValidator : IServiceMessageStructureValidator
    {
        public ValidationResult Validate(IEnumerable<IServiceMessage> messages)
        {
            Contract.Requires<ArgumentNullException>(messages != null);
            Contract.Ensures(Contract.Result<ValidationResult>() != null);

            // We take into account the presence of groups (flowId)
            var goupedByFlowIdMessages =
                from message in messages
                let flowId = GetFlowIdAttr(message)
                group message by flowId;

            var validationResults = (
                from goupedMessages in goupedByFlowIdMessages
                select new { FlowId = goupedMessages.Key, Result = ValidateInternal(goupedMessages) }).ToList();

            if (!validationResults.Any())
            {
                return new ValidationResult(ValidationState.Valid, new Details());
            }

            return validationResults.Aggregate(
                new ValidationResult(ValidationState.Valid, new Details()),
                (acc, next) =>
                    {
                        var curState = acc.State;
                        if (next.Result.State > acc.State)
                        {
                            curState = next.Result.State;
                        }

                        return new ValidationResult(curState, acc.Details.Combine(next.Result.Details));
                    });
        }

        [NotNull] 
        private static ValidationResult ValidateInternal([NotNull] IEnumerable<IServiceMessage> messages)
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
                    || curMessage.Name == ServiceMessageConstants.TestFinishedMessageName
                    || curMessage.Name == ServiceMessageConstants.TestIgnoredMessageName;

                if (isStackPopMessage && hasNameAttribute)
                {
                    if (messageStack.Count == 0)
                    {
                        return new ValidationResult(ValidationState.NotValid, new Details());
                    }

                    var prevMessage = messageStack.Pop();

                    var prevName = prevMessage.Name.Replace("Started", string.Empty);
                    var curName = curMessage.Name.Replace("Finished", string.Empty).Replace("Ignored", string.Empty);
                    if (prevName != curName)
                    {
                        return new ValidationResult(ValidationState.NotValid, new Details(string.Format("Message \"{0}\" has no corresponding message for start", curMessage)));
                    }

                    var prevNameAttr = prevMessage.GetAttribute(ServiceMessageConstants.MessageAttributeName);
                    var curNameAttr = curMessage.GetAttribute(ServiceMessageConstants.MessageAttributeName);
                    if (prevNameAttr != curNameAttr)
                    {
                        return new ValidationResult(ValidationState.NotValid, new Details(string.Format("Message \"{0}\" has no corresponding message for start with valid name", curMessage.Name)));
                    }
                }
            }

            if (messageStack.Count > 0)
            {
                return new ValidationResult(
                    ValidationState.NotValid,
                    new Details(messageStack.Select(curMessage => string.Format("Message \"{0}\" has no corresponding message for finish", curMessage))));
            }

            return new ValidationResult(ValidationState.Valid, new Details());
        }

        [NotNull]
        private static string GetFlowIdAttr([NotNull] IServiceMessage message)
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string flowId;
            if (message.TryGetAttribute(ServiceMessageConstants.MessageAttributeFlowId, out flowId))
            {
                return flowId ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
