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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    public class RuntimeFrameworkServiceTests
    {
        private RuntimeFrameworkService _runtimeService;

        [SetUp]
        public void CreateServiceContext()
        {
            var services = new ServiceContext();
            _runtimeService = new RuntimeFrameworkService();
            services.Add(_runtimeService);
            services.ServiceManager.StartServices();
        }

        [TearDown]
        public void StopService()
        {
            _runtimeService.StopService();
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_runtimeService.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [TestCase("mock-assembly.exe", "2.0.50727", false)]
        [TestCase("net-2.0/mock-assembly.exe", "2.0.50727", false)]
        [TestCase("net-4.0/mock-assembly.exe", "4.0.30319", false)]
        // TODO: Change this case when the 4.0/4.5 bug is fixed
        [TestCase("net-4.5/mock-assembly.exe", "4.0.30319", false)]
        [TestCase("mock-cpp-clr-x64.dll", "4.0.30319", false)]
        [TestCase("mock-cpp-clr-x86.dll", "4.0.30319", true)]
        [TestCase("nunit-agent.exe", "2.0.50727", false)]
        [TestCase("nunit-agent-x86.exe", "2.0.50727", true)]
        // TODO: Make the following cases work correctly in case we want to use
        // the engine to run them in the future.
        [TestCase("netcf-3.5/mock-assembly.exe", "2.0.50727", false)]
        [TestCase("sl-5.0/mock-assembly.dll", "4.0.30319", false)]
        [TestCase("portable/mock-assembly.dll", "4.0.30319", false)]
        public void SelectRuntimeFramework(string assemblyName, string expectedVersion, bool runAsX86)
        {
            // Some files don't actually exist on our CI servers
            Assume.That(assemblyName, Does.Exist);
            var package = new TestPackage(assemblyName);

            var returnValue = _runtimeService.SelectRuntimeFramework(package);
            var framework = RuntimeFramework.Parse(returnValue);

            Assert.That(package.GetSetting("RuntimeFramework", ""), Is.EqualTo(returnValue));
            Assert.That(package.GetSetting("RunAsX86", false), Is.EqualTo(runAsX86));
            Assert.That(framework.ClrVersion.ToString(), Is.EqualTo(expectedVersion));
        }

        [Test]
        public void AvailableFrameworks()
        {
            var available = _runtimeService.AvailableRuntimes;
            Assert.That(available.Count, Is.GreaterThan(0));
            foreach (var framework in available)
                Console.WriteLine("Available: {0}", framework.DisplayName);
        }
    }
}
