// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Builders
{
    internal sealed class ProviderCache
    {
        private readonly Dictionary<Type, object> _instances = new();

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
