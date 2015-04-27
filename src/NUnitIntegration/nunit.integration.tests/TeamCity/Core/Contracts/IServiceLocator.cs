using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IServiceLocator
    {
        [NotNull] T GetService<T>([CanBeNull] string serviceName = null);

        [NotNull] IEnumerable<T> GetAllServices<T>();

        IDisposable AddMapping<T>([NotNull] Func<T> mapping, [CanBeNull] string serviceName = null);

        IDisposable RegisterExtension([NotNull] IServiceLocatorConfigurationExtension configurationExtension);
    }
}