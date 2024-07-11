// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Attributes
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

            public T[] GetCustomAttributes<T>(bool inherit)
                where T : class
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

            public object? Invoke(object? fixture, params object?[]? args)
            {
                return _baseInfo.Invoke(fixture, args);
            }

            public bool IsDefined<T>(bool inherit)
                where T : class
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
