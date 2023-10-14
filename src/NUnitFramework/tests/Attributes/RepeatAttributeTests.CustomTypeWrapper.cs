// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Attributes
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

            public ITypeInfo? BaseType => _baseInfo.BaseType;

            public string Name => _baseInfo.Name;

            public string FullName => _baseInfo.FullName;

            public Assembly Assembly => _baseInfo.Assembly;

            public string? Namespace => _baseInfo.Namespace;

            public bool IsAbstract => _baseInfo.IsAbstract;

            public bool IsGenericType => _baseInfo.IsGenericType;

            public bool ContainsGenericParameters => _baseInfo.ContainsGenericParameters;

            public bool IsGenericTypeDefinition => _baseInfo.IsGenericTypeDefinition;

            public bool IsSealed => _baseInfo.IsSealed;

            public bool IsStaticClass => _baseInfo.IsStaticClass;

            public object Construct(object?[]? args)
            {
                return _baseInfo.Construct(args);
            }

            public ConstructorInfo? GetConstructor(Type[] argTypes)
            {
                return _baseInfo.GetConstructor(argTypes);
            }

            public T[] GetCustomAttributes<T>(bool inherit)
                where T : class
            {
                return _baseInfo.GetCustomAttributes<T>(inherit);
            }

            public string GetDisplayName()
            {
                return _baseInfo.GetDisplayName();
            }

            public string GetDisplayName(object?[]? args)
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

            public bool IsDefined<T>(bool inherit)
                where T : class
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

            public IMethodInfo[] GetMethodsWithAttribute<T>(bool inherit)
                where T : class
            {
                return _baseInfo.GetMethodsWithAttribute<T>(inherit);
            }
        }
    }
}
