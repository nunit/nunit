// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt


using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Results
{
    [TestFixture]
    public class TestAttachmentTests
    {
        private string _tempFilePath;

        [OneTimeSetUp]
        public void CreateTempFile()
        {
            _tempFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, TestAttachmentsTests.TempFileName);
            File.Create(_tempFilePath).Dispose();
        }

        [OneTimeTearDown]
        public void RemoveTempFile()
        {
            File.Delete(_tempFilePath);
        }

        [Test]
        public void FilePathAndAttachmentPassedThroughToTestResult()
        {
            var result = TestBuilder.RunTestCase(typeof(TestAttachmentsTests), nameof(TestAttachmentsTests.AttachmentWithDescription));
            Assert.That(result.TestAttachments, 
                Has.Exactly(1).Property(nameof(TestAttachment.FilePath)).EqualTo(_tempFilePath)
                .And.Property(nameof(TestAttachment.Description)).EqualTo(TestAttachmentsTests.Description));
        }

        [Test]
        public void RelativeFilePathsAreMadeAbsolute()
        {
            var result = TestBuilder.RunTestCase(typeof(TestAttachmentsTests), nameof(TestAttachmentsTests.AttachmentWithRelativePath));
            Assert.That(result.TestAttachments, Has.Exactly(1).Property(nameof(TestAttachment.FilePath)).EqualTo(_tempFilePath));
        }

        [Test]
        public void AttachmentWithNoDescriptionResolvesToNull()
        {
            var result = TestBuilder.RunTestCase(typeof(TestAttachmentsTests), nameof(TestAttachmentsTests.AttachmentWithNoDescription));
            Assert.That(result.TestAttachments, Has.Exactly(1).Property(nameof(TestAttachment.Description)).Null);
        }
    }
}
