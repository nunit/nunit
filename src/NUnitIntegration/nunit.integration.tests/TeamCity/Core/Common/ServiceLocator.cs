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