// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

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
        public TestMessage(string destination, string text, string? testId)
        {
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentNullException.ThrowIfNull(text);

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
        public string? TestId { get; }

        /// <summary>
        /// Returns the XML representation of the <see cref="TestMessage"/> object.
        /// </summary>
        public string ToXml()
        {
            using var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter, XmlExtensions.FragmentWriterSettings))
            {
                ToXml(writer);
            }
            return stringWriter.ToString();
        }

        internal void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("test-message");

            writer.WriteAttributeString("destination", Destination);

            if (TestId is not null)
                writer.WriteAttributeString("testid", TestId);

            writer.WriteCDataSafe(Message);

            writer.WriteEndElement();
        }
    }
}
