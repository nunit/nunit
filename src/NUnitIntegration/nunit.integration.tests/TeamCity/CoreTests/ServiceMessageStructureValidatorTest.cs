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
        public void ShouldCheckSuiteMessagesWhenAnyCases(string codes, string expectedValidationStateStr)
        {
            // Given
            var expectedValidationState = (ValidationState)Enum.Parse(typeof(ValidationState), expectedValidationStateStr);
            var instance = CreateInstance();
            var allCodes = codes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
            var messages = allCodes.Select(CreateSuiteMessage).ToList();

            // When
            var validationResult = instance.Validate(messages.Select(i => i.Object));

            // Then
            validationResult.State.ShouldBe(expectedValidationState);
        }

        private static Mock<IServiceMessage> CreateSuiteMessage(string code)
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
