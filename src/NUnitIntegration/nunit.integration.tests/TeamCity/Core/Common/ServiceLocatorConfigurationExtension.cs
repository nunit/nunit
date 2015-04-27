using System;

using NUnit.Integration.Tests.TeamCity.Core.Cases;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Common
{
    public sealed class ServiceLocatorConfigurationExtension : IServiceLocatorConfigurationExtension
    {
        private static readonly Lazy<CaseRepository> TestsRepository = new Lazy<CaseRepository>(() => new CaseRepository());
        private static readonly Lazy<ICertEngine> CertEngine = new Lazy<ICertEngine>(() => new CertEngine());
        private static readonly Lazy<IServiceMessageReplacements> ServiceMessageReplacements = new Lazy<IServiceMessageReplacements>(() => new ServiceMessageReplacements());
        private static readonly Lazy<IServiceMessageParser> ServiceMessageParser = new Lazy<IServiceMessageParser>(() => new ServiceMessageParser());
        private static readonly Lazy<IServiceMessageValidator> ServiceMessageValidator = new Lazy<IServiceMessageValidator>(() => new ServiceMessageValidator());
        private static readonly Lazy<IServiceMessageStructureValidator> ServiceMessageStructureValidator = new Lazy<IServiceMessageStructureValidator>(() => new ServiceMessageStructureValidator());
        private static readonly Lazy<IOutputValidator> OutputValidator = new Lazy<IOutputValidator>(() => new OutputValidator());
        private static readonly Lazy<IProcessManager> ProcessManager = new Lazy<IProcessManager>(() => new ProcessManager());


        private static readonly Lazy<ICase> CaseTwoSuccesfull = new Lazy<ICase>(() => new CaseTwoSuccesfullTests());

        public IDisposable Initialize(IServiceLocator serviceLocator)
        {
            Contract.Requires<ArgumentNullException>(serviceLocator != null);

            return new CompositDisposable
            {
                serviceLocator.AddMapping<ICheckList>(() => TestsRepository.Value),
                serviceLocator.AddMapping<ICaseRepository>(() => TestsRepository.Value),
                serviceLocator.AddMapping(() => CertEngine.Value),
                serviceLocator.AddMapping(() => ServiceMessageReplacements.Value),
                serviceLocator.AddMapping(() => ServiceMessageParser.Value),
                serviceLocator.AddMapping(() => ServiceMessageValidator.Value),
                serviceLocator.AddMapping(() => ServiceMessageStructureValidator.Value),
                serviceLocator.AddMapping(() => OutputValidator.Value),
                serviceLocator.AddMapping(() => ProcessManager.Value),

                // Cases
                serviceLocator.AddMapping(() => CaseTwoSuccesfull.Value, CaseTwoSuccesfull.Value.CaseId),
            };
        }
    }
}
