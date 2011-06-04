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

using System;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestMethod class represents a Test implemented as a method.
    /// Because of how exceptions are handled internally, this class
    /// must incorporate processing of expected exceptions. A change to
    /// the Test interface might make it easier to process exceptions
    /// in an object that aggregates a TestMethod in the future.
    /// </summary>
	public class TestMethod : Test
	{
		#region Fields
		/// <summary>
		/// The test method
		/// </summary>
		internal MethodInfo method;

        /// <summary>
        /// The ExpectedExceptionProcessor for this test, if any
        /// </summary>
        internal ExpectedExceptionProcessor exceptionProcessor;

        /// <summary>
        /// Arguments to be used in invoking the method
        /// </summary>
	    internal object[] arguments;

        /// <summary>
        /// The expected result of the method return value
        /// </summary>
	    internal object expectedResult;

        /// <summary>
        /// Indicated whether the method has an expected result.
        /// </summary>
	    internal bool hasExpectedResult;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
		public TestMethod( MethodInfo method ) 
			: base( method.ReflectedType ) 
		{
            this.Name = method.Name;
            this.FullName += "." + this.Name;

            // Disambiguate call to base class methods
            // TODO: This should not be here - it's a presentation issue
            if( method.DeclaringType != method.ReflectedType)
                this.Name = method.DeclaringType.Name + "." + method.Name;

            this.method = method;
		}
		#endregion

        #region Properties
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method that performs the test.</value>
		public MethodInfo Method
		{
			get { return method; }
		}

        /// <summary>
        /// Gets or sets the exception processor.
        /// </summary>
        /// <value>The exception processor.</value>
        public ExpectedExceptionProcessor ExceptionProcessor
        {
            get { return exceptionProcessor; }
            set { exceptionProcessor = value; }
        }

        /// <summary>
        /// Gets a value indicating whether an exception is expected.
        /// </summary>
        /// <value><c>true</c> if an exception is expected; otherwise, <c>false</c>.</value>
		public bool ExceptionExpected
		{
            get { return exceptionProcessor != null; }
		}

        #endregion

        #region Test Overrides

        /// <summary>
        /// Overridden to return a TestCaseResult.
        /// </summary>
        /// <returns>A TestResult for this test.</returns>
        public override TestResult MakeTestResult()
        {
            return new TestCaseResult(this);
        }

        /// <summary>
        /// Gets a bool indicating whether the current test
        /// has any descendant tests.
        /// </summary>
        public override bool HasChildren
        {
            get { return false; }
        }

#if !NUNITLITE
        /// <summary>
        /// Gets a boolean value indicating whether this 
        /// test should run on it's own thread.
        /// </summary>
        internal override bool ShouldRunOnOwnThread
        {
            get
            {
                if (base.ShouldRunOnOwnThread)
                    return true;

                int timeout = TestExecutionContext.CurrentContext.TestCaseTimeout;
                if (Properties.ContainsKey(PropertyNames.Timeout))
                    timeout = (int)Properties.Get(PropertyNames.Timeout);

                return timeout > 0;
            }
        }
#endif

        /// <summary>
        /// Returns an XmlNode representing the current result after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            XmlNode thisNode = XmlHelper.AddElement(parentNode, "test-case");

            PopulateTestNode(thisNode, recursive);

            return thisNode;
        }

		/// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
#if CLR_2_0 || CLR_4_0
        public override System.Collections.Generic.IList<ITest> Tests
#else
        public override System.Collections.IList Tests
#endif
        {
            get { return new ITest[0]; }
        }

        /// <summary>
        /// The name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string TestKind
        {
            get { return "test-case"; }
        }

        #endregion
    }
}
