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
using NUnit.Engine.Services;
using NUnit.Engine.Tests.Assemblies;

namespace NUnit.Engine.Tests
{
    public class DomainManagerTests
    {
        static string path1 = TestPath("/test/bin/debug/test1.dll");
        static string path2 = TestPath("/test/bin/debug/test2.dll");
        static string path3 = TestPath("/test/utils/test3.dll");

        [Test]
        public void GetPrivateBinPath()
        {
            string[] assemblies = new string[] { path1, path2, path3 };

            Assert.AreEqual(
                TestPath("bin/debug") + Path.PathSeparator + TestPath("utils"),
                DomainManager.GetPrivateBinPath(TestPath("/test"), assemblies));
        }

        [Test]
        public void GetCommonAppBase_OneElement()
        {
            string[] assemblies = new string[] { path1 };

            Assert.AreEqual(
                TestPath("/test/bin/debug"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public void GetCommonAppBase_TwoElements_SameDirectory()
        {
            string[] assemblies = new string[] { path1, path2 };

            Assert.AreEqual(
                TestPath("/test/bin/debug"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public void GetCommonAppBase_TwoElements_DifferentDirectories()
        {
            string[] assemblies = new string[] { path1, path3 };

            Assert.AreEqual(
                TestPath("/test"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public void GetCommonAppBase_ThreeElements_DiferentDirectories()
        {
            string[] assemblies = new string[] { path1, path2, path3 };

            Assert.AreEqual(
                TestPath("/test"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public void UnloadUnloadedDomain()
        {
            AppDomain domain = AppDomain.CreateDomain("DomainManagerTests-domain");
            AppDomain.Unload(domain);

            DomainManager manager = new DomainManager();
            manager.Unload(domain);
        }

        [Test, Platform("Linux,Net", Reason = "get_SetupInformation() fails on Windows+Mono")]
		public void AppDomainSetUpCorrect()
		{
            ServiceContext context = new ServiceContext();
            context.Add(new SettingsService());
            context.Add(new DomainManager());

            string mockDll = MockAssembly.AssemblyPath;
            AppDomainSetup setup = context.DomainManager.CreateAppDomainSetup(new TestPackage(mockDll));

            Assert.That(setup.ApplicationName, Is.StringStarting("Tests_"));
			Assert.That(setup.ApplicationBase, Is.SamePath(Path.GetDirectoryName(mockDll)), "ApplicationBase");
			Assert.That( 
                Path.GetFileName( setup.ConfigurationFile ),
                Is.EqualTo("mock-assembly.dll.config").IgnoreCase,
                "ConfigurationFile");
			Assert.AreEqual( null, setup.PrivateBinPath, "PrivateBinPath" );
			Assert.That(setup.ShadowCopyDirectories, Is.SamePath(Path.GetDirectoryName(mockDll)), "ShadowCopyDirectories" );
        }

		/// <summary>
        /// Take a valid Linux filePath and make a valid windows filePath out of it
        /// if we are on Windows. Change slashes to backslashes and, if the
        /// filePath starts with a slash, add C: in front of it.
        /// </summary>
        private static string TestPath(string path)
        {
            if (Path.DirectorySeparatorChar != '/')
            {
                path = path.Replace('/', Path.DirectorySeparatorChar);
                if (path[0] == Path.DirectorySeparatorChar)
                    path = "C:" + path;
            }

            return path;
        }
    }
}
