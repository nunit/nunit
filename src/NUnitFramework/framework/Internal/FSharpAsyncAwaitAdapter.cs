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
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    internal static class FSharpAsyncAwaitAdapter
    {
        private static MethodInfo _startImmediateAsTaskMethod;

        public static bool IsAwaitable(Type awaitableType)
        {
            return GetAsyncInfo(awaitableType) != null;
        }

        public static Type GetResultType(Type awaitableType)
        {
            return GetAsyncInfo(awaitableType)?.ResultType;
        }

        private static AsyncInfo GetAsyncInfo(Type asyncType)
        {
            if (asyncType == null) return null;

            if (!asyncType.GetTypeInfo().IsGenericType) return null;
            var genericDefinition = asyncType.GetGenericTypeDefinition();
            if (genericDefinition.FullName != "Microsoft.FSharp.Control.FSharpAsync`1") return null;

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

        public static AwaitAdapter TryCreate(object awaitable)
        {
            if (awaitable == null) return null;

            var info = GetAsyncInfo(awaitable.GetType());
            if (info == null) return null;

            if (_startImmediateAsTaskMethod == null)
            {
                var asyncHelperMethodsType = info.FSharpAsyncTypeDefinition.GetTypeInfo().Assembly.GetType("Microsoft.FSharp.Control.FSharpAsync");
                if (asyncHelperMethodsType == null)
                    throw new InvalidOperationException("Cannot find non-generic FSharpAsync type in the same assembly as the generic one.");

                _startImmediateAsTaskMethod = asyncHelperMethodsType
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Single(method =>
                   {
                       if (method.Name != "StartImmediateAsTask") return false;
                       var typeArguments = method.GetGenericArguments();
                       if (typeArguments.Length != 1) return false;

                       var parameters = method.GetParameters();
                       if (parameters.Length != 2) return false;

                       if (parameters[0].ParameterType != info.FSharpAsyncTypeDefinition.MakeGenericType(typeArguments[0])) return false;

                       Type someType;
                       return parameters[1].ParameterType.IsFSharpOption(out someType)
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
