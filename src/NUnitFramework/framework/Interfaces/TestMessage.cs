// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Diagnostics;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The <see cref="TestMessage"/> class holds a message sent by a test to all listeners
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class TestMessage
    {
        /// <summary>
        /// Construct with text, destination type and
        /// the name of the test that produced the message.
        /// </summary>
        /// <param name="destination">Destination of the message</param>
        /// <param name="text">Text to be sent</param>
        /// <param name="testId">ID of the test that produced the message</param>
        public TestMessage(string destination, string text, string testId)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Destination = destination;
            Message = text;
            TestId = testId;
        }

        /// <summary>
        /// Converts <see cref="TestMessage"/> object to string
        /// </summary>
        public override string ToString()
        {
            return Destination + ": " + Message;
        }

        /// <summary>
        /// The message to send to listeners
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The Destination of the message.
        /// </summary>
        public string Destination { get; }

        /// <summary>
        /// The ID of the test that sent the message
        /// </summary>
        public string TestId { get; }

        /// <summary>
        /// Returns the XML representation of the <see cref="TestMessage"/> object.
        /// </summary>
        public string ToXml()
        {
            TNode tnode = new TNode("test-message", Message, true);

            if (Destination != null)
                tnode.AddAttribute("destination", Destination);

            if (TestId != null)
                tnode.AddAttribute("testid", TestId);

            return tnode.OuterXml;
        }
    }
}
