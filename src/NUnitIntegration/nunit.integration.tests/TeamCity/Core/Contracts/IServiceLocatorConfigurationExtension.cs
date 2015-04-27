using System;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface IServiceLocatorConfigurationExtension
    {
        IDisposable Initialize([NotNull] ServiceLocator serviceLocator);
    }
}