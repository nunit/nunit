// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Diagnostics;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The TestMessage class holds a message sent by a test to all listeners
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class TestMessage
    {
        /// <summary>
        /// Construct with text, destination type and
        /// the name of the test that produced the message.
        /// </summary>
        /// <param name="destination">Destination of message</param>
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
        /// A message to send to listeners
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Destination of a message.
        /// </summary>
        public string Destination { get; }

        /// <summary>
        /// ID of the test that sent a message
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
