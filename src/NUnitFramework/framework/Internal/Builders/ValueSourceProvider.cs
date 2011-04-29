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
using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Builders
{
    /// <summary>
    /// ValueSourceProvider supplies data items for individual parameters
    /// from named data sources in the test class or a separate class.
    /// </summary>
    public class ValueSourceProvider : IDataPointProvider
    {
        #region IDataPointProvider Members

        /// <summary>
        /// Determine whether any data sources are available for a parameter.
        /// </summary>
        /// <param name="parameter">A ParameterInfo test parameter</param>
        /// <returns>True if any data is available, otherwise false.</returns>
        public bool HasDataFor(ParameterInfo parameter)
        {
            return parameter.IsDefined(typeof(ValueSourceAttribute), false);
        }

        /// <summary>
        /// Return an IEnumerable providing test data for use with
        /// one parameter of a parameterized test.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IEnumerable GetDataFor(ParameterInfo parameter)
        {
            ObjectList data = new ObjectList();

#if true // EXPERIMENTAL
            foreach (ValueSourceAttribute attr in parameter.GetCustomAttributes(typeof(ValueSourceAttribute), false))
            {
                foreach (object item in attr.GetData(parameter))
                    data.Add(item);
            }
#else
            foreach (ProviderReference providerRef in GetSourcesFor(parameter, parentSuite))
            {
                IEnumerable instance = providerRef.GetInstance();
                if (instance != null)
                    foreach (object o in instance)
                        data.Add(o);
            }
#endif

            return data;
        }
        #endregion

        #region Helper Methods
        //private static ProviderList GetSourcesFor(ParameterInfo parameter, Test parent)
        //{
        //    ProviderList sources = new ProviderList();
        //    TestFixture parentSuite = parent as TestFixture;

        //    foreach (ValueSourceAttribute sourceAttr in parameter.GetCustomAttributes(typeof(ValueSourceAttribute), false))
        //    {
        //        Type sourceType = sourceAttr.SourceType;
        //        string sourceName = sourceAttr.SourceName;

        //        if (sourceType != null)
        //            sources.Add(new ProviderReference(sourceType, sourceName));
        //        else if (parentSuite != null)
        //            sources.Add(new ProviderReference(parentSuite.FixtureType, parentSuite.arguments, sourceName));
        //        else
        //            sources.Add(new ProviderReference(parameter.Member.ReflectedType, sourceName));
        //    }
        //    return sources;
        //}
        #endregion
    }
}
