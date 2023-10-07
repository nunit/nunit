// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IParameterInfo interface is an abstraction of a .NET parameter.
    /// </summary>
    public interface IParameterInfo : IReflectionInfo
    {
        /// <summary>
        /// Gets a value indicating whether the parameter is optional
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets an IMethodInfo representing the method for which this is a parameter
        /// </summary>
        IMethodInfo Method { get; }

        /// <summary>
        /// Gets the underlying .NET ParameterInfo
        /// </summary>
        ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the Type of the parameter
        /// </summary>
        Type ParameterType { get; }
    }
}
