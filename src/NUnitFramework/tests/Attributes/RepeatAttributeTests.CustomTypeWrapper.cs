// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Attributes
{
    public partial class RepeatAttributeTests
    {
        private sealed class CustomTypeWrapper : ITypeInfo
        {
            private readonly ITypeInfo _baseInfo;
            private readonly Attribute[] _extraMethodAttributes;

            public CustomTypeWrapper(ITypeInfo baseInfo, Attribute[] extraMethodAttributes)
            {
                _baseInfo = baseInfo;
                _extraMethodAttributes = extraMethodAttributes;
            }

            public IMethodInfo[] GetMethods(BindingFlags flags)
            {
                return _baseInfo.GetMethods(flags)
                    .Select(info => new CustomMethodWrapper(info, _extraMethodAttributes))
                    .ToArray();
            }

            public Type Type => _baseInfo.Type;

            public ITypeInfo BaseType => _baseInfo.BaseType;

            public string Name => _baseInfo.Name;

            public string FullName => _baseInfo.FullName;

            public Assembly Assembly => _baseInfo.Assembly;

            public string Namespace => _baseInfo.Namespace;

            public bool IsAbstract => _baseInfo.IsAbstract;

            public bool IsGenericType => _baseInfo.IsGenericType;

            public bool ContainsGenericParameters => _baseInfo.ContainsGenericParameters;

            public bool IsGenericTypeDefinition => _baseInfo.IsGenericTypeDefinition;

            public bool IsSealed => _baseInfo.IsSealed;

            public bool IsStaticClass => _baseInfo.IsStaticClass;

            public object Construct(object[] args)
            {
                return _baseInfo.Construct(args);
            }

            public ConstructorInfo GetConstructor(Type[] argTypes)
            {
                return _baseInfo.GetConstructor(argTypes);
            }

            public T[] GetCustomAttributes<T>(bool inherit) where T : class
            {
                return _baseInfo.GetCustomAttributes<T>(inherit);
            }

            public string GetDisplayName()
            {
                return _baseInfo.GetDisplayName();
            }

            public string GetDisplayName(object[] args)
            {
                return _baseInfo.GetDisplayName(args);
            }

            public Type GetGenericTypeDefinition()
            {
                return _baseInfo.GetGenericTypeDefinition();
            }

            public bool HasConstructor(Type[] argTypes)
            {
                return _baseInfo.HasConstructor(argTypes);
            }

            public bool HasMethodWithAttribute(Type attrType)
            {
                return _baseInfo.HasMethodWithAttribute(attrType);
            }

            public bool IsDefined<T>(bool inherit) where T : class
            {
                return _baseInfo.IsDefined<T>(inherit);
            }

            public bool IsType(Type type)
            {
                return _baseInfo.IsType(type);
            }

            public ITypeInfo MakeGenericType(Type[] typeArgs)
            {
                return _baseInfo.MakeGenericType(typeArgs);
            }
        }
    }
}
