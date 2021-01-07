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
using NUnit.Compatibility;
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
        public bool IsOptional
        {
            get { return ParameterInfo.IsOptional;  }
        }

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
        public Type ParameterType
        {
            get { return ParameterInfo.ParameterType;  }
        }

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
