// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// GenericMethodHelper is able to deduce the Type arguments for
    /// a generic method from the actual arguments provided.
    /// </summary>
    public class GenericMethodHelper
    {
        private static class ConflictingTypesMarkerClass
        {
        }

        /// <summary>
        /// A special value, which is used to indicate that BestCommonType() method
        /// was unable to find a common type for the specified arguments.
        /// </summary>
        private static readonly Type ConflictingTypesMarker = typeof(ConflictingTypesMarkerClass);

        /// <summary>
        /// Construct a GenericMethodHelper for a method
        /// </summary>
        /// <param name="method">MethodInfo for the method to examine</param>
        public GenericMethodHelper(MethodInfo method)
        {
            Guard.ArgumentValid(method.IsGenericMethod, "Specified method must be generic", nameof(method));

            Method = method;

            TypeParms = Method.GetGenericArguments();
            TypeArgs = new Type[TypeParms.Length];

            var parms = Method.GetParameters();
            ParmTypes = new Type[parms.Length];
            for (int i = 0; i < parms.Length; i++)
                ParmTypes[i] = parms[i].ParameterType;
        }

        private MethodInfo Method { get; }

        private Type[] TypeParms { get; }
        private Type[] TypeArgs { get; }

        private Type[] ParmTypes { get; }

        /// <summary>
        /// Return the type arguments for the method, deducing them
        /// from the arguments actually provided.
        /// </summary>
        /// <param name="argList">The arguments to the method</param>
        /// <param name="typeArguments">If successful, an array of type arguments.</param>
        public bool TryGetTypeArguments(object?[] argList, [NotNullWhen(true)] out Type[]? typeArguments)
        {
            Guard.ArgumentValid(argList.Length == ParmTypes.Length, "Supplied arguments do not match required method parameters", nameof(argList));

            for (int argIndex = 0; argIndex < ParmTypes.Length; argIndex++)
            {
                var arg = argList[argIndex];

                if (arg is not null)
                {
                    Type argType = arg.GetType();
                    TryApplyArgType(ParmTypes[argIndex], argType);
                }
            }

            foreach (var typeArg in TypeArgs)
            {
                if (typeArg is null || typeArg == ConflictingTypesMarker)
                {
                    typeArguments = null;
                    return false;
                }
            }

            typeArguments = TypeArgs;
            return true;
        }

        private void TryApplyArgType(Type parmType, Type argType)
        {
            if (parmType.IsGenericParameter)
            {
                ApplyArgType(parmType, argType);
            }
            else if (parmType.ContainsGenericParameters)
            {
                Type[] genericArgTypes = parmType.IsArray
                    ? new[] { parmType.GetElementType()! }
                    : parmType.GetGenericArguments();

                if (argType.HasElementType)
                {
                    ApplyArgType(genericArgTypes[0], argType.GetElementType()!);
                }
                else if (argType.IsGenericType && IsAssignableToGenericType(argType, parmType))
                {
                    Type[] argTypes = argType.GetGenericArguments();

                    if (argTypes.Length == genericArgTypes.Length)
                    {
                        for (int i = 0; i < genericArgTypes.Length; i++)
                            TryApplyArgType(genericArgTypes[i], argTypes[i]);
                    }
                }
            }
        }

        private void ApplyArgType(Type parmType, Type argType)
        {
            // Note: parmType must be generic parameter type - checked by caller
            var index = parmType.GenericParameterPosition;
            if (!TypeHelper.TryGetBestCommonType(TypeArgs[index], argType, out TypeArgs[index]))
                TypeArgs[index] = ConflictingTypesMarker;
        }

        // Simulates IsAssignableTo generics
        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var iterator in interfaceTypes)
            {
                if (iterator.IsGenericType)
                {
                    // The Type returned by GetGenericTyeDefinition may have the
                    // FullName set to null, so we do our own comparison
                    Type gtd = iterator.GetGenericTypeDefinition();
                    if (gtd.Name == genericType.Name && gtd.Namespace == genericType.Namespace)
                        return true;
                }
            }

            if (givenType.IsGenericType)
            {
                // The Type returned by GetGenericTyeDefinition may have the
                // FullName set to null, so we do our own comparison
                Type gtd = givenType.GetGenericTypeDefinition();
                if (gtd.Name == genericType.Name && gtd.Namespace == genericType.Namespace)
                    return true;
            }

            Type? baseType = givenType.BaseType;
            if (baseType is null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
