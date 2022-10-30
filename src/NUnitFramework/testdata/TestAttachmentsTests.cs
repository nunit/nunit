// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData
{
    [TestFixture]
    public class TestAttachmentsTests
    {
        public const string TempFileName = "TestAttachmentsTests.tmp";
        public const string Description = "Description for attachment";

        [Test]
        public void AttachmentWithRelativePath()
        {
            TestContext.AddTestAttachment(TempFileName);
        }

        [Test]
        public void AttachmentWithNoDescription()
        {
            TestContext.AddTestAttachment(TempFileName);
        }

        [Test]
        public void AttachmentWithDescription()
        {
            TestContext.AddTestAttachment(TempFileName, Description);
        }
    }
}
