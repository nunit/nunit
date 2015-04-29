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

using Moq;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core;

using Shouldly;

namespace NUnit.Integration.Tests.TeamCity.CoreTests
{
    public sealed class ServiceMessageStructureValidatorTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        // Case format: [message] [name] {flowId}
        [Test]
        [TestCase("", "Valid")]
        [TestCase("ss 1, ss 2, sf 2, sf 1", "Valid")]
        [TestCase("ss 1, ss 2, sf 1, sf 2", "NotValid")]
        [TestCase("ss 1", "NotValid")]
        [TestCase("sf 1", "NotValid")]
        [TestCase("ss 1, ss 2, sf 1", "NotValid")]
        [TestCase("ss 2, sf 1, sf 2", "NotValid")]
        [TestCase("ss 1, ss 2, sf 2, ss 3, ss 4, sf 4, sf 3, sf 1", "Valid")]
        [TestCase("ts 1, tf 1", "Valid")]
        [TestCase("ts 1, ti 1", "Valid")]
        [TestCase("ts 1, ti 1, tf 1", "NotValid")]
        [TestCase("ss 2, ts 1, ti 1, sf 2", "Valid")]
        [TestCase("ss 2, ts 1, tf 1, sf 2", "Valid")]
        [TestCase("ss 2, ts 1, tf 1", "NotValid")]
        [TestCase("ss 2, ts 1, sf 2", "NotValid")]
        [TestCase("ss 2, ts 1, sf 2, tf 1", "NotValid")]
        [TestCase("ts 1, ts 2, tf 1, tf 1", "NotValid")]
        [TestCase("ts 1 f1, ts 2 f2, tf 1 f1, tf 2 f2", "Valid")]
        [TestCase("ts 1 f1, ts 1 f2, tf 1 f1, tf 1 f2", "Valid")]
        [TestCase("ss 1 f1, ts 3 f3, ss 2 f2, sf 1 f1, sf 2 f2, tf 3 f3", "Valid")]
        public void ShouldCheckSuiteMessagesWhenAnyCases(string codes, string expectedValidationStateStr)
        {
            // Given
            var expectedValidationState = (ValidationState)Enum.Parse(typeof(ValidationState), expectedValidationStateStr);
            var instance = CreateInstance();
            var allCodes = codes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
            var messages = allCodes.Select(CreateMessage).ToList();

            // When
            var validationResult = instance.Validate(messages.Select(i => i.Object));

            // Then
            validationResult.State.ShouldBe(expectedValidationState);
        }

        private static Mock<IServiceMessage> CreateMessage(string code)
        {
            var items = code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
            if (items.Count < 2)
            {
                throw new InvalidOperationException(string.Format("Invalide test code \"{0}\"", code));
            }

            var message = items[0];
            var name = items[1];
            string flowId = null;
            if (items.Count > 2)
            {
                flowId = items[2];
            }

            switch (message.ToLowerInvariant())
            {
                case "ss":
                    return CreateMessage(ServiceMessageConstants.TestSuiteStartedMessageName, name, flowId);

                case "sf":
                    return CreateMessage(ServiceMessageConstants.TestSuiteFinishedMessageName, name, flowId);

                case "ts":
                    return CreateMessage(ServiceMessageConstants.TestStartedMessageName, name, flowId);

                case "tf":
                    return CreateMessage(ServiceMessageConstants.TestFinishedMessageName, name, flowId);

                case "ti":
                    return CreateMessage(ServiceMessageConstants.TestIgnoredMessageName, name, flowId);

                default:
                    throw new InvalidOperationException(string.Format("Invalide test code \"{0}\"", code));
            }
        }

        private static Mock<IServiceMessage> CreateMessage(string messageName, string nameAttr, string flowIdAttr)
        {
            var message = new Mock<IServiceMessage>();
            message.SetupGet(i => i.Name).Returns(messageName);

            var attrs = new List<string> { ServiceMessageConstants.MessageAttributeName };
            if (!string.IsNullOrEmpty(flowIdAttr))
            {
                attrs.Add("flowId");
            }

            message.SetupGet(i => i.Attributes).Returns(attrs.ToArray());
            
            // ReSharper disable once RedundantAssignment
            var nameAttrValue = nameAttr;
            message.Setup(i => i.TryGetAttribute(ServiceMessageConstants.MessageAttributeName, out nameAttrValue)).Returns(true);
            message.Setup(i => i.GetAttribute(ServiceMessageConstants.MessageAttributeName)).Returns(nameAttr);

            // ReSharper disable once RedundantAssignment
            var flowIdAttrValue = flowIdAttr;
            message.Setup(i => i.TryGetAttribute(ServiceMessageConstants.MessageAttributeFlowId, out flowIdAttrValue)).Returns(true);
            message.Setup(i => i.GetAttribute(ServiceMessageConstants.MessageAttributeFlowId)).Returns(flowIdAttr);
            
            return message;
        }

        private ServiceMessageStructureValidator CreateInstance()
        {
            var instance = new ServiceMessageStructureValidator();
            return instance;
        }
    }
}
