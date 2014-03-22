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
using NUnit.Framework;
using NUnit.Framework.Assertions;
using NUnit.Framework.Constraints;

#endregion

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class FileExistsConstraintTests
    {
        private FileExistsConstraint _constraint;

        [SetUp]
        public void SetUp()
        {
            _constraint = new FileExistsConstraint();    
        }

        [Test]
        public void PassesWhenFileInfoExists()
        {
            using(new TestFile("Test1.txt", "TestText1.txt"))
            {
                var actual = new FileInfo("Test1.txt");
                Assert.That(_constraint.ApplyTo(actual).IsSuccess);
            }
        }

        [Test]
        public void PassesWhenStringExists()
        {
            using(new TestFile("Test1.txt", "TestText1.txt"))
            {
                Assert.That(_constraint.ApplyTo("Test1.txt").IsSuccess);
            }
        }

        [Test]
        public void FailsWhenFileInfoDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo(new FileInfo("Garbage.txt")).Status == ConstraintStatus.Failure);
        }

        [Test]
        public void FailsWhenStringDoesNotExist()
        {
            Assert.That(_constraint.ApplyTo("Garbage.txt").Status == ConstraintStatus.Failure);
        }

        [Test]
        public void FailsWhenFileInfoIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((FileInfo)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
        }

        [Test]
        public void FailsWhenStringIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _constraint.ApplyTo((string)null));
            Assert.That(ex.Message, Is.StringStarting("The actual value must be a non-null string or FileInfo"));
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
