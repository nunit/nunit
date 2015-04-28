using System;
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
            if (items.Count != 2)
            {
                throw new InvalidOperationException(string.Format("Invalide test code \"{0}\"", code));
            }

            switch (items[0].ToLowerInvariant())
            {
                case "ss":
                    return CreateMessageWithName(ServiceMessageConstants.TestSuiteStartedMessageName, items[1]);

                case "sf":
                    return CreateMessageWithName(ServiceMessageConstants.TestSuiteFinishedMessageName, items[1]);

                case "ts":
                    return CreateMessageWithName(ServiceMessageConstants.TestStartedMessageName, items[1]);

                case "tf":
                    return CreateMessageWithName(ServiceMessageConstants.TestFinishedMessageName, items[1]);

                case "ti":
                    return CreateMessageWithName(ServiceMessageConstants.TestIgnoredMessageName, items[1]);

                default:
                    throw new InvalidOperationException(string.Format("Invalide test code \"{0}\"", code));
            }
        }

        private static Mock<IServiceMessage> CreateMessageWithName(string name, string nameAttrValue)
        {
            var message = new Mock<IServiceMessage>();
            message.SetupGet(i => i.Name).Returns(name);
            message.SetupGet(i => i.Attributes).Returns(new[] { "name" });
            
            // ReSharper disable once RedundantAssignment
            string attValue = nameAttrValue;
            message.Setup(i => i.TryGetAttribute("name", out attValue)).Returns(true);
            message.Setup(i => i.GetAttribute("name")).Returns(nameAttrValue);
            return message;
        }

        private ServiceMessageStructureValidator CreateInstance()
        {
            var instance = new ServiceMessageStructureValidator();
            return instance;
        }
    }
}
