// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
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
using NUnit.Tests.Assemblies;

namespace NUnit.Engine.Services.Tests
{
    public class DomainManagerTests
    {
        private DomainManager _domainManager;
        private TestPackage _package = new TestPackage(MockAssembly.AssemblyPath);

        [SetUp]
        public void CreateDomainManager()
        {
            var context = new ServiceContext();
            _domainManager = new DomainManager();
            context.Add(_domainManager);
            context.ServiceManager.StartServices();
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_domainManager.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [Test, Platform("Linux,Net", Reason = "get_SetupInformation() fails on Windows+Mono")]
        public void CanCreateDomain()
        {
            var domain = _domainManager.CreateDomain(_package);

            Assert.NotNull(domain);
            var setup = domain.SetupInformation;

            Assert.That(setup.ApplicationName, Does.StartWith("Tests_"));
            Assert.That(setup.ApplicationBase, Is.SamePath(Path.GetDirectoryName(MockAssembly.AssemblyPath)), "ApplicationBase");
            Assert.That(
                Path.GetFileName(setup.ConfigurationFile),
                Is.EqualTo("mock-assembly.exe.config").IgnoreCase,
                "ConfigurationFile");
            Assert.AreEqual(null, setup.PrivateBinPath, "PrivateBinPath");
            Assert.That(setup.ShadowCopyFiles, Is.Null.Or.EqualTo("false"));
            //Assert.That(setup.ShadowCopyDirectories, Is.SamePath(Path.GetDirectoryName(MockAssembly.AssemblyPath)), "ShadowCopyDirectories" );
        }

        [Test, Platform("Linux,Net", Reason = "get_SetupInformation() fails on Windows+Mono")]
        public void CanCreateDomainWithApplicationBaseSpecified()
        {
            string assemblyDir = Path.GetDirectoryName(_package.FullName);
            string basePath = Path.GetDirectoryName(Path.GetDirectoryName(assemblyDir));
            string relPath = assemblyDir.Substring(basePath.Length + 1);

            _package.Settings["BasePath"] = basePath;
            var domain = _domainManager.CreateDomain(_package);

            Assert.NotNull(domain);
            var setup = domain.SetupInformation;

            Assert.That(setup.ApplicationName, Does.StartWith("Tests_"));
            Assert.That(setup.ApplicationBase, Is.SamePath(basePath), "ApplicationBase");
            Assert.That(
                Path.GetFileName(setup.ConfigurationFile),
                Is.EqualTo("mock-assembly.exe.config").IgnoreCase,
                "ConfigurationFile");
            Assert.That(setup.PrivateBinPath, Is.SamePath(relPath), "PrivateBinPath");
            Assert.That(setup.ShadowCopyFiles, Is.Null.Or.EqualTo("false"));
        }

        [Test]
        public void CanUnloadDomain()
        {
            var domain = _domainManager.CreateDomain(_package);
            _domainManager.Unload(domain);

            CheckDomainIsUnloaded(domain);
        }

        [Test]
        public void UnloadingTwiceDoesNoHarm()
        {
            var domain = _domainManager.CreateDomain(_package);
            _domainManager.Unload(domain);
            _domainManager.Unload(domain);

            CheckDomainIsUnloaded(domain);
        }

        #region Helper Methods

        private void CheckDomainIsUnloaded(AppDomain domain)
        {
            // HACK: Either the Assert will succeed or the
            // exception should be thrown.
            bool unloaded = false;

            try
            {
                unloaded = domain.IsFinalizingForUnload();
            }
            catch (AppDomainUnloadedException)
            {
                unloaded = true;
            }

            Assert.True(unloaded, "Domain was not unloaded");
        }

        #endregion
    }
}
