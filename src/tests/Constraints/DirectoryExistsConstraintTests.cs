// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if !NUNITLITE
#region Using Directives

using System;
using System.IO;
using NUnit.Framework.Assertions;
using NUnit.Framework.Constraints;

#endregion

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class DirectoryExistsConstraintTests
    {
        private DirectoryExistsConstraint _constraint;
        private string _goodDir;
        private const string GarbageDir = @"Z:\I\hope\this\is\garbage";

        [SetUp]
        public void SetUp()
        {
            _constraint = new DirectoryExistsConstraint();
            _goodDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        [Test]
        public void PassesWhenDirectoryInfoExists()
        {
            var actual = new DirectoryInfo(_goodDir);
            Assert.That(_constraint.ApplyTo(actual).IsSuccess);
        }

        [Test]
        public void PassesWhenStringExists()
        {
            Assert.That(_constraint.ApplyTo(_goodDir).IsSuccess);
        }

        [Test]
        public void FailsWhenDirectoryInfoDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo(new DirectoryInfo(GarbageDir)).Status == ConstraintStatus.Failure);
        }

        [Test]
        public void FailsWhenStringDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo(GarbageDir).Status == ConstraintStatus.Failure);
        }

        [Test]
        public void FailsWhenNotStringOrDirectoryInfo()
        {
            var ex = Assert.Throws<ArgumentException>(() => _constraint.ApplyTo(42));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a string or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenDirectoryInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((DirectoryInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or DirectoryInfo"));
        }

        [Test]
        public void FailsWhenStringIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => _constraint.ApplyTo(string.Empty));
            Assert.That(ex.Message, Is.StringStarting("The actual value cannot be an empty string"));
        }

    }
}
#endif