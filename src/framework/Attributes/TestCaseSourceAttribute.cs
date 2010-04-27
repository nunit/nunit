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
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
#if CLR_2_0
using System.Collections.Generic;
#endif

namespace NUnit.Framework
{
    /// <summary>
    /// FactoryAttribute indicates the source to be used to
    /// provide test cases for a test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseSourceAttribute : NUnitAttribute, ITestCaseSource
    {
        private readonly string sourceName;
        private readonly Type sourceType;

        /// <summary>
        /// Construct with the name of the factory - for use with languages
        /// that don't support params arrays.
        /// </summary>
        /// <param name="sourceName">An array of the names of the factories that will provide data</param>
        public TestCaseSourceAttribute(string sourceName)
        {
            this.sourceName = sourceName;
        }

        /// <summary>
        /// Construct with a Type and name - for use with languages
        /// that don't support params arrays.
        /// </summary>
        /// <param name="sourceType">The Type that will provide data</param>
        /// <param name="sourceName">The name of the method, property or field that will provide data</param>
        public TestCaseSourceAttribute(Type sourceType, string sourceName)
        {
            this.sourceType = sourceType;
            this.sourceName = sourceName;
        }

        /// <summary>
        /// The name of a the method, property or fiend to be used as a source
        /// </summary>
        public string SourceName
        {
            get { return sourceName; }   
        }

        /// <summary>
        /// A Type to be used as a source
        /// </summary>
        public Type SourceType
        {
            get { return sourceType;  }
        }

        #region ITestCaseSource Members
#if CLR_2_0
        public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
        {
            List<ITestCaseData> data = new List<ITestCaseData>();
#else
        public IEnumerable GetTestCasesFor(MethodInfo method)
        {
            ArrayList data = new ArrayList();
#endif
            Type sourceType = this.sourceType;
            if (sourceType == null)
                sourceType = method.ReflectedType;

            IEnumerable source = null;
            MemberInfo[] members = sourceType.GetMember(sourceName, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (members.Length == 1)
            {
                MemberInfo member = members[0];
                object sourceobject = Internal.Reflect.Construct(sourceType);
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo field = member as FieldInfo;
                        source = (IEnumerable)field.GetValue(sourceobject);
                        break;
                    case MemberTypes.Property:
                        PropertyInfo property = member as PropertyInfo;
                        source = (IEnumerable)property.GetValue(sourceobject, null);
                        break;
                    case MemberTypes.Method:
                        MethodInfo m = member as MethodInfo;
                        source = (IEnumerable)m.Invoke(sourceobject, null);
                        break;
                }

                int nparms = method.GetParameters().Length;

                foreach (object obj in source)
                {
                    ITestCaseData testCase = obj as ITestCaseData;
                    if (testCase != null)
                        data.Add(testCase);
                    else
                    {
                        ParameterSet parms = new ParameterSet();
                        object[] array = obj as object[];
                        parms.Arguments = array != null && array.Length == nparms
                            ? array
                            : new object[] { obj };

                        data.Add(parms);
                    }
                }
            }

            return data;
        }

        #endregion
    }
}
