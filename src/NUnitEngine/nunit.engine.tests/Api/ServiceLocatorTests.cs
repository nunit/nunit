using System;
using NUnit.Framework;

namespace NUnit.Engine.Tests.Api
{
    public class ServiceLocatorTests
    {
        private ITestEngine testEngine;

        [SetUp]
        public void CreateEngine()
        {
            testEngine = new TestEngine {InternalTraceLevel = InternalTraceLevel.Off};
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
        [TestCase(typeof(IRecentFiles))]
        [TestCase(typeof(ITestAgency))]
        public void CanAccessUserSettings(Type serviceType)
        {
            CheckAccessToService(serviceType);
        }
    }
}
