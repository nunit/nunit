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

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core;

namespace NUnit.Integration.Tests.TeamCity.CoreTests
{
    public sealed class ServiceMessageStructureValidatorTest
    {
        // Case format: [message] [name] {flowId}
        // Where message is: ss - testSuiteStarted, sf - testSuiteFinished, ts - testStarted, tf - testFinished, i - testIgnored
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
            var actualValidationResult = instance.Validate(messages);

            // Then
            Assert.AreEqual(actualValidationResult.State, expectedValidationState);
        }

        [NotNull] 
        private static IServiceMessage CreateMessage([NotNull] string code)
        {
            Contract.Requires<ArgumentNullException>(code != null);
            Contract.Ensures(Contract.Result<IServiceMessage>() != null);

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

        [NotNull]
        private static IServiceMessage CreateMessage([NotNull] string messageName, [NotNull] string nameAttr, [CanBeNull] string flowIdAttr)
        {
            Contract.Requires<ArgumentNullException>(messageName != null);
            Contract.Requires<ArgumentNullException>(nameAttr != null);
            Contract.Ensures(Contract.Result<IServiceMessage>() != null);

            var attributes = new Dictionary<string, string>
            {
                { ServiceMessageConstants.MessageAttributeName, nameAttr }
            };

            if (!string.IsNullOrEmpty(flowIdAttr))
            {
                attributes.Add(ServiceMessageConstants.MessageAttributeFlowId, flowIdAttr);
            }

            return new ServiceMessageStub(messageName, attributes);
        }

        private ServiceMessageStructureValidator CreateInstance()
        {
            var instance = new ServiceMessageStructureValidator();
            return instance;
        }

        private sealed class ServiceMessageStub : IServiceMessage
        {
            private readonly IDictionary<string, string> _attributes;

            public ServiceMessageStub([NotNull] string name, [NotNull] IDictionary<string, string> attributes)
            {
                Contract.Requires<ArgumentNullException>(name != null);

                _attributes = attributes;
                Name = name;
            }

            /// <summary>
            /// Service message name, i.e. messageName in ##teamcity[messageName 'ddd']. 
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// For one-value service messages returns value, i.e. 'aaa' for ##teamcity[message 'aaa']
            /// or <code>null</code> otherwise, i.e. ##teamcity[message aa='aaa']
            /// </summary>
            public string DefaultValue
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Emptry for one-value service messages, i.e. ##teamcity[message 'aaa'], returns all keys otherwise
            /// </summary>
            public IEnumerable<string> Attributes
            {
                get
                {
                    Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                    return _attributes.Keys;
                }
            }

            public bool TryGetAttribute(string attributeName, out string value)
            {
                return _attributes.TryGetValue(attributeName, out value);
            }
        }
    }
}
