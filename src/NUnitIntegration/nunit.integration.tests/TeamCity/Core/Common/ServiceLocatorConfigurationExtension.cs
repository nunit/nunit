using System;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;
using NUnit.Integration.Tests.TeamCity.Core.TestFramework;

namespace NUnit.Integration.Tests.TeamCity.Core.Common
{
    public sealed class ServiceLocatorConfigurationExtension : IServiceLocatorConfigurationExtension
    {
        private static readonly Lazy<TestsRepository> TestsRepository = new Lazy<TestsRepository>(() => new TestsRepository());
        private static readonly Lazy<ICertEngine> CertEngine = new Lazy<ICertEngine>(() => new CertEngine());
        private static readonly Lazy<ICmdLineToolTest> TestFrameworkCmdLineToolTest1 = new Lazy<ICmdLineToolTest>(() => new CmdLineToolTestTwoSuccesfullTests());
        private static readonly Lazy<IServiceMessageReplacements> ServiceMessageReplacements = new Lazy<IServiceMessageReplacements>(() => new ServiceMessageReplacements());
        private static readonly Lazy<IServiceMessageParser> ServiceMessageParser = new Lazy<IServiceMessageParser>(() => new ServiceMessageParser());
        private static readonly Lazy<IServiceMessageValidator> ServiceMessageValidator = new Lazy<IServiceMessageValidator>(() => new ServiceMessageValidator());
        private static readonly Lazy<IServiceMessageStructureValidator> ServiceMessageStructureValidator = new Lazy<IServiceMessageStructureValidator>(() => new ServiceMessageStructureValidator());
        private static readonly Lazy<IOutputValidator> OutputValidator = new Lazy<IOutputValidator>(() => new OutputValidator());

        public IDisposable Initialize(ServiceLocator serviceLocator)
        {
            Contract.Requires<ArgumentNullException>(serviceLocator != null);

            return new CompositDisposable
            {
                serviceLocator.AddMapping<ICheckList>(() => TestsRepository.Value),
                serviceLocator.AddMapping<ITestsRepository>(() => TestsRepository.Value),
                serviceLocator.AddMapping(() => CertEngine.Value),
                serviceLocator.AddMapping(() => ServiceMessageReplacements.Value),
                serviceLocator.AddMapping(() => ServiceMessageParser.Value),
                serviceLocator.AddMapping(() => ServiceMessageValidator.Value),
                serviceLocator.AddMapping(() => ServiceMessageStructureValidator.Value),

                // TestFramework
                serviceLocator.AddMapping(() => TestFrameworkCmdLineToolTest1.Value, TestFrameworkCmdLineToolTest1.Value.CaseId)
            };
        }
    }
}
