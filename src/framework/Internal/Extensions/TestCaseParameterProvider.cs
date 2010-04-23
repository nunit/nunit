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
using System.Collections;
#if CLR_2_0
using System.Collections.Generic;
#endif
using System.Reflection;
using System.Text;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Core.Builders
{
    /// <summary>
    /// TestCaseParameterProvider supplies test cases specified by TestCaseAttribute
    /// </summary>
    public class TestCaseParameterProvider : ITestCaseProvider 
    {
        /// <summary>
        /// Determine whether any test cases are available for a parameterized method.
        /// </summary>
        /// <param name="method">A MethodInfo representing a parameterized test</param>
        /// <returns>True if any cases are available, otherwise false.</returns>
        public bool HasTestCasesFor(MethodInfo method)
        {
            return method.IsDefined(typeof(TestCaseAttribute), false);
        }

        /// <summary>
        /// Return an IEnumerable providing test cases for use in
        /// running a parameterized test.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable GetTestCasesFor(MethodInfo method)
        {
#if CLR_2_0
            List<ParameterSet> list = new List<ParameterSet>();
#else
			ArrayList list = new ArrayList();
#endif

            TestCaseAttribute[] attrs = (TestCaseAttribute[])Reflect.GetAttributesOfType(method, typeof(TestCaseAttribute), false);

            ParameterInfo[] parameters = method.GetParameters();
            int argsNeeded = parameters.Length;

            foreach (TestCaseAttribute attr in attrs)
            {
                ParameterSet parms;

                try
                {
                    parms = new ParameterSet(attr);

                    //if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
                    //    parms.Arguments = new object[]{parms.Arguments};

                    if (argsNeeded == 1 && method.GetParameters()[0].ParameterType == typeof(object[]))
                    {
                        if (parms.Arguments.Length > 1 ||
                            parms.Arguments.Length == 1 && parms.Arguments[0].GetType() != typeof(object[]))
                        {
                            parms.Arguments = new object[] { parms.Arguments };
                        }
                    }


                    if (parms.Arguments.Length == argsNeeded)
                        PerformSpecialConversions(parms.Arguments, parameters);
                }
                catch (Exception ex)
                {
                    parms = new ParameterSet( ex );
                }

                list.Add( parms );
			}

			return list;
        }
        
        /// <summary>
        /// Performs several special conversions allowed by NUnit in order to
        /// permit arguments with types that cannot be used in the constructor
        /// of an Attribute such as TestCaseAttribute.
        /// </summary>
        /// <param name="arglist">The arguments to be converted</param>
        /// <param name="parameters">The ParameterInfo array for the method</param>
        private static void PerformSpecialConversions(object[] arglist, ParameterInfo[] parameters)
        {
            for (int i = 0; i < arglist.Length; i++)
            {
                object arg = arglist[i];
                Type targetType = parameters[i].ParameterType;

                if (arg == null)
                    continue;

                if (arg is SpecialValue && (SpecialValue)arg == SpecialValue.Null)
                {
                    arglist[i] = null;
                    continue;
                }

                if (targetType.IsAssignableFrom(arg.GetType()))
                    continue;
                
                if (arg is DBNull)
                {
                    arglist[i] = null;
                    continue;
                }

                bool convert = false;

                if (targetType == typeof(decimal))
                    convert = arg is double || arg is string;
                else 
                if (targetType == typeof(DateTime) || targetType == typeof(TimeSpan))
                    convert = arg is string;

                if (convert)
                try
                {
                    arglist[i] = Convert.ChangeType(arg, targetType, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    // Do nothing - the incompatible argument will be
                    // reported when the method is inoked.r
                }
            }
        }
    }
}
