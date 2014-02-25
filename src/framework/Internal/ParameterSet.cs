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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterSet encapsulates method arguments and
    /// other selected parameters needed for constructing
    /// a parameterized test case.
    /// </summary>
    public class ParameterSet : ITestCaseData, IApplyToTest
    {
        #region Instance Fields

        /// <summary>
        /// The expected result to be returned
        /// </summary>
        private object _expectedResult;

        private ExpectedExceptionData _exceptionData;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public ParameterSet(Exception exception)
        {
            this.RunState = RunState.NotRunnable;
            this.Properties = new PropertyBag();
            this.Properties.Set(PropertyNames.SkipReason, ExceptionHelper.BuildMessage(exception));
            this.Properties.Set(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
        }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public ParameterSet(object[] args)
        {
            this.Arguments = this.OriginalArguments = args;
            this.RunState = RunState.Runnable;
            this.Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestCaseData
        /// </summary>
        /// <param name="data"></param>
        public ParameterSet(ITestCaseData data)
            : this((ITestExpectedResult)data)
        {
            this.TestName = data.TestName;
            this.RunState = data.RunState;
            this.Arguments = this.OriginalArguments = data.Arguments;
            this.ExceptionData = data.ExceptionData;

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing <see cref="ITestExpectedResult"/>
        /// </summary>
        /// <param name="data"></param>
        public ParameterSet(ITestExpectedResult data)
        {
            RunState = RunState.Runnable;
            if (data.HasExpectedResult)
                ExpectedResult = data.ExpectedResult;

            Properties = new PropertyBag();
        }

        #endregion

        #region ITestCaseData Members

        /// <summary>
        /// The RunState for this set of parameters.
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// The arguments to be used in running the test,
        /// which must match the method signature.
        /// </summary>
        public object[] Arguments { get; internal set; }

        /// <summary>
        /// The expected result of the test, which
        /// must match the method return type.
        /// </summary>
        public object ExpectedResult
        {
            get { return _expectedResult; }
            set
            {
                _expectedResult = value;
                HasExpectedResult = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an expected result was specified.
        /// </summary>
        public bool HasExpectedResult { get; set; }

        /// <summary>
        /// Data about any expected exception
        /// </summary>
        public ExpectedExceptionData ExceptionData
        {
            get { return _exceptionData; }
            protected set { _exceptionData = value; }
        }

        /// <summary>
        /// Sets the name of the expected exception.
        /// </summary>
        /// <param name="expectedExceptionName">Name of the expected exception.</param>
        protected void SetExpectedExceptionName(string expectedExceptionName)
        {
            _exceptionData.ExpectedExceptionName = expectedExceptionName;
        }

        /// <summary>
        /// A name to be used for this test case in lieu
        /// of the standard generated name containing
        /// the argument list.
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets the property dictionary for this test
        /// </summary>
        public IPropertyBag Properties { get; private set; }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object[] OriginalArguments { get; private set; }

        /// <summary>
        /// Gets a flag indicating whether an exception is expected.
        /// </summary>
        public bool ExceptionExpected
        {
            get { return ExceptionData.ExpectedExceptionName != null; }
        }

        #endregion

        #region IApplyToTest Members

        /// <summary>
        /// Applies ParameterSet _values to the test itself.
        /// </summary>
        /// <param name="test">A test.</param>
        public void ApplyToTest(Test test)
        {
            if (this.RunState != RunState.Runnable)
                test.RunState = this.RunState;

            foreach (string key in Properties.Keys)
                foreach (object value in Properties[key])
                    test.Properties.Add(key, value);

            TestMethod testMethod = test as TestMethod;
            if (testMethod != null && ExceptionData.ExpectedExceptionName != null)
                testMethod.CustomDecorators.Add(new ExpectedExceptionDecorator(this.ExceptionData));
        }

        #endregion
    }
}
