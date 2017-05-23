// ***********************************************************************
// Copyright © 2017 Charlie Poole
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

#if ASYNC
using System;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    partial class AwaitUtils
    {
        private sealed class AwaitableApiAccessors
        {
            private readonly MethodInfo _getAwaiterMethod;
            private readonly MethodInfo _isCompletedGetter;
            private readonly MethodInfo _getResultMethod;
            private readonly bool _isICriticalNotifyCompletion;

            public Type ResultType => _getResultMethod.ReturnType;

            private AwaitableApiAccessors(MethodInfo getAwaiterMethod, MethodInfo isCompletedGetter, MethodInfo getResultMethod, bool isICriticalNotifyCompletion)
            {
                _getAwaiterMethod = getAwaiterMethod;
                _isCompletedGetter = isCompletedGetter;
                _getResultMethod = getResultMethod;
                _isICriticalNotifyCompletion = isICriticalNotifyCompletion;
            }

            public static AwaitableApiAccessors TryCreate(Type awaitableType)
            {
                // https://github.com/dotnet/csharplang/blob/master/spec/expressions.md#awaitable-expressions

                var getAwaiterMethod = GetPublicInstanceMethod(awaitableType, "GetAwaiter", EmptyTypes);
                if (getAwaiterMethod == null || getAwaiterMethod.IsGenericMethod)
                    return null;

                var awaiterType = getAwaiterMethod.ReturnType;
                if (!INotifyCompletionType.IsAssignableFrom(awaiterType))
                    return null;

                var isCompletedGetter = GetPublicInstancePropertyGetter(awaiterType, "IsCompleted", typeof(bool), EmptyTypes);
                if (isCompletedGetter == null)
                    return null;

                var getResultMethod = GetPublicInstanceMethod(awaiterType, "GetResult", EmptyTypes);
                if (getResultMethod == null || getResultMethod.IsGenericMethod)
                    return null;

                return new AwaitableApiAccessors(
                    getAwaiterMethod,
                    isCompletedGetter,
                    getResultMethod,
                    ICriticalNotifyCompletionType.IsAssignableFrom(awaiterType));
            }

            public object GetAwaiter(object awaitable) => InvokeUnwrapTargetInvocationException(_getAwaiterMethod, awaitable, null);

            public bool IsCompleted(object awaiter) => (bool)InvokeUnwrapTargetInvocationException(_isCompletedGetter, awaiter, null);

            public object GetResult(object awaiter) => InvokeUnwrapTargetInvocationException(_getResultMethod, awaiter, null);

            /// <summary>
            /// Uses OnCompleted or UnsafeOnCompleted as known statically
            /// </summary>
            public void OnCompleted(object awaiter, Action continuation)
            {
                InvokeUnwrapTargetInvocationException(
                    _isICriticalNotifyCompletion ? UnsafeOnCompletedMethod : OnCompletedMethod,
                    awaiter,
                    new object[] { continuation });
            }


            #region ReflectionUtils

#if PORTABLE
            private static readonly Type[] EmptyTypes = new Type[0];
#else
            private static readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
            private static readonly Type INotifyCompletionType = Type.GetType("System.Runtime.CompilerServices.INotifyCompletion", true);
            private static readonly Type ICriticalNotifyCompletionType = Type.GetType("System.Runtime.CompilerServices.ICriticalNotifyCompletion", true);
            private static readonly MethodInfo OnCompletedMethod = INotifyCompletionType.GetMethod("OnCompleted");
            private static readonly MethodInfo UnsafeOnCompletedMethod = ICriticalNotifyCompletionType.GetMethod("UnsafeOnCompleted");



            private static MethodInfo GetPublicInstanceMethod(Type type, string name, Type[] types)
            {
#if NETSTANDARD1_6 || PORTABLE
                return type.GetTypeInfo().DeclaredMethods.SingleOrDefault(method =>
                    method.IsPublic &&
                    !method.IsStatic &&
                    method.Name == name &&
                    method.GetParameters().ParametersMatch(types)
                );
#else
                return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, types, null);
#endif
            }
            private static MethodInfo GetPublicInstancePropertyGetter(Type type, string name, Type returnType, Type[] types)
            {
#if NETSTANDARD1_6 || PORTABLE
                return type.GetTypeInfo().DeclaredProperties.SingleOrDefault(property =>
                    property.GetMethod.IsPublic &&
                    !property.GetMethod.IsStatic &&
                    property.Name == name &&
                    property.GetIndexParameters().ParametersMatch(types)
                )?.GetMethod;
#else
                return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance, null, returnType, types, null)?.GetGetMethod();
#endif
            }



            private static object InvokeUnwrapTargetInvocationException(MethodInfo methodInfo, object obj, object[] parameters)
            {
                try
                {
                    return methodInfo.Invoke(obj, parameters);
                }
                catch (TargetInvocationException ex) when (ex.InnerException != null)
                {
                    ExceptionHelper.Rethrow(ex.InnerException);
                    throw null; // Impossible to get here
                }
            }

            #endregion
        }
    }
}
#endif