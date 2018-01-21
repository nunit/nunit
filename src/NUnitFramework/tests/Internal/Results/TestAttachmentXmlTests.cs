// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public class TestAttachmentXmlTests
    {
        private const string AttachmentsXName = "attachments";
        private const string AttachmentXName = "attachment";
        private const string FilepathXName = "filePath";
        private const string DescriptionXName = "description";

        private TestResult _result;

        [SetUp]
        public void SetUp()
        {
            _result = TestUtilities.Fakes.GetTestMethod(this, nameof(FakeMethod)).MakeTestResult();
        }

        [Test]
        public void SingleAttachmentXmlAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", null));
            var xml = _result.ToXml(false);

            var attachmentsNode = xml.SelectSingleNode(AttachmentsXName);
            Assert.That(attachmentsNode, Is.Not.Null, $"{AttachmentsXName} node not found");

            var attachmentNode = attachmentsNode.SelectSingleNode(AttachmentXName);
            Assert.That(attachmentNode, Is.Not.Null, $"{AttachmentXName} node not found");

            var filePathNode = attachmentNode.SelectSingleNode(FilepathXName);
            Assert.That(filePathNode, Has.Property(nameof(TNode.Value)).EqualTo("file.txt"));
        }

        [Test]
        public void MultipleAttachmentXmlAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file1.txt", null));
            _result.AddTestAttachment(new TestAttachment("file2.txt", null));
            var xml = _result.ToXml(false);

            var attachmentNodeList = xml.SelectSingleNode(AttachmentsXName).SelectNodes(AttachmentXName);
            Assert.That(attachmentNodeList, Has.Count.EqualTo(2));

            var filePathsNodes = attachmentNodeList.Select(n => n.SelectSingleNode(FilepathXName)).ToList();
            Assert.That(filePathsNodes, Has.Exactly(1).Property(nameof(TNode.Value)).EqualTo("file1.txt"));
            Assert.That(filePathsNodes, Has.Exactly(1).Property(nameof(TNode.Value)).EqualTo("file2.txt"));
        }

        [Test]
        public void NoAttachmentsNodeIfListEmpty()
        {
            var xml = _result.ToXml(false);

            var attachmentsNode = xml.SelectSingleNode(AttachmentsXName);
            Assert.That(attachmentsNode, Is.Null, $"Unexpected {AttachmentsXName} node found");
        }

        [Test]
        public void DescriptionNodeAsExpected()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", "description"));
            var xml = _result.ToXml(false);

            var descriptionNode = xml.SelectSingleNode(AttachmentsXName).SelectSingleNode(AttachmentXName).SelectSingleNode(DescriptionXName);
            Assert.That(descriptionNode, Has.Property(nameof(TNode.ValueIsCDATA)).True);
            Assert.That(descriptionNode, Has.Property(nameof(TNode.Value)).EqualTo("description"));
        }

        [Test]
        public void NoDescriptionWhenNull()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", null));
            var xml = _result.ToXml(false);

            var attachmentNode = xml.SelectSingleNode(AttachmentsXName).SelectSingleNode(AttachmentXName);
            Assert.That(attachmentNode.ChildNodes, Has.Exactly(0).Property(nameof(TNode.Name)).EqualTo(DescriptionXName));
        }

        private void FakeMethod() { }
    }
}
