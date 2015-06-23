﻿// ***********************************************************************
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
        private AppDomain _domain;

        [SetUp]
        public void CreateDomainManagerAndDomain()
        {
            var context = new ServiceContext();
            _domainManager = new DomainManager();
            context.Add(_domainManager);
            context.ServiceManager.StartServices();

            _domain = _domainManager.CreateDomain(new TestPackage(MockAssembly.AssemblyPath));
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_domainManager.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [Test, Platform("Linux,Net", Reason = "get_SetupInformation() fails on Windows+Mono")]
        public void CanCreateDomain()
        {
            Assert.NotNull(_domain);
            var setup = _domain.SetupInformation;

            Assert.That(setup.ApplicationName, Does.StartWith("Tests_"));
            Assert.That(setup.ApplicationBase, Is.SamePath(Path.GetDirectoryName(MockAssembly.AssemblyPath)), "ApplicationBase");
            Assert.That(
                Path.GetFileName(setup.ConfigurationFile),
                Is.EqualTo("mock-nunit-assembly.exe.config").IgnoreCase,
                "ConfigurationFile");
            Assert.AreEqual(null, setup.PrivateBinPath, "PrivateBinPath");
            Assert.That(setup.ShadowCopyFiles, Is.Null.Or.EqualTo("false"));
            //Assert.That(setup.ShadowCopyDirectories, Is.SamePath(Path.GetDirectoryName(MockAssembly.AssemblyPath)), "ShadowCopyDirectories" );
        }

        [Test]
        public void CanUnloadDomain()
        {
            _domainManager.Unload(_domain);

            CheckDomainIsUnloaded();
        }

        [Test]
        public void UnloadingTwiceDoesNoHarm()
        {

            _domainManager.Unload(_domain);
            _domainManager.Unload(_domain);

            CheckDomainIsUnloaded();
        }

        [Test]
        [TestCase(@"file.config", @"file.dll", @"d:\", @"file.config")]
        [TestCase(@"file.config", @"", @"d:\", "file.config")]
        [TestCase(@"file.config", null, @"d:\", "file.config")]
        [TestCase(@"", @"c:\file.dll", @"d:\", @"c:\file.dll.config")]
        [TestCase(null, @"c:\file.dll", @"d:\", @"c:\file.dll.config")]
        [TestCase(null, @"c:\file.dll", @"", @"c:\file.dll.config")]
        [TestCase(null, @"c:\file.dll", null, @"c:\file.dll.config")]
        [TestCase(@"", @"file.dll", @"d:\", @"d:\file.dll.config")]
        [TestCase(@"", @"file.dll", @"", @"file.dll.config")]
        [TestCase(null, @"file.dll", null, @"file.dll.config")]
        [TestCase(null, @"", null, "")]
        [TestCase(null, null, null, "")]
        public void ShouldProvideConfigFileName(string settingsConfigFile, string testFile, string appBaseDir, string expectedConfigFileName)
        {
            settingsConfigFile = ConvertFileName(settingsConfigFile);
            testFile = ConvertFileName(testFile);
            appBaseDir = ConvertFileName(appBaseDir);
            expectedConfigFileName = ConvertFileName(expectedConfigFileName);

            var actualConfigFileName = DomainManager.TryGetConfigFileName(settingsConfigFile, testFile, appBaseDir);
            Assert.AreEqual(expectedConfigFileName, actualConfigFileName);
        }

        private string ConvertFileName(string fileName)
        {
            if (fileName == null)
            {
                return null;
            }

            return fileName.Replace('\\', Path.DirectorySeparatorChar);
        }

        #region Helper Methods

        private void CheckDomainIsUnloaded()
        {
            // HACK: Either the Assert will succeed or the
            // exception should be thrown.
            bool unloaded = false;

            try
            {
                unloaded = _domain.IsFinalizingForUnload();
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
