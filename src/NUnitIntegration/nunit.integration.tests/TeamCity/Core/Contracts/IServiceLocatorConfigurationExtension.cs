using System;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface IServiceLocatorConfigurationExtension
    {
        IDisposable Initialize([NotNull] IServiceLocator serviceLocator);
    }
}