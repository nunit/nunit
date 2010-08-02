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
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Builders
{
    class ProviderReference
    {
        private Type providerType;
        private string providerName;
        private object[] providerArgs;

        public ProviderReference(Type providerType, string providerName)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");
            if (providerName == null)
                throw new ArgumentNullException("providerName");

            this.providerType = providerType;
            this.providerName = providerName;
        }

        public ProviderReference(Type providerType, object[] args, string providerName)
            : this(providerType, providerName)
        {
            this.providerArgs = args;
        }

        public string Name
        {
            get { return this.providerName; }
        }

        public IEnumerable GetInstance()
        {
                MemberInfo[] members = providerType.GetMember(
                    providerName,
                    MemberTypes.Field | MemberTypes.Method | MemberTypes.Property,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (members.Length == 0)
                    throw new Exception(string.Format(
                        "Unable to locate {0}.{1}", providerType.FullName, providerName));

                return (IEnumerable)GetProviderObjectFromMember(members[0]);
        }

        private object GetProviderObjectFromMember(MemberInfo member)
        {
            object providerObject = null;
            object instance = null;

            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo providerProperty = member as PropertyInfo;
                    MethodInfo getMethod = providerProperty.GetGetMethod(true);
                    if (!getMethod.IsStatic)
                        //instance = ProviderCache.GetInstanceOf(providerType);
                        instance = Reflect.Construct(providerType, providerArgs);
                    providerObject = providerProperty.GetValue(instance, null);
                    break;

                case MemberTypes.Method:
                    MethodInfo providerMethod = member as MethodInfo;
                    if (!providerMethod.IsStatic)
                        //instance = ProviderCache.GetInstanceOf(providerType);
                        instance = Reflect.Construct(providerType, providerArgs);
                    providerObject = providerMethod.Invoke(instance, null);
                    break;

                case MemberTypes.Field:
                    FieldInfo providerField = member as FieldInfo;
                    if (!providerField.IsStatic)
                        //instance = ProviderCache.GetInstanceOf(providerType);
                        instance = Reflect.Construct(providerType, providerArgs);
                    providerObject = providerField.GetValue(instance);
                    break;
            }

            return providerObject;
        }
    }

#if CLR_2_0 || CLR_4_0
    class ProviderList : System.Collections.Generic.List<ProviderReference> { }
#else
    class ProviderList : ArrayList { }
#endif
}
