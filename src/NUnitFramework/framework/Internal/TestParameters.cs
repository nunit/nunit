// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
    /// TestParameters is the abstract base class for all classes
    /// that know how to provide data for constructing a test.
    /// </summary>
    public abstract class TestParameters : ITestData, IApplyToTest
    {
        #region Constructors

        /// <summary>
        /// Default Constructor creates an empty parameter set
        /// </summary>
        public TestParameters()
        {
            RunState = RunState.Runnable;
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public TestParameters(object[] args)
        {
            RunState = RunState.Runnable;
            InitializeAguments(args);
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public TestParameters(Exception exception)
        {
            RunState = RunState.NotRunnable;
            Properties = new PropertyBag();

            Properties.Set(PropertyNames.SkipReason, ExceptionHelper.BuildMessage(exception));
            Properties.Set(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestData
        /// </summary>
        /// <param name="data"></param>
        public TestParameters(ITestData data)
        {
            RunState = data.RunState;
            Properties = new PropertyBag();

            TestName = data.TestName;

            InitializeAguments(data.Arguments);

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }

        private void InitializeAguments(object[] args)
        {
            OriginalArguments = args;

            // We need to copy args, since we may change them
            var numArgs = args.Length;
            Arguments = new object[numArgs];
            Array.Copy(args, Arguments, numArgs);
        }

        #endregion

        #region ITestData Members

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
        }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object[] OriginalArguments { get; private set; }

        #endregion
    }
}
