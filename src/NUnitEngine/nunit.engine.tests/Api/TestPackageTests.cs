﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.Engine.Tests.Api
{
    public class TestPackageTests_SingleAssembly
    {
        private TestPackage package;

        [SetUp]
        public void CreatePackage()
        {
            package = new TestPackage("test.dll");
        }

        [Test]
        public void AssemblyPathIsUsedAsFilePath()
        {
            Assert.AreEqual(Path.GetFullPath("test.dll"), package.FullName);
        }

        [Test]
        public void AssemblyPathIsIncludedInList()
        {
            Assert.AreEqual(
                new[] { Path.GetFullPath("test.dll") },
                package.TestFiles);
        }

        [Test]
        public void FileNameIsUsedAsPackageName()
        {
            Assert.That(package.Name, Is.EqualTo("test.dll"));
        }
    }

    public class TestPackageTests_MultipleAssemblies
    {
        private TestPackage package;

        [SetUp]
        public void CreatePackage()
        {
            package = new TestPackage(new[] { "test1.dll", "test2.dll", "test3.dll" });
        }

        [Test]
        public void PackageIsAnonymous()
        {
            Assert.Null(package.FullName);
        }

        [Test]
        public void AssemblyPathsAreIncludedInList()
        {
            var expectedAssemblies = new[] {
            Path.GetFullPath("test1.dll"),
            Path.GetFullPath("test2.dll"),
            Path.GetFullPath("test3.dll") };

            Assert.AreEqual(expectedAssemblies, package.TestFiles);
        }
    }
}
