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
using System.Collections;
using System.IO;
using NUnit.Framework.Assertions;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class EmptyConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new EmptyConstraint();
            ExpectedDescription = "<empty>";
            StringRepresentation = "<empty>";
        }

        static object[] SuccessData = new object[]
        {
            string.Empty,
            new object[0],
#if !NETCOREAPP1_1
            new ArrayList(),
#endif
            new System.Collections.Generic.List<int>()
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( "Hello", "\"Hello\"" ),
            new TestCaseData( new object[] { 1, 2, 3 }, "< 1, 2, 3 >" )
        };

        [TestCase(null)]
        [TestCase(5)]
        public void InvalidDataThrowsArgumentException(object data)
        {
            Assert.Throws<ArgumentException>(() => TheConstraint.ApplyTo(data));
        }

        [Test]
        public void NullStringGivesFailureResult()
        {
            string actual = null;
            var result = TheConstraint.ApplyTo(actual);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
        }

        [Test]
        public void NullArgumentExceptionMessageContainsTypeName()
        {
            int? testInput = null;
            Assert.That(() => TheConstraint.ApplyTo(testInput),
               Throws.ArgumentException.With.Message.Contains("System.Int32"));
        }
    }

    [TestFixture]
    public class EmptyStringConstraintTest : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new EmptyStringConstraint();
            ExpectedDescription = "<empty>";
            StringRepresentation = "<emptystring>";
        }

        static object[] SuccessData = new object[]
        {
            string.Empty
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( "Hello", "\"Hello\"" ),
            new TestCaseData( null, "null")
        };
    }

    [TestFixture]
    public class EmptyDirectoryConstraintTest
    {
        [Test]
        public void EmptyDirectory()
        {
            using (var testDir = new TestDirectory())
            {
                Assert.That(testDir.Directory, Is.Empty);
            }
        }

        [Test]
        public void NotEmptyDirectory()
        {
            using (var testDir = new TestDirectory())
            {
                File.Create(Path.Combine(testDir.Directory.FullName, "DUMMY.FILE")).Dispose();

                Assert.That(testDir.Directory, Is.Not.Empty);
            }
        }
    }
}
