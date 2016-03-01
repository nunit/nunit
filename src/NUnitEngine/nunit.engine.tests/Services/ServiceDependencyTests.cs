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
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    using Fakes;

    public class ServiceDependencyTests
    {
        private ServiceContext _services;

        [SetUp]
        public void CreateServiceContext()
        {
            _services = new ServiceContext();
        }

        [Test]
        public void RecentFilesService_SettingsServiceError()
        {
            var fake = new FakeSettingsService();
            fake.FailToStart = true;
            _services.Add(fake);
            var service = new RecentFilesService();
            _services.Add(service);
            ((IService)fake).StartService();
            service.StartService();
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Error));
        }

        [Test]
        public void RecentFilesService_SettingsServiceMissing()
        {
            var service = new RecentFilesService();
            _services.Add(service);
            service.StartService();
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Error));
        }

        [Test]
        public void DefaultTestRunnerFactory_ProjectServiceError()
        {
            var fake = new FakeProjectService();
            fake.FailToStart = true;
            _services.Add(fake);
            var service = new RecentFilesService();
            _services.Add(service);
            ((IService)fake).StartService();
            service.StartService();
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Error));
        }

        [Test]
        public void DefaultTestRunnerFactory_ProjectServiceMissing()
        {
            var service = new DefaultTestRunnerFactory();
            _services.Add(service);
            service.StartService();
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Error));
        }
    }
}
