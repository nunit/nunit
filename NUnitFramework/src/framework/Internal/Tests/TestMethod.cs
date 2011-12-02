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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;

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
        /// A list of all decorators applied to the test by attributes or parameterset arguments
        /// </summary>
        private List<ICommandDecorator> decorators = new System.Collections.Generic.List<ICommandDecorator>();

        /// <summary>
        /// Indicated whether the method has an expected result.
        /// </summary>
	    internal bool hasExpectedResult;

        /// <summary>
        /// The result that the test method is expected to return.
        /// </summary>
        internal object expectedResult;

		#endregion

		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        public TestMethod(MethodInfo method) : this(method, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="parentSuite">The suite or fixture to which the new test will be added</param>
        public TestMethod(MethodInfo method, Test parentSuite) 
			: base( method.ReflectedType ) 
		{
            this.Name = method.Name;
            this.FullName += "." + this.Name;

            // Disambiguate call to base class methods
            // TODO: This should not be here - it's a presentation issue
            if( method.DeclaringType != method.ReflectedType)
                this.Name = method.DeclaringType.Name + "." + method.Name;

            // Needed to give proper fullname to test in a parameterized fixture.
            // Without this, the arguments to the fixture are not included.
            string prefix = method.ReflectedType.FullName;
            if (parentSuite != null)
            {
                prefix = parentSuite.FullName;
                this.FullName = prefix + "." + this.Name;
            }

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
        /// Gets a list of custom decorators for this test.
        /// </summary>
        public IList<ICommandDecorator> CustomDecorators
        {
            get { return decorators; }
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
                // TODO: Remove this kluge!
                else if (Parent != null && Parent.Properties.ContainsKey(PropertyNames.Timeout))
                    timeout = (int)Parent.Properties.Get(PropertyNames.Timeout);

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
            XmlNode thisNode = XmlHelper.AddElement(parentNode, XmlElementName);

            PopulateTestNode(thisNode, recursive);

            return thisNode;
        }

		/// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
        public override IList<ITest> Tests
        {
            get { return new ITest[0]; }
        }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string XmlElementName
        {
            get { return "test-case"; }
        }

        protected override TestCommand MakeTestCommand(ITestFilter filter)
        {
            TestCommand command = new TestMethodCommand(this);

            command = ApplyDecoratorsToCommand(command);

            return command;
        }

        #endregion

        #region Helper Methods

        private TestCommand ApplyDecoratorsToCommand(TestCommand command)
        {
            CommandDecoratorList decorators = new CommandDecoratorList();

            // Add Standard stuff
            decorators.Add(new SetUpTearDownDecorator());

#if !NUNITLITE
            if (ShouldRunOnOwnThread)
                decorators.Add(new ThreadedTestDecorator());
#endif

            // Add Decorators supplied by attributes and parameter sets
            foreach (ICommandDecorator decorator in CustomDecorators)
                decorators.Add(decorator);

            decorators.OrderByStage();

            foreach (ICommandDecorator decorator in decorators)
            {
                command = decorator.Decorate(command);
            }

            return command;
        }

        #endregion
    }
}
