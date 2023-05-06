// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
    }
}
