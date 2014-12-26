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

using System;
using System.IO;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
    [TestFixture]
    public class AssemblyHelperTests
    {
        private static readonly string THIS_ASSEMBLY_NAME = "nunit.engine.tests";
        private static readonly string THIS_ASSEMBLY_PATH = THIS_ASSEMBLY_NAME + ".dll";

        public void GetNameForAssembly()
        {
            var assemblyName = AssemblyHelper.GetAssemblyName(this.GetType().Assembly);
            Assert.That(assemblyName.Name, Is.EqualTo(THIS_ASSEMBLY_NAME).IgnoreCase);
            Assert.That(assemblyName.FullName, Is.EqualTo(THIS_ASSEMBLY_PATH).IgnoreCase);
        }

        [Test]
        public void GetPathForAssembly()
        {
            string path = AssemblyHelper.GetAssemblyPath(this.GetType().Assembly);
            Assert.That(Path.GetFileName(path), Is.EqualTo(THIS_ASSEMBLY_PATH).IgnoreCase);
            Assert.That(File.Exists(path));
        }

        [Test]
        public void GetPathForType()
        {
            string path = AssemblyHelper.GetAssemblyPath(this.GetType());
            Assert.That(Path.GetFileName(path), Is.EqualTo(THIS_ASSEMBLY_PATH).IgnoreCase);
            Assert.That(File.Exists(path));
        }

        [Test]
        public void GetDirectoryName()
        {
            string path = AssemblyHelper.GetDirectoryName(this.GetType().Assembly);
            Assert.That(File.Exists(Path.Combine(path, THIS_ASSEMBLY_PATH)));
        }
    }
}
