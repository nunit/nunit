// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The MethodWrapper class wraps a MethodInfo so that it may
    /// be used in a platform-independent manner.
    /// </summary>
    public class MethodWrapper : IMethodInfo, IEquatable<MethodWrapper>
    {
        /// <summary>
        /// Construct a MethodWrapper for a Type and a MethodInfo.
        /// </summary>
        public MethodWrapper(Type type, MethodInfo method)
        {
            TypeInfo = new TypeWrapper(type);
            MethodInfo = method;
        }

        /// <summary>
        /// Construct a MethodInfo for a given Type and method name.
        /// </summary>
        public MethodWrapper(Type type, string methodName)
        {
            TypeInfo = new TypeWrapper(type);
            MethodInfo = type.GetMethod(methodName);
        }

        #region IMethod Implementation

        /// <summary>
        /// Gets the Type from which this method was reflected.
        /// </summary>
        public ITypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the MethodInfo for this method.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string Name
        {
            get { return MethodInfo.Name;  }
        }

        /// <summary>
        /// Gets a value indicating whether the method is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return MethodInfo.IsAbstract; }
        }

        /// <summary>
        /// Gets a value indicating whether the method is public.
        /// </summary>
        public bool IsPublic
        {
            get { return MethodInfo.IsPublic;  }
        }

        /// <summary>
        /// Gets a value indicating whether the method is static.
        /// </summary>
        public bool IsStatic => MethodInfo.IsStatic;

        /// <summary>
        /// Gets a value indicating whether the method contains unassigned generic type parameters.
        /// </summary>
        public bool ContainsGenericParameters
        {
            get { return MethodInfo.ContainsGenericParameters; }
        }

        /// <summary>
        /// Gets a value indicating whether the method is a generic method.
        /// </summary>
        public bool IsGenericMethod
        {
            get { return MethodInfo.IsGenericMethod; }
        }

        /// <summary>
        /// Gets a value indicating whether the MethodInfo represents the definition of a generic method.
        /// </summary>
        public bool IsGenericMethodDefinition
        {
            get { return MethodInfo.IsGenericMethodDefinition; }
        }

        /// <summary>
        /// Gets the return Type of the method.
        /// </summary>
        public ITypeInfo ReturnType
        {
            get { return new TypeWrapper(MethodInfo.ReturnType); }
        }

        /// <summary>
        /// Gets the parameters of the method.
        /// </summary>
        /// <returns></returns>
        public IParameterInfo[] GetParameters()
        {
            var parameters = MethodInfo.GetParameters();
            var result = new IParameterInfo[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                result[i] = new ParameterWrapper(this, parameters[i]);

            return result;
        }

        /// <summary>
        /// Returns the Type arguments of a generic method or the Type parameters of a generic method definition.
        /// </summary>
        public Type[] GetGenericArguments()
        {
            return MethodInfo.GetGenericArguments();
        }

        /// <summary>
        /// Replaces the type parameters of the method with the array of types provided and returns a new IMethodInfo.
        /// </summary>
        /// <param name="typeArguments">The type arguments to be used</param>
        /// <returns>A new IMethodInfo with the type arguments replaced</returns>
        public IMethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            return new MethodWrapper(TypeInfo.Type, MethodInfo.MakeGenericMethod(typeArguments));
        }

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this method
        /// </summary>
        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            return MethodInfo.GetAttributes<T>(inherit);
        }

        /// <summary>
        /// Gets a value indicating whether one or more attributes of the specified type are defined on the method.
        /// </summary>
        public bool IsDefined<T>(bool inherit) where T : class
        {
            return MethodInfo.HasAttribute<T>(inherit);
        }

        /// <summary>
        /// Invokes the method, converting any TargetInvocationException to an NUnitException.
        /// </summary>
        /// <param name="fixture">The object on which to invoke the method</param>
        /// <param name="args">The argument list for the method</param>
        /// <returns>The return value from the invoked method</returns>
        public object? Invoke(object? fixture, params object?[]? args)
        {
            return Reflect.InvokeMethod(MethodInfo, fixture, args);
        }

        /// <summary>
        /// Override ToString() so that error messages in NUnit's own tests make sense
        /// </summary>
        public override string ToString()
        {
            return MethodInfo.Name;
        }

        #endregion

        /// <inheritdoc />
        public bool Equals(MethodWrapper? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return MethodInfo.Equals(other.MethodInfo);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MethodWrapper)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }
    }
}
