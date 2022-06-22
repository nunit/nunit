// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    internal sealed class AttributeProviderWrapper<T> : ICustomAttributeProvider
        where T : Attribute
    {
        private readonly ICustomAttributeProvider _innerProvider;

        public AttributeProviderWrapper(ICustomAttributeProvider innerProvider)
        {
            _innerProvider = innerProvider;
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            var attributes = _innerProvider.GetCustomAttributes(attributeType, inherit);
            return GetFiltered(attributes);
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            var attributes = _innerProvider.GetCustomAttributes(inherit);
            return GetFiltered(attributes);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        private static T[] GetFiltered(object[] attributes)
        {
            List<T> filtered = null;
            foreach (var attribute in attributes)
            {
                if (attribute is T t)
                {
                    filtered ??= new List<T>();
                    filtered.Add(t);
                }
            }

            return filtered?.ToArray() ?? Array.Empty<T>();
        }
    }
}
