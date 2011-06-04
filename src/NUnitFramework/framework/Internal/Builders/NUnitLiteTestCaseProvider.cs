using System;
using System.Reflection;
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Builders
{
    /// <summary>
    /// NUnitLiteTestCaseProvider wraps DataAttributeTestCaseProvider and
    /// returns a NonRunnable ParameterSet if an exception is thrown.
    /// </summary>
    public class NUnitLiteTestCaseProvider : Extensibility.ITestCaseProvider
    {
        private DataAttributeTestCaseProvider provider = new DataAttributeTestCaseProvider();

        #region ITestCaseProvider Members

        /// <summary>
        /// Determine whether any test cases are available for a parameterized method.
        /// </summary>
        /// <param name="method">A MethodInfo representing a parameterized test</param>
        /// <returns>True if any cases are available, otherwise false.</returns>
        public bool HasTestCasesFor(System.Reflection.MethodInfo method)
        {
            return provider.HasTestCasesFor(method);
        }

        /// <summary>
        /// Return an IEnumerable providing test cases for use in
        /// running a paramterized test.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
#if CLR_2_0 || CLR_4_0
        public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
        {
            List<ITestCaseData> testCases = new List<ITestCaseData>();
#else
        public IEnumerable GetTestCasesFor(MethodInfo method)
        {
            ArrayList testCases = new ArrayList();
#endif

            foreach (DataAttribute attr in method.GetCustomAttributes(typeof(DataAttribute), false))
            {
                ITestCaseSource source = attr as ITestCaseSource;
                if (source != null)
                {
                    try
                    {
                        foreach (ITestCaseData testCase in ((ITestCaseSource)attr).GetTestCasesFor(method))
                            testCases.Add(testCase);
                        continue;
                    }
                    catch (System.Reflection.TargetInvocationException ex)
                    {
                        testCases.Add(new ParameterSet(ex.InnerException));
                    }
                    catch (System.Exception ex)
                    {
                        testCases.Add(new ParameterSet(ex));
                    }
                }
            }

            return testCases;
        }

        #endregion
    }
}
