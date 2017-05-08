// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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

using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Internal.Results
{
    public class TestResultAttachmentTests
    {
        private TestResult _result;

        [SetUp]
        public void SetUp()
        {
            _result = TestUtilities.Fakes.GetTestMethod(this, "FakeMethod").MakeTestResult();
        }

        [Test]
        public void SingleAttachmentXmlAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", null));

            var xml = _result.ToXml(false);

            var attachmentsNode = xml.SelectSingleNode("attachments");
            Assert.That(attachmentsNode, Is.Not.Null, "Attachments node not found");

            var attachmentNode = attachmentsNode.SelectSingleNode("attachment");
            Assert.That(attachmentNode, Is.Not.Null, "Attachments node not found");
            Assert.That(attachmentNode, Has.Property("Value").EqualTo("file.txt"));
        }

        [Test]
        public void MultipleAttachmentXmlAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file1.txt", null));
            _result.AddTestAttachment(new TestAttachment("file2.txt", null));

            var xml = _result.ToXml(false);

            var attachmentsNode = xml.SelectSingleNode("attachments");
            Assert.That(attachmentsNode, Is.Not.Null, "Attachments node not found");

            var attachmentNodeList = attachmentsNode.SelectNodes("attachment");
            Assert.That(attachmentNodeList, Has.Count.EqualTo(2));

            Assert.That(attachmentNodeList, Has.Exactly(1).Property("Value").EqualTo("file1.txt"));
            Assert.That(attachmentNodeList, Has.Exactly(1).Property("Value").EqualTo("file2.txt"));
        }

        [Test]
        public void NoAttachmentsNodeIfListEmpty()
        {
            var xml = _result.ToXml(false);

            var attachmentsNode = xml.SelectSingleNode("attachments");
            Assert.That(attachmentsNode, Is.Null, "Unexpected attachments node found");
        }

        [Test]
        public void DescriptionAttributeAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", "description"));

            var xml = _result.ToXml(false);

            var attachmentNode = xml.SelectSingleNode("attachments").SelectSingleNode("attachment");

            Assert.That(attachmentNode.Attributes["description"], Is.EqualTo("description"));
        }

        [Test]
        public void NoDescriptionWhenNull()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", null));
            var xml = _result.ToXml(false);

            var attachmentNode = xml.SelectSingleNode("attachments").SelectSingleNode("attachment");

            Assert.That(attachmentNode.Attributes, Does.Not.ContainKey("description"));
        }

        private void FakeMethod() { }
    }
}
