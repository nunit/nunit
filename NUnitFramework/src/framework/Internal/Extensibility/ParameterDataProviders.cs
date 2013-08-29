// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Builders;

namespace NUnit.Framework.Extensibility
{
#if NUNITLITE
    class ParameterDataProviders : IParameterDataProvider
    {
        private List<IParameterDataProvider> Extensions = new List<IParameterDataProvider>();

        public ParameterDataProviders()
        {
            Extensions.Add(new ParameterDataProvider());
            Extensions.Add(new DatapointProvider());
        }
#else
    class ParameterDataProviders : ExtensionPoint, IParameterDataProvider
    {
        public ParameterDataProviders(ExtensionHost host)
            : base("ParameterDataProviders", host) { }
#endif

        #region IDataPointProvider Members

        /// <summary>
        /// Determine whether any data is available for a parameter.
        /// </summary>
        /// <param name="parameter">A ParameterInfo representing one
        /// argument to a parameterized test</param>
        /// <returns>True if any data is available, otherwise false.</returns>
        public bool HasDataFor(ParameterInfo parameter)
        {
            foreach (IParameterDataProvider provider in Extensions)
                if (provider.HasDataFor(parameter))
                    return true;

            return false;
        }

        /// <summary>
        /// Return an IEnumerable providing data for use with the
        /// supplied parameter.
        /// </summary>
        /// <param name="parameter">A ParameterInfo representing one
        /// argument to a parameterized test</param>
        /// <returns>An IEnumerable providing the required data</returns>
        public IEnumerable GetDataFor(ParameterInfo parameter)
        {
            var list = new List<object>();

            foreach (IParameterDataProvider provider in Extensions)
                if (provider.HasDataFor(parameter))
                    foreach (object o in provider.GetDataFor(parameter))
                        list.Add(o);

            return list;
        }
        #endregion

#if !NUNITLITE
        #region ExtensionPoint Overrides
        protected override bool IsValidExtension(object extension)
        {
            return extension is IParameterDataProvider;
        }
        #endregion
#endif
    }
}
