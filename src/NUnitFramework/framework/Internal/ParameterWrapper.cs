// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The ParameterWrapper class wraps a ParameterInfo so that it may
    /// be used in a platform-independent manner.
    /// </summary>
    public class ParameterWrapper : IParameterInfo
    {
        /// <summary>
        /// Construct a ParameterWrapper for a given method and parameter
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameterInfo"></param>
        public ParameterWrapper(IMethodInfo method, ParameterInfo parameterInfo)
        {
            Method = method;
            ParameterInfo = parameterInfo;
        }

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the parameter is optional
        /// </summary>
        public bool IsOptional => ParameterInfo.IsOptional;

        /// <summary>
        /// Gets an IMethodInfo representing the method for which this is a parameter.
        /// </summary>
        public IMethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the underlying ParameterInfo
        /// </summary>
        public ParameterInfo ParameterInfo { get; private set; }

        /// <summary>
        /// Gets the Type of the parameter
        /// </summary>
        public Type ParameterType => ParameterInfo.ParameterType;

        #endregion

        #region Methods

        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this method
        /// </summary>
        public T[] GetCustomAttributes<T>(bool inherit) where T : class
        {
            return ParameterInfo.GetAttributes<T>(inherit);
        }

        /// <summary>
        /// Gets a value indicating whether one or more attributes of the specified type are defined on the parameter.
        /// </summary>
        public bool IsDefined<T>(bool inherit) where T : class
        {
            return ParameterInfo.HasAttribute<T>(inherit);
        }

        #endregion
    }
}
