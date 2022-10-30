// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

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
        /// Gets a value indicating whether the method is static.
        /// </summary>
        bool IsStatic { get; }

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
        object? Invoke(object? fixture, params object?[]? args);

        #endregion
    }
}
