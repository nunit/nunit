using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Api.Tests
{
    public class ServiceLocatorTests
    {
        private ITestEngine testEngine;

        [SetUp]
        public void CreateEngine()
        {
            testEngine = new TestEngine();
            testEngine.InitializeServices(".", InternalTraceLevel.Off);
        }

        private void CheckAccessToService(Type serviceType)
        {
            object service = testEngine.Services.GetService(serviceType);
            Assert.NotNull(service, "GetService(Type) returned null");
            Assert.That(service, Is.InstanceOf(serviceType));
        }

        private void CheckAccessToService<T>() where T: class
        {
            T service = testEngine.Services.GetService<T>();
            Assert.NotNull(service, "GetService<T>() returned null");
        }

        [TestCase(typeof(ISettings))]
        [TestCase(typeof(ITestAgency))]
        public void CanAccessUserSettings(Type serviceType)
        {
            CheckAccessToService(serviceType);
        }
    }
}
