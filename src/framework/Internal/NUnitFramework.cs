// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Collections.Specialized;
using System.Diagnostics;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Static methods that implement aspects of the NUnit framework that cut 
	/// across individual test types, extensions, etc. Some of these use the 
	/// methods of the Reflect class to implement operations specific to the 
	/// NUnit Framework.
	/// </summary>
	public class NUnitFramework
	{
        #region Properties
        private static Assembly frameworkAssembly;
        private static bool frameworkAssemblyInitialized;
        private static Assembly FrameworkAssembly
        {
            get
            {
                if (!frameworkAssemblyInitialized)
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        if (assembly.GetName().Name == "nunit.framework" ||
                            assembly.GetName().Name == "NUnitLite")
                        {
                            frameworkAssembly = assembly;
                            break;
                        }

                frameworkAssemblyInitialized = true;

                return frameworkAssembly;
            }
        }
        #endregion

        #region Check SetUp and TearDown methods
        public static bool CheckSetUpTearDownMethods(Type fixtureType, Type attributeType, ref string reason)
        {
            foreach( MethodInfo theMethod in Reflect.GetMethodsWithAttribute(fixtureType, attributeType, true ))
                if ( theMethod.IsAbstract ||
                     !theMethod.IsPublic && !theMethod.IsFamily ||
                     theMethod.GetParameters().Length > 0 ||
                     !theMethod.ReturnType.Equals(typeof(void)))
                {
                    reason = string.Format( "Invalid signature for SetUp or TearDown method: {0}", theMethod.Name );
                    return false;
                }

            return true;
        }
        #endregion

		#region ApplyCommonAttributes
        /// <summary>
        /// Modify a newly constructed test based on a type or method by 
        /// applying any of NUnit's common attributes.
        /// </summary>
        /// <param name="member">The type or method from which the test was constructed</param>
        /// <param name="test">The test to which the attributes apply</param>
        public static void ApplyCommonAttributes(MemberInfo member, Test test)
        {
            ApplyCommonAttributes( Reflect.GetAttributes( member, false ), test );
        }

        /// <summary>
        /// Modify a newly constructed test based on an assembly by applying 
        /// any of NUnit's common attributes.
        /// </summary>
        /// <param name="assembly">The assembly from which the test was constructed</param>
        /// <param name="test">The test to which the attributes apply</param>
        public static void ApplyCommonAttributes(Assembly assembly, Test test)
        {
            ApplyCommonAttributes( Reflect.GetAttributes( assembly, false ), test );
        }

        /// <summary>
        /// Modify a newly constructed test by applying any of NUnit's common
        /// attributes, based on an input array of attributes. This method checks
        /// for all attributes, relying on the fact that specific attributes can only
        /// occur on those constructs on which they are allowed.
        /// </summary>
        /// <param name="attributes">An array of attributes possibly including NUnit attributes
        /// <param name="test">The test to which the attributes apply</param>
        public static void ApplyCommonAttributes(Attribute[] attributes, Test test)
        {
            foreach (Attribute attribute in attributes)
            {
				Type attributeType = attribute.GetType();
				string attributeName = attributeType.FullName;
                bool isValid = test.RunState != RunState.NotRunnable;

                if (attribute is TestFixtureAttribute)
                {
                    if (test.Description == null)
                        test.Description = ((TestFixtureAttribute)attribute).Description;
                }
                else if (attribute is TestAttribute)
                {
                    if (test.Description == null)
                        test.Description = ((TestAttribute)attribute).Description;
                }
                else if (attribute is ISetRunState)
                {
                    if (isValid)
                    {
                        ISetRunState irs = (ISetRunState)attribute;
                        test.RunState = irs.GetRunState();
                        test.IgnoreReason = irs.GetReason();
                    }
                }
                else if (attribute is CategoryAttribute)
                {
                    test.Categories.Add(((CategoryAttribute)attribute).Name);
                }
                else if (attribute is PropertyAttribute)
                {
                    IDictionary props = ((PropertyAttribute)attribute).Properties;
                    if (props != null)
                        foreach (DictionaryEntry entry in props)
                            test.Properties.Add(entry.Key, entry.Value);
                }
            }
        }
		#endregion

        #region ApplyExpectedExceptionAttribute
        /// <summary>
        /// Modify a newly constructed test by checking for ExpectedExceptionAttribute
        /// and setting properties on the test accordingly.
        /// </summary>
        /// <param name="method">The method being processed, possibly annotated with an ExpectedExceptionAttribute</param>
        /// <param name="testMethod">The test to which the attributes apply</param>
        public static void ApplyExpectedExceptionAttribute(MethodInfo method, TestMethod testMethod)
        {
            ExpectedExceptionAttribute[] attributes = 
                (ExpectedExceptionAttribute[])method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                testMethod.ExceptionProcessor = new ExpectedExceptionProcessor(testMethod, attributes[0]);
        }

        #endregion

        #region GetResultState
        /// <summary>
        /// Returns a result state for a special exception.
        /// If the exception is not handled specially, returns
        /// ResultState.Error.
        /// </summary>
        /// <param name="ex">The exception to be examined</param>
        /// <returns>A ResultState</returns>
        public static ResultState GetResultState(Exception ex)
        {
            if (ex is System.Threading.ThreadAbortException)
                return ResultState.Cancelled;

            if (ex is AssertionException)
                return ResultState.Failure;
 
            if (ex is IgnoreException)
                return ResultState.Ignored;

            if (ex is InconclusiveException)
                return ResultState.Inconclusive;

            if (ex is SuccessException)
                return ResultState.Success;

            return ResultState.Error;
        }
        #endregion
    }
}
