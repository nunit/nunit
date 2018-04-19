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

using System;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// GenericMethodHelper is able to deduce the Type arguments for
    /// a generic method from the actual arguments provided.
    /// </summary>
    public class GenericMethodHelper
    {
        private static class ConflictingTypesMarkerClass { }

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
        public bool TryGetTypeArguments(object[] argList, out Type[] typeArguments)
        {
            Guard.ArgumentValid(argList.Length == ParmTypes.Length, "Supplied arguments do not match required method parameters", nameof(argList));

            for (int argIndex = 0; argIndex < ParmTypes.Length; argIndex++)
            {
                var arg = argList[argIndex];

                if (arg != null)
                {
                    Type argType = arg.GetType();
                    TryApplyArgType(ParmTypes[argIndex], argType);
                }
            }

            foreach (var typeArg in TypeArgs)
            {
                if (typeArg == null || typeArg == ConflictingTypesMarker)
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
            else if (parmType.GetTypeInfo().ContainsGenericParameters)
            {
                var genericArgTypes = parmType.GetGenericArguments();

                if (argType.HasElementType)
                {
                    ApplyArgType(genericArgTypes[0], argType.GetElementType());
                }
                else if (argType.GetTypeInfo().IsGenericType && IsAssignableToGenericType(argType, parmType))
                {
                    Type[] argTypes = argType.GetGenericArguments();

                    if (argTypes.Length == genericArgTypes.Length)
                        for (int i = 0; i < genericArgTypes.Length; i++)
                            TryApplyArgType(genericArgTypes[i], argTypes[i]);
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
                if (iterator.GetTypeInfo().IsGenericType)
                {
                    // The Type returned by GetGenericTyeDefinition may have the
                    // FullName set to null, so we do our own comparison
                    Type gtd = iterator.GetGenericTypeDefinition();
                    if (gtd.Name == genericType.Name && gtd.Namespace == genericType.Namespace)
                        return true;
                }
            }

            if (givenType.GetTypeInfo().IsGenericType)
            {
                // The Type returned by GetGenericTyeDefinition may have the
                // FullName set to null, so we do our own comparison
                Type gtd = givenType.GetGenericTypeDefinition();
                if (gtd.Name == genericType.Name && gtd.Namespace == genericType.Namespace)
                    return true;
            }

            Type baseType = givenType.GetTypeInfo().BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
