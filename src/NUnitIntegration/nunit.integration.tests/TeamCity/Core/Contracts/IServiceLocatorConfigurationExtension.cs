using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System;

    using JetBrains.TeamCityCert.Tools.Common;

    public interface IServiceLocatorConfigurationExtension
    {
        IDisposable Initialize([NotNull] ServiceLocator serviceLocator);
    }
}