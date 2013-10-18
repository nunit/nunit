// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

#if !NUNITLITE
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Extensibility;

namespace NUnit.Framework.Internal.Extensibility
{
	/// <summary>
	/// TestCaseBuilderCollection is an ExtensionPoint for TestCaseBuilders 
	/// and implements the ITestCaseBuilder interface itself, passing calls 
	/// on to the individual builders.
	/// 
	/// The builders are added to the collection by inserting them at
	/// the start, as to take precedence over those added earlier. 
	/// </summary>
	public class TestCaseBuilderCollection : ExtensionPoint, ITestCaseBuilder2
	{
		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseBuilderCollection"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
		public TestCaseBuilderCollection(IExtensionHost host)
			: base("TestCaseBuilders", host) { }

		#endregion

        #region ITestCaseBuilder Members
        /// <summary>
        /// Examine the method and determine if it is suitable for
        /// any TestCaseBuilder to use in building a Test
        /// </summary>
        /// <param name="method">The method to be used as a test case</param>
        /// <returns>True if the method can be used to build a Test</returns>
        public bool CanBuildFrom(MethodInfo method)
        {
            foreach (ITestCaseBuilder builder in Extensions)
                if (builder.CanBuildFrom(method))
                    return true;
            return false;
        }

        /// <summary>
        /// Build a Test from the method provided.
        /// </summary>
        /// <param name="method">The method to be used</param>
        /// <returns>A Test or null</returns>
        public Test BuildFrom(MethodInfo method)
        {
            foreach (ITestCaseBuilder builder in Extensions)
            {
                if (builder.CanBuildFrom(method))
                    return builder.BuildFrom(method);
            }

            return null;
        }
        #endregion

        #region ITestCaseBuilder2 Members

        /// <summary>
        /// Examine the method and determine if it is suitable for
        /// any TestCaseBuilder to use in building a Test
        /// </summary>
        /// <param name="method">The method to be used as a test case</param>
        /// <param name="suite">The test suite for which the method is being built</param>
        /// <returns>True if the method can be used to build a Test</returns>
        public bool CanBuildFrom(MethodInfo method, Test suite)
        {
            foreach (ITestCaseBuilder builder in Extensions)
            {
                if (builder is ITestCaseBuilder2)
                {
                    ITestCaseBuilder2 builder2 = (ITestCaseBuilder2)builder;
                    if (builder2.CanBuildFrom(method, suite))
                        return true;
                }
                else if (builder.CanBuildFrom(method))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Build a Test from the method provided.
        /// </summary>
        /// <param name="method">The method to be used</param>
        /// <param name="suite">The test suite for which the method is being built</param>
        /// <returns>A Test or null</returns>
        public Test BuildFrom(MethodInfo method, Test suite)
        {
            foreach (ITestCaseBuilder builder in Extensions)
            {
                if (builder is ITestCaseBuilder2)
                {
                    ITestCaseBuilder2 builder2 = (ITestCaseBuilder2)builder;
                    if (builder2.CanBuildFrom(method, suite))
                        return builder2.BuildFrom(method, suite);
                }
                else if (builder.CanBuildFrom(method))
                    return builder.BuildFrom(method);
            }

            return null;
        }
        #endregion

        #region ExtensionPoint Overrides

        /// <summary>
        /// Determines whether an objext is a valid TestCaseBuilder extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// 	<c>true</c> if the specified extension is a valid TestCaseBuilder; otherwise, <c>false</c>.
        /// </returns>
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestCaseBuilder; 
		}

		#endregion
	}
}
#endif
