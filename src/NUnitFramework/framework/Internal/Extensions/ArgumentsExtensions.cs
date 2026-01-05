// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class ArgumentsExtensions
    {
        /// <summary>
        /// Checks if the last element of an argument list is a <see cref="CancellationToken"/>
        /// </summary>
        public static bool LastArgumentIsCancellationToken(this object?[] arguments)
        {
            return arguments.Length > 0 && arguments[arguments.Length - 1]?.GetType() == typeof(CancellationToken);
        }

        /// <summary>
        /// Checks if the last type of a parameter list has a type <see cref="CancellationToken"/>
        /// </summary>
        public static bool LastParameterAcceptsCancellationToken(this IParameterInfo[] parameters)
        {
            return parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);
        }

        /// <summary>
        /// Checks if the last type of a parameter list has a type <see cref="CancellationToken"/>
        /// </summary>
        public static bool LastParameterAcceptsCancellationToken(this ParameterInfo[] parameters)
        {
            return parameters.Length > 0 && parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);
        }

        /// <summary>
        /// Checks if the parameter is a 'params' array.
        /// </summary>
        public static bool ParameterIsParamsArray(this IParameterInfo lastParameter)
        {
            return lastParameter.ParameterType.IsArray && lastParameter.ParameterInfo.ParameterIsParamsArray();
        }

        /// <summary>
        /// Checks if the parameter is a 'params' array.
        /// </summary>
        public static bool ParameterIsParamsArray(this ParameterInfo lastParameter)
        {
            return lastParameter.ParameterType.IsArray && lastParameter.HasAttribute<ParamArrayAttribute>(false);
        }

        /// <summary>
        /// Checks if the last parameter in a parameter list is a 'params' array.
        /// </summary>
        public static bool LastParameterIsParamsArray(this IParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
            {
                return false;
            }

            return parameters[parameters.Length - 1].ParameterIsParamsArray();
        }
    }
}
