using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface IServiceLocator
    {
        [NotNull] T GetService<T>([CanBeNull] string serviceName = null);

        [NotNull] IEnumerable<T> GetAllServices<T>();

        IDisposable AddMapping<T>([NotNull] Func<T> mapping, [CanBeNull] string serviceName = null);

        IDisposable RegisterExtension([NotNull] IServiceLocatorConfigurationExtension configurationExtension);
    }
}