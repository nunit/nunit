// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
                AddCaseMapping<CaseOneSuccesfulTest>(serviceLocator),
                AddCaseMapping<CaseTwoSuccesfulTests>(serviceLocator),
                AddCaseMapping<CaseOneFailedTest>(serviceLocator),
                AddCaseMapping<CaseTwoFailedTests>(serviceLocator),
                AddCaseMapping<CaseOneIgnoredTest>(serviceLocator),
                AddCaseMapping<CaseTwoIgnoredTests>(serviceLocator),
                AddCaseMapping<CaseSuccesfulIgnoredFailedTests>(serviceLocator),
                AddCaseMapping<CaseMultithreadingTests>(serviceLocator),
            };
        }

        private IDisposable AddCaseMapping<TCase>(IServiceLocator serviceLocator)
            where TCase : ICase, new()
        {
            var @case = new TCase();
            return serviceLocator.AddMapping<ICase>(() => @case, @case.CaseId);
        }
    }
}
