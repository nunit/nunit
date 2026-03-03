// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class ArgumentsExtensions
    {
        extension(Array array)
        {
            /// <summary>
            /// Returns the elements of the array as a flat <c>object?[]</c>,
            /// suitable for use as individual test method arguments.
            /// </summary>
            public object?[] Unpack()
            {
                var result = new object?[array.Length];
                for (var i = 0; i < array.Length; i++)
                    result[i] = array.GetValue(i);
                return result;
            }
        }

        extension(object?[] arguments)
        {
            /// <summary>
            /// Checks if the last element of an argument list is a <see cref="CancellationToken"/>
            /// </summary>
            public bool LastArgumentIsCancellationToken()
            {
                return arguments.Length > 0 && arguments[arguments.Length - 1]?.GetType() == typeof(CancellationToken);
            }
        }

        extension(IParameterInfo lastParameter)
        {
            /// <summary>
            /// Checks if the parameter is a 'params' array.
            /// </summary>
            public bool ParameterIsParamsArray()
            {
                return lastParameter.ParameterType.IsArray && lastParameter.ParameterInfo.ParameterIsParamsArray();
            }
        }

        extension(ParameterInfo lastParameter)
        {
            /// <summary>
            /// Checks if the parameter is a 'params' array.
            /// </summary>
            public bool ParameterIsParamsArray()
            {
                return lastParameter.ParameterType.IsArray && lastParameter.HasAttribute<ParamArrayAttribute>(false);
            }
        }

        extension(IParameterInfo[] parameters)
        {
            /// <summary>
            /// Checks if the last type of a parameter list has a type <see cref="CancellationToken"/>
            /// </summary>
            public bool LastParameterAcceptsCancellationToken()
            {
                return parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);
            }

            /// <summary>
            /// Checks if the last parameter in a parameter list is a 'params' array.
            /// </summary>
            public bool LastParameterIsParamsArray()
            {
                if (parameters.Length == 0)
                {
                    return false;
                }

                return parameters[parameters.Length - 1].ParameterIsParamsArray();
            }

            /// <summary>
            /// Returns true if the parameter list contains exactly one parameter of the specified type.
            /// </summary>
            /// <param name="type">The exact type the single parameter must have.</param>
            public bool HasSingleParameterOfType(Type type)
            {
                return parameters.Length == 1 && parameters[0].ParameterType == type;
            }

            /// <summary>
            /// Determines whether <paramref name="array"/> should be unpacked into individual
            /// test arguments, or passed as a single argument to the test method.
            /// </summary>
            /// <param name="array">The array yielded by the <see cref="TestCaseSourceAttribute"/> source,
            /// which may either be a container holding one argument per element (e.g. <c>new object[] { a, b, c }</c>)
            /// or the actual argument value itself (e.g. an <c>int[]</c> to be passed to an <see cref="System.Array"/> parameter).</param>
            public bool ShouldUnpackArrayAsArguments(Array array)
            {
                int argsNeeded = parameters.Length;

                // No parameters
                if (argsNeeded == 0)
                    return false;

                Type paramType = parameters[0].ParameterType;
                Type arrayType = array.GetType();

                // Classic argument-container pattern: new object[] { actualArg }
                // Except when the parameter is of type object — then the array IS the argument, not a container.
                // Note that it is impossible to pass a single length object array to a method expecting an object array.
                if (arrayType == typeof(object[]) && array.Length == 1 && paramType != typeof(object))
                    return true;

                // If the parameter accepts this array type
                // directly (e.g. Array, IList<T>, IEnumerable) then the array IS the argument.
                // But only if we don't need more arguments.
                // i.e. We expect one argument or one argument plus a params array.
                if (paramType.IsAssignableFrom(arrayType) && (argsNeeded == 1 || argsNeeded == 2 && parameters.LastParameterIsParamsArray()))
                    return false;

                // A params parameter always absorbs the elements individually
                if (parameters.LastParameterIsParamsArray())
                    return true;

                // Multiple parameters: each element maps to one parameter
                if (argsNeeded > 1)
                    return true;

                // Single parameter from here on.

                // The parameter is of type object, so the array is the argument, not a container.
                if (paramType == typeof(object))
                    return false;

                // Classic argument-container pattern: new object[] { actualArg }
                // Unpack and let the count mismatch produce a clear error.
                return true;
            }
        }

        extension(ParameterInfo[] parameters)
        {
            /// <summary>
            /// Checks if the last type of a parameter list has a type <see cref="CancellationToken"/>
            /// </summary>
            public bool LastParameterAcceptsCancellationToken()
            {
                return parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);
            }
        }
    }
}
