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

namespace NUnit.Core
{
	/// <summary>
	/// Static methods that implement aspects of the NUnit framework that cut 
	/// across individual test types, extensions, etc. Some of these use the 
	/// methods of the Reflect class to implement operations specific to the 
	/// NUnit Framework.
	/// </summary>
	public class NUnitFramework
	{
        #region Constants

		#region Attribute Names
		// NOTE: Attributes used in switch statements must be const

        // Attributes that apply to Assemblies, Classes and Methods
        public const string IgnoreAttribute = "NUnit.Framework.IgnoreAttribute";
		public const string PlatformAttribute = "NUnit.Framework.PlatformAttribute";
		public const string CultureAttribute = "NUnit.Framework.CultureAttribute";
		public const string ExplicitAttribute = "NUnit.Framework.ExplicitAttribute";
        public const string CategoryAttribute = "NUnit.Framework.CategoryAttribute";
        public const string PropertyAttribute = "NUnit.Framework.PropertyAttribute";
		public const string DescriptionAttribute = "NUnit.Framework.DescriptionAttribute";
        public const string RequiredAddinAttribute = "NUnit.Framework.RequiredAddinAttribute";

        // Attributes that apply only to Classes
        public const string TestFixtureAttribute = "NUnit.Framework.TestFixtureAttribute";
        public const string SetUpFixtureAttribute = "NUnit.Framework.SetUpFixtureAttribute";

        // Attributes that apply only to Methods
        public const string TestAttribute = "NUnit.Framework.TestAttribute";
        public const string TestCaseAttribute = "NUnit.Framework.TestCaseAttribute";
        public const string TestCaseSourceAttribute = "NUnit.Framework.TestCaseSourceAttribute";
        public const string TheoryAttribute = "NUnit.Framework.TheoryAttribute";
        public static readonly string SetUpAttribute = "NUnit.Framework.SetUpAttribute";
        public static readonly string TearDownAttribute = "NUnit.Framework.TearDownAttribute";
        public static readonly string FixtureSetUpAttribute = "NUnit.Framework.TestFixtureSetUpAttribute";
        public static readonly string FixtureTearDownAttribute = "NUnit.Framework.TestFixtureTearDownAttribute";
        public static readonly string ExpectedExceptionAttribute = "NUnit.Framework.ExpectedExceptionAttribute";

        // Attributes that apply only to Properties
        public static readonly string SuiteAttribute = "NUnit.Framework.SuiteAttribute";
        #endregion

        #region Other Framework Types
        public static readonly string AssertException = "NUnit.Framework.AssertionException";
        public static readonly string IgnoreException = "NUnit.Framework.IgnoreException";
        public static readonly string InconclusiveException = "NUnit.Framework.InconclusiveException";
        public static readonly string SuccessException = "NUnit.Framework.SuccessException";
        public static readonly string AssertType = "NUnit.Framework.Assert";
		public static readonly string ExpectExceptionInterface = "NUnit.Framework.IExpectException";
        #endregion

        #region Core Types
        public static readonly string SuiteBuilderAttribute = typeof(SuiteBuilderAttribute).FullName;
        public static readonly string SuiteBuilderInterface = typeof(ISuiteBuilder).FullName;

        public static readonly string TestCaseBuilderAttributeName = typeof(TestCaseBuilderAttribute).FullName;
        public static readonly string TestCaseBuilderInterfaceName = typeof(ITestCaseBuilder).FullName;

        public static readonly string TestDecoratorAttributeName = typeof(TestDecoratorAttribute).FullName;
        public static readonly string TestDecoratorInterfaceName = typeof(ITestDecorator).FullName;
        #endregion

        #endregion

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
        public static bool CheckSetUpTearDownMethods(Type fixtureType, string attributeName, ref string reason)
        {
            foreach( MethodInfo theMethod in Reflect.GetMethodsWithAttribute(fixtureType, attributeName, true ))
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
                //else if (attribute is DescriptionAttribute)
                //{
                //    test.Description = ((DescriptionAttribute)attribute).Description;
                //}
                else if (attribute is ExplicitAttribute)
                {
                    if (isValid)
                    {
                        test.RunState = RunState.Explicit;
                        test.IgnoreReason = ((ExplicitAttribute)attribute).Reason;
                    }
                }
                else if (attribute is IgnoreAttribute)
                {
                    if (isValid)
                    {
                        test.RunState = RunState.Ignored;
                        test.IgnoreReason = ((ExplicitAttribute)attribute).Reason;
                    }
                }
                else if (attribute is PlatformAttribute)
                {
                    PlatformHelper pHelper = new PlatformHelper();
                    PlatformAttribute platformAttribute = (PlatformAttribute)attribute;
                    if (isValid && !pHelper.IsPlatformSupported(platformAttribute))
                    {
                        test.RunState = RunState.Skipped;
                        test.IgnoreReason = platformAttribute.Reason;
                        if (test.IgnoreReason == null)
                            test.IgnoreReason = pHelper.Reason;
                    }
                }
                else if (attribute is CultureAttribute)
                {
                    CultureDetector cultureDetector = new CultureDetector();
                    if (isValid && !cultureDetector.IsCultureSupported((CultureAttribute)attribute))
                    {
                        test.RunState = RunState.Skipped;
                        test.IgnoreReason = cultureDetector.Reason;
                    }
                }
                else if (attribute is RequiredAddinAttribute)
                {
                    string required = ((RequiredAddinAttribute)attribute).RequiredAddin;
                    if (!IsAddinAvailable(required))
                    {
                        test.RunState = RunState.NotRunnable;
                        test.IgnoreReason = string.Format("Required addin {0} not available", required);
                    }
                }
                else if (attribute is System.STAThreadAttribute)
                {
                    test.Properties.Add("APARTMENT_STATE", System.Threading.ApartmentState.STA);
                }
                else if (attribute is System.MTAThreadAttribute)
                {
                    test.Properties.Add("APARTMENT_STATE", System.Threading.ApartmentState.MTA);
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

        #region IsSuiteBuilder
        public static bool IsSuiteBuilder( Type type )
		{
			return type.IsDefined(typeof(SuiteBuilderAttribute), false )
				&& Reflect.HasInterface( type, SuiteBuilderInterface );
		}
		#endregion

		#region IsTestCaseBuilder
		public static bool IsTestCaseBuilder( Type type )
		{
			return type.IsDefined(typeof(TestCaseBuilderAttribute), false )
				&& Reflect.HasInterface( type, TestCaseBuilderInterfaceName );
		}
		#endregion

		#region IsTestDecorator
		public static bool IsTestDecorator( Type type )
		{
			return type.IsDefined(typeof(TestDecoratorAttribute), false )
				&& Reflect.HasInterface( type, TestDecoratorInterfaceName );
		}
		#endregion

        #region IsAddinAvailable
        public static bool IsAddinAvailable(string name)
        {
            foreach (Addin addin in CoreExtensions.Host.AddinRegistry.Addins)
                if (addin.Name == name && addin.Status == AddinStatus.Loaded)
                    return true;

            return false;
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

            string name = ex.GetType().FullName;

            if (name == NUnitFramework.AssertException)
                return ResultState.Failure;
            else
                if (name == NUnitFramework.IgnoreException)
                    return ResultState.Ignored;
                else
                    if (name == NUnitFramework.InconclusiveException)
                        return ResultState.Inconclusive;
                    else
                        if (name == NUnitFramework.SuccessException)
                            return ResultState.Success;
                        else
                            return ResultState.Error;
        }
        #endregion
    }
}
