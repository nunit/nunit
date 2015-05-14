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
    using Fakes;

    public class ServiceManagerTests
    {
        private IService _settingsService;
        private ServiceManager _serviceManager;

#if NUNIT_ENGINE
        private IService _projectService;
#endif

        [SetUp]
        public void SetUp()
        {
            _serviceManager = new ServiceManager();

            _settingsService = new FakeSettingsService();
            _serviceManager.AddService(_settingsService);

#if NUNIT_ENGINE
            _projectService = new FakeProjectService();
            _serviceManager.AddService(_projectService);
#endif
        }

        [Test]
        public void InitializeServices()
        {
            _serviceManager.StartServices();

            IService service = _serviceManager.GetService(typeof(ISettings));
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Started));
#if NUNIT_ENGINE
            service = _serviceManager.GetService(typeof(IProjectService));
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Started));
#endif
        }

        [Test]
        public void InitializationFailure()
        {
            ((FakeSettingsService)_settingsService).FailToStart = true;
            Assert.That(() => _serviceManager.StartServices(), 
                Throws.InstanceOf<InvalidOperationException>().And.Message.Contains("FakeSettingsService"));
        }

        [Test]
        public void AccessServiceByClass()
        {
            IService service = _serviceManager.GetService(typeof(FakeSettingsService));
            Assert.That(service, Is.SameAs(_settingsService));
        }

        [Test]
        public void AccessServiceByInterface()
        {
            IService service = _serviceManager.GetService(typeof(ISettings));
            Assert.That(service, Is.SameAs(_settingsService));
        }
    }
}
