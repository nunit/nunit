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
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
    public class CombinatorialTestCaseProvider : ITestCaseProvider2
    {
        #region Static Members
        static IDataPointProvider2 dataPointProvider =
            (IDataPointProvider2)CoreExtensions.Host.GetExtensionPoint("DataPointProviders");

        //static readonly string CombinatorialAttribute = "NUnit.Framework.CombinatorialAttribute";
        static readonly string PairwiseAttribute = "NUnit.Framework.PairwiseAttribute";
        static readonly string SequentialAttribute = "NUnit.Framework.SequentialAttribute";
        #endregion

        #region ITestCaseProvider Members
        public bool HasTestCasesFor(System.Reflection.MethodInfo method)
        {
            if (method.GetParameters().Length == 0)
                return false;

            foreach (ParameterInfo parameter in method.GetParameters())
                if (!dataPointProvider.HasDataFor(parameter))
                    return false;

            return true;
        }

        public IEnumerable GetTestCasesFor(MethodInfo method)
        {
            return GetStrategy(method, null).GetTestCases();
        }
        #endregion

        #region ITestCaseProvider2 Members
        public bool HasTestCasesFor(System.Reflection.MethodInfo method, Test suite)
        {
            return HasTestCasesFor(method);
        }

        public IEnumerable GetTestCasesFor(MethodInfo method, Test suite)
        {
            return GetStrategy(method, suite).GetTestCases();
        }
        #endregion

        #region GetStrategy
        private CombiningStrategy GetStrategy(MethodInfo method, Test suite)
        {
            ParameterInfo[] parameters = method.GetParameters();
            IEnumerable[] sources = new IEnumerable[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                sources[i] = dataPointProvider.GetDataFor(parameters[i], suite);

            if (method.IsDefined(typeof(NUnit.Framework.SequentialAttribute), false))
                return new SequentialStrategy(sources);

            if (method.IsDefined(typeof(NUnit.Framework.PairwiseAttribute), false) &&
                method.GetParameters().Length > 2)
                    return new PairwiseStrategy(sources);

            return new CombinatorialStrategy(sources);
        }
        #endregion
    }
}
