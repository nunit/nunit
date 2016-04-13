// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
// ***********************************************************************using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
    public class DirectoryFinderTests
    {
        private DirectoryInfo _baseDir;

        [SetUp]
        public void InitializeBaseDir()
        {
            _baseDir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        }

        [TestCase("net-4.0", 1)]
        [TestCase("net-*", 4)]
        [TestCase("*/v2-tests", 1)]
        [TestCase("add*/v?-*", 1)]
        [TestCase("**/v2-tests", 1)]
        [TestCase("addins/**", 2)]
        [TestCase("addins/../net-*", 4)]
        [TestCase("addins/v2-tests/", 1)]
        [TestCase("addins//v2-tests/", 1)]
        [TestCase("addins/./v2-tests/", 1)]
        public void GetDirectories(string pattern, int count)
        {
            var dirList = DirectoryFinder.GetDirectories(_baseDir, pattern);
            Assert.That(dirList.Count, Is.EqualTo(count));
        }

        [TestCase("net-4.0/nunit.framework.dll", 1)]
        [TestCase("net-*/nunit.framework.dll", 4)]
        [TestCase("net-*/*.framework.dll", 4)]
        [TestCase("*/v2-tests/*.dll", 2)]
        [TestCase("add*/v?-*/*.dll", 2)]
        [TestCase("**/v2-tests/*.dll", 2)]
        [TestCase("addins/**/*.dll", 7)]
        [TestCase("addins/../net-*/nunit.framework.dll", 4)]
        public void GetFiles(string pattern, int count)
        {
            var files = DirectoryFinder.GetFiles(_baseDir, pattern);
            Assert.That(files.Count, Is.EqualTo(count));
        }
    }
}
