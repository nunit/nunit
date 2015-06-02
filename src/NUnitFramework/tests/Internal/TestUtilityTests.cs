// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using NUnit.Framework.Attributes;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class TestUtilityTests
    {
        [Test]
        public void ShouldRetTestAssemblyWhenHasParentTestAssembly()
        {
            // Given
            var test = new TestDummy();
            var testAssembly = new TestAssembly("foo.dll");

            // When
            test.Parent = testAssembly;
            var actualTest = TestUtility.GetTestAssembly(test);

            // Then
            Assert.AreEqual(testAssembly, actualTest);
        }

        [Test]
        public void ShouldRetTestAssemblyWhenHasMoreParents()
        {
            // Given
            var test = new TestDummy();
            var test2 = new TestDummy();
            var test3 = new TestDummy();
            var testAssembly = new TestAssembly("foo.dll");

            // When
            test.Parent = test2;
            test2.Parent = testAssembly;
            testAssembly.Parent = test3;
            var actualTest = TestUtility.GetTestAssembly(test);

            // Then
            Assert.AreEqual(testAssembly, actualTest);
        }

        [Test]
        public void ShouldRetTestAssemblyWhenHasOnlyTestAssembly()
        {
            // Given
            var testAssembly = new TestAssembly("foo.dll");

            // When
            var actualTest = TestUtility.GetTestAssembly(testAssembly);

            // Then
            Assert.AreEqual(testAssembly, actualTest);
        }

        [Test]
        public void ShouldRetNullAsTestAssemblyWhenNullArg()
        {
            // Given
            var test = new TestDummy();
            var test2 = new TestDummy();
            var test3 = new TestDummy();
            test.Parent = test2;
            test2.Parent = test3;

            // When
            var actualTest = TestUtility.GetTestAssembly(null);

            // Then
            Assert.AreEqual(null, actualTest);
        }

        [Test]
        public void ShouldRetNullWhenHasNotTestAssemblyInTheParents()
        {
            // Given
            var test = new TestDummy();
            var test2 = new TestDummy();
            var test3 = new TestDummy();

            // When
            test.Parent = test2;
            test2.Parent = test3;
            var actualTest = TestUtility.GetTestAssembly(test);

            // Then
            Assert.AreEqual(null, actualTest);
        }
    }
}
