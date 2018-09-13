// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The TestMessage class holds a unit of message from test
    /// </summary>
	public class TestMessage
	{
        /// <summary>
        /// Construct with text, destination type and
        /// the name of the test that produced the message.
        /// </summary>
        /// <param name="destination">Destination of message</param>
        /// <param name="text">Text to be sent</param>
        /// <param name="testId">Id of the test that produced the message</param>
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
            Text = text;
            TestId = testId;
        }

        /// <summary>
        /// Return string representation of the object for debugging
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return Destination + ": " + Text;
		}

        /// <summary>
        /// Get the text 
        /// </summary>
		public string Text { get; }

        /// <summary>
        /// Get the destination of the test that created the message
        /// </summary>
        public string Destination { get; }

        /// <summary>
        /// Get the id of the test that created the output
        /// </summary>
        public string TestId { get; }

        /// <summary>
        /// Convert the TestMessage object to an XML string
        /// </summary>
        public string ToXml()
        {
            TNode tnode = new TNode("test-message", Text, true);

            if (Destination != null)
                tnode.AddAttribute("destination", Destination);

            if (TestId != null)
                tnode.AddAttribute("testid", TestId);

            return tnode.OuterXml;
        }
    }
}
