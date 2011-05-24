// ***********************************************************************
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

namespace NUnit.Engine.Api.Tests
{
    public class TestPackageTests
    {
        [Test]
        public void CreateWithSingleFile()
        {
            TestPackage package = new TestPackage("test.dll");
            string[] expected = new string[] { Path.GetFullPath("test.dll") };

            Assert.That(package.Name, Is.EqualTo("test.dll"));
            Assert.That(package.FullName, Is.EqualTo(Path.GetFullPath("test.dll")));
            Assert.That(package.TestFiles, Is.EqualTo(expected));

            Assert.That(package.Settings.Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateWithMultipleFiles()
        {
            TestPackage package = new TestPackage(new string[] {"test1.dll", "test2.dll", "test3.dll"});
            string[] expected = new string[] { 
                Path.GetFullPath("test1.dll"),
                Path.GetFullPath("test2.dll"),
                Path.GetFullPath("test3.dll")
            };

            Assert.That(package.Name, Is.EqualTo("test1.dll"));
            Assert.That(package.FullName, Is.EqualTo(Path.GetFullPath("test1.dll")));
            Assert.That(package.TestFiles, Is.EqualTo(expected));

            Assert.That(package.Settings.Count, Is.EqualTo(0));
        }
    }
}
