using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core.Common
{
    public sealed class ServiceLocator : IServiceLocator
    {
        private static readonly ServiceLocator SharedRoot = new ServiceLocator();
        private readonly Dictionary<ServiceKey, Func<object>> _instanceProviders = new Dictionary<ServiceKey, Func<object>>();
        
        private ServiceLocator()
        {            
        }
        
        public static IServiceLocator Root
        {
            get
            {
                return SharedRoot;
            }
        }

        [NotNull] public T GetService<T>([CanBeNull] string serviceName = null)
        {
            Contract.Ensures(!Equals(Contract.Result<T>(), null));

            Func<object> instanceProvider;
            if (!_instanceProviders.TryGetValue(new ServiceKey(typeof(T), serviceName), out instanceProvider))
            {
                throw new InvalidOperationException(string.Format("Type mapping for \"{0}\" is not registered", typeof(T)));
            }
            
            return (T)instanceProvider();
        }

        [NotNull] public IEnumerable<T> GetAllServices<T>()
        {
            Contract.Ensures(!Equals(Contract.Result<IEnumerable<T>>(), null));
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return from provider in _instanceProviders 
                   where provider.Key.ServiceType == typeof(T)
                   select (T)provider.Value();
        }

        public IDisposable AddMapping<T>([NotNull] Func<T> mapping, [CanBeNull] string serviceName = null)
        {
            Contract.Requires<ArgumentNullException>(mapping != null);

            var serviceKey = new ServiceKey(typeof(T), serviceName);
            _instanceProviders.Add(new ServiceKey(typeof(T), serviceName), () => (object)mapping());
            return Disposable.Create(() => _instanceProviders.Remove(serviceKey));
        }

        public IDisposable RegisterExtension(IServiceLocatorConfigurationExtension configurationExtension)
        {
            Contract.Requires<ArgumentNullException>(configurationExtension != null);

            return configurationExtension.Initialize(this);
        }

        private struct ServiceKey 
        {
            [CanBeNull] private readonly string _serviceName;

            public ServiceKey([NotNull] Type serviceType, [CanBeNull] string serviceName = null)
                : this()
            {
                Contract.Requires<ArgumentNullException>(serviceType != null);
                ServiceType = serviceType;
                _serviceName = serviceName;
            }

            public Type ServiceType { [NotNull] get; private set; }
        }
    }
}