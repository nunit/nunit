// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Framework.Internal
{
    internal class AttributeProviderWrapper<T> : ICustomAttributeProvider
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
            return GetCustomAttributes(attributeType, inherit).Any();
        }

        private static T[] GetFiltered(IEnumerable<object> attributes)
        {
            return attributes
                   .OfType<T>()
                   .ToArray();
        }
    }
}
