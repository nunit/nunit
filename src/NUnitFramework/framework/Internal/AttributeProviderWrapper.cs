// ***********************************************************************
// Copyright (c) 2008-2021 Charlie Poole, Rob Prouse
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

            return filtered?.ToArray() ??
#if NETSTANDARD2_0_OR_GREATER
                Array.Empty<T>();
#else
                new T[0];
#endif
        }
    }
}
