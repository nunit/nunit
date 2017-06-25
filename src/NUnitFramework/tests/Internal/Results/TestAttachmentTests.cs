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


using System.IO;
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
