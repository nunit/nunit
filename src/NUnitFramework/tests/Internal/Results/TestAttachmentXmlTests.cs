// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

            TNode? attachmentsNode = xml.SelectSingleNode(AttachmentsXName);
            Assert.That(attachmentsNode, Is.Not.Null);
            var attachmentNodeList = attachmentsNode.SelectNodes(AttachmentXName);
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

            TNode? attachmentNode = xml.SelectSingleNode(AttachmentsXName)?.SelectSingleNode(AttachmentXName);
            Assert.That(attachmentNode, Is.Not.Null);

            var descriptionNode = attachmentNode.SelectSingleNode(DescriptionXName);
            Assert.That(descriptionNode, Has.Property(nameof(TNode.ValueIsCDATA)).True);
            Assert.That(descriptionNode, Has.Property(nameof(TNode.Value)).EqualTo("description"));
        }

        [Test]
        public void NoDescriptionWhenNull()
        {
            _result.AddTestAttachment(new TestAttachment("file.txt", null));
            var xml = _result.ToXml(false);

            TNode? attachmentNode = xml.SelectSingleNode(AttachmentsXName)?.SelectSingleNode(AttachmentXName);
            Assert.That(attachmentNode, Is.Not.Null);
            Assert.That(attachmentNode.ChildNodes, Has.Exactly(0).Property(nameof(TNode.Name)).EqualTo(DescriptionXName));
        }

        private void FakeMethod() { }
    }
}
