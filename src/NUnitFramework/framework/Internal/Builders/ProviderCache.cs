// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal.Builders
{
    internal sealed class ProviderCache
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public object GetInstanceOf(Type providerType)
        {
            return GetInstanceOf(providerType, null);
        }

        public object GetInstanceOf(Type providerType, object[]? providerArgs)
        {
            if (!_instances.TryGetValue(providerType, out var instance))
                _instances.Add(providerType, instance = Reflect.Construct(providerType, providerArgs));

            return instance;
        }
    }
}
