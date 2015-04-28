using System;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity
{
    [SetUpFixture]
    public sealed class TestSetUpClass
    {
        private IDisposable _disp;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            _disp = ServiceLocator.Root.RegisterExtension(new ServiceLocatorConfigurationExtension());
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            if (_disp == null)
            {
                return;
            }

            _disp.Dispose();
        }
    }
}
