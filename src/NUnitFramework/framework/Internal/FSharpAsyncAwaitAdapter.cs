// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    internal static class FSharpAsyncAwaitAdapter
    {
        private static MethodInfo? _startImmediateAsTaskMethod;

        public static bool IsAwaitable(Type awaitableType)
        {
            return GetAsyncInfo(awaitableType) is not null;
        }

        public static Type? GetResultType(Type awaitableType)
        {
            return GetAsyncInfo(awaitableType)?.ResultType;
        }

        private static AsyncInfo? GetAsyncInfo(Type asyncType)
        {
            if (asyncType is null)
                return null;

            if (!asyncType.IsGenericType)
                return null;
            var genericDefinition = asyncType.GetGenericTypeDefinition();
            if (genericDefinition.FullName != "Microsoft.FSharp.Control.FSharpAsync`1")
                return null;

            return new AsyncInfo(genericDefinition, asyncType.GetGenericArguments()[0]);
        }

        private sealed class AsyncInfo
        {
            public AsyncInfo(Type fSharpAsyncTypeDefinition, Type resultType)
            {
                FSharpAsyncTypeDefinition = fSharpAsyncTypeDefinition;
                ResultType = resultType;
            }

            public Type FSharpAsyncTypeDefinition { get; }
            public Type ResultType { get; }
        }

        public static AwaitAdapter? TryCreate(object awaitable)
        {
            if (awaitable is null)
                return null;

            var info = GetAsyncInfo(awaitable.GetType());
            if (info is null)
                return null;

            if (_startImmediateAsTaskMethod is null)
            {
                var asyncHelperMethodsType = info.FSharpAsyncTypeDefinition.Assembly.GetType("Microsoft.FSharp.Control.FSharpAsync");
                if (asyncHelperMethodsType is null)
                    throw new InvalidOperationException("Cannot find non-generic FSharpAsync type in the same assembly as the generic one.");

                _startImmediateAsTaskMethod = asyncHelperMethodsType
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Single(method =>
                   {
                       if (method.Name != "StartImmediateAsTask")
                           return false;
                       var typeArguments = method.GetGenericArguments();
                       if (typeArguments.Length != 1)
                           return false;

                       var parameters = method.GetParameters();
                       if (parameters.Length != 2)
                           return false;

                       if (parameters[0].ParameterType != info.FSharpAsyncTypeDefinition.MakeGenericType(typeArguments[0]))
                           return false;

                       return parameters[1].ParameterType.IsFSharpOption(out Type? someType)
                           && someType.FullName == "System.Threading.CancellationToken";
                   });
            }

            var task = _startImmediateAsTaskMethod
                .MakeGenericMethod(info.ResultType)
                .Invoke(null, new[] { awaitable, null });

            return AwaitAdapter.FromAwaitable(task);
        }
    }
}
