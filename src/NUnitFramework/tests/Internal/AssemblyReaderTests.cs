// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class AssemblyReaderTests
    {
        private static readonly string THIS_ASSEMBLY_PATH = AssemblyHelper.GetAssemblyPath(Assembly.GetExecutingAssembly());
        private AssemblyReader rdr;

        [SetUp]
        public void CreateReader()
        {
            rdr = new AssemblyReader( this.GetType().Assembly );
        }

        [TearDown]
        public void DisposeReader()
        {
            if ( rdr != null )
                rdr.Dispose();

            rdr = null;
        }

        [Test]
        public void CreateFromPath()
        {
            Assert.That(new AssemblyReader(THIS_ASSEMBLY_PATH).AssemblyPath, Is.SamePath(THIS_ASSEMBLY_PATH));
        }

        [Test]
        public void CreateFromAssembly()
        {
            Assert.That(rdr.AssemblyPath, Is.SamePath(THIS_ASSEMBLY_PATH));
        }

        [Test]
        public void IsValidPeFile()
        {
            Assert.IsTrue( rdr.IsValidPeFile );
        }

        //TODO: Needs a non-PE file in the directory
        //[Test]
        //public void IsValidPeFile_Fails()
        //{
        //    string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        //    Assert.IsFalse( new AssemblyReader( configFile ).IsValidPeFile );
        //}

        [Test]
        public void IsDotNetFile()
        {
            Assert.IsTrue( rdr.IsDotNetFile );
        }

        [Test]
        public void ImageRuntimeVersion()
        {
            string runtimeVersion = rdr.ImageRuntimeVersion;

            StringAssert.StartsWith( "v", runtimeVersion );
            Version version = new Version( runtimeVersion.Substring( 1 ) );
            // This fails when we force running under a prior version
            // Assert.LessOrEqual( version, Environment.Version );
        }

    }
}
#endif
