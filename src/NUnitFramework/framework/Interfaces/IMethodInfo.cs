// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IMethodInfo class is used to encapsulate information
    /// about a method in a platform-independent manner.
    /// </summary>
    public interface IMethodInfo : IReflectionInfo
    {
        #region Properties

        /// <summary>
        /// Gets the Type from which this method was reflected.
        /// </summary>
        ITypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the MethodInfo for this method.
        /// </summary>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the method is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether the method is public.
        /// </summary>
        bool IsPublic { get; }

        /// <summary>
        /// Gets a value indicating whether the method contains unassigned generic type parameters.
        /// </summary>
        bool ContainsGenericParameters { get; }

        /// <summary>
        /// Gets a value indicating whether the method is a generic method.
        /// </summary>
        bool IsGenericMethod { get; }

        /// <summary>
        /// Gets a value indicating whether the MethodInfo represents the definition of a generic method.
        /// </summary>
        bool IsGenericMethodDefinition { get; }

        /// <summary>
        /// Gets the return Type of the method.
        /// </summary>
        ITypeInfo ReturnType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the parameters of the method.
        /// </summary>
        /// <returns></returns>
        IParameterInfo[] GetParameters();

        /// <summary>
        /// Returns the Type arguments of a generic method or the Type parameters of a generic method definition.
        /// </summary>
        Type[] GetGenericArguments();

        /// <summary>
        /// Replaces the type parameters of the method with the array of types provided and returns a new IMethodInfo.
        /// </summary>
        /// <param name="typeArguments">The type arguments to be used</param>
        /// <returns>A new IMethodInfo with the type arguments replaced</returns>
        IMethodInfo MakeGenericMethod(params Type[] typeArguments);

        /// <summary>
        /// Invokes the method, converting any TargetInvocationException to an NUnitException.
        /// </summary>
        /// <param name="fixture">The object on which to invoke the method</param>
        /// <param name="args">The argument list for the method</param>
        /// <returns>The return value from the invoked method</returns>
        object Invoke(object fixture, params object[] args);

        #endregion
    }
}
