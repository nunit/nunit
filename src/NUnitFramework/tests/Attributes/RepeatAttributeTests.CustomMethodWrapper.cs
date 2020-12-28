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
        private sealed class CustomMethodWrapper : IMethodInfo
        {
            private readonly IMethodInfo _baseInfo;
            private readonly Attribute[] _extraAttributes;

            public CustomMethodWrapper(IMethodInfo baseInfo, Attribute[] extraAttributes)
            {
                _baseInfo = baseInfo;
                _extraAttributes = extraAttributes;
            }

            public T[] GetCustomAttributes<T>(bool inherit) where T : class
            {
                return _baseInfo.GetCustomAttributes<T>(inherit)
                    .Concat(_extraAttributes.OfType<T>())
                    .ToArray();
            }

            public ITypeInfo TypeInfo => _baseInfo.TypeInfo;

            public MethodInfo MethodInfo => _baseInfo.MethodInfo;

            public string Name => _baseInfo.Name;

            public bool IsAbstract => _baseInfo.IsAbstract;

            public bool IsPublic => _baseInfo.IsPublic;

            public bool IsStatic => _baseInfo.IsStatic;

            public bool ContainsGenericParameters => _baseInfo.ContainsGenericParameters;

            public bool IsGenericMethod => _baseInfo.IsGenericMethod;

            public bool IsGenericMethodDefinition => _baseInfo.IsGenericMethodDefinition;

            public ITypeInfo ReturnType => _baseInfo.ReturnType;

            public Type[] GetGenericArguments()
            {
                return _baseInfo.GetGenericArguments();
            }

            public IParameterInfo[] GetParameters()
            {
                return _baseInfo.GetParameters();
            }

            public object Invoke(object fixture, params object[] args)
            {
                return _baseInfo.Invoke(fixture, args);
            }

            public bool IsDefined<T>(bool inherit) where T : class
            {
                return _baseInfo.IsDefined<T>(inherit);
            }

            public IMethodInfo MakeGenericMethod(params Type[] typeArguments)
            {
                return _baseInfo.MakeGenericMethod(typeArguments);
            }
        }
    }
}
