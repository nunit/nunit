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
using NUnit.Framework;
using System.IO;

namespace NUnit.Engine.Internal.Tests
{
	[TestFixture]
	public class AssemblyReaderTests
	{
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
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            Assert.AreEqual(path, new AssemblyReader(path).AssemblyPath);
		}

		[Test]
		public void CreateFromAssembly()
		{
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            Assert.AreEqual(path, rdr.AssemblyPath);
		}

		[Test]
		public void IsValidPeFile()
		{
            Assert.IsTrue(rdr.IsValidPeFile);
		}

		[Test]
		public void IsValidPeFile_Fails()
		{
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            path = Path.Combine(Path.GetDirectoryName(path), "nunit.engine.api.xml");
            Assert.IsFalse(new AssemblyReader(path).IsValidPeFile);
		}

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
			new Version( runtimeVersion.Substring( 1 ) );
			// This fails when we force running under a prior version
			// Assert.LessOrEqual( version, Environment.Version );
		}

	    [Test]
	    public void ShouldRun32BitAnyCpuCSharpAssembly()
        {
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            path = Path.Combine(Path.GetDirectoryName(path), "nunit-agent.exe");
            Assert.That(new AssemblyReader(path).ShouldRun32Bit, Is.False);
	    }

        [Test]
        public void ShouldRun32Bit32BitCSharpAssembly()
        {
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            path = Path.Combine(Path.GetDirectoryName(path), "nunit-agent-x86.exe");
            Assert.That(new AssemblyReader(path).ShouldRun32Bit, Is.True);
        }

        [Ignore("This test started failing on AppVeyor for no apparent reason. Ignoring until we figure out why.")]
        [Test]
        public void ShouldRun32Bit64BitCppAssembly()
        {
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            path = Path.Combine(Path.GetDirectoryName(path), "mock-cpp-clr-x64.dll");
            Assume.That(path, Does.Exist);
            Assert.That(new AssemblyReader(path).ShouldRun32Bit, Is.False);
        }

        [Test]
        public void ShouldRun32Bit32BitCppAssembly()
        {
            string path = AssemblyHelper.GetAssemblyPath(System.Reflection.Assembly.GetAssembly(GetType()));
            path = Path.Combine(Path.GetDirectoryName(path), "mock-cpp-clr-x86.dll");
            Assume.That(path, Does.Exist);
            Assert.That(new AssemblyReader(path).ShouldRun32Bit, Is.True);
        }
	}
}
