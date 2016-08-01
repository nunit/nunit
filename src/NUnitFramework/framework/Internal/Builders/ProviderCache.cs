// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Builders
{
    class ProviderCache
    {
        private static Dictionary<CacheEntry, object> instances = new Dictionary<CacheEntry, object>();

        public static object GetInstanceOf(Type providerType)
        {
            return GetInstanceOf(providerType, null);
        }

        public static object GetInstanceOf(Type providerType, object[] providerArgs)
        {
            CacheEntry entry = new CacheEntry(providerType, providerArgs);

            object instance = instances.ContainsKey(entry)
                ?instances[entry]
                : null;

            if (instance == null)
                instances[entry] = instance = Reflect.Construct(providerType, providerArgs);

            return instance;
        }

        public static void Clear()
        {
            foreach (CacheEntry key in instances.Keys)
            {
                IDisposable provider = instances[key] as IDisposable;
                if (provider != null)
                    provider.Dispose();
            }

            instances.Clear();
        }

        class CacheEntry
        {
            private Type providerType;

            public CacheEntry(Type providerType, object[] providerArgs)
            {
                this.providerType = providerType;
            }

            public override bool Equals(object obj)
            {
                CacheEntry other = obj as CacheEntry;
                if (other == null) return false;

                return this.providerType == other.providerType;
            }

            public override int GetHashCode()
            {
                return providerType.GetHashCode();
            }
        }
    }
}
