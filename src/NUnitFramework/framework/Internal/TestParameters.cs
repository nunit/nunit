// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
    public abstract class TestParameters : ITestData
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
            InitializeArguments(args);
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

            InitializeArguments(data.Arguments);

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }

        private void InitializeArguments(object[] args)
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
        /// If not <see langword="null"/>, overrides the list of argument display names to be used when generating a test name.
        /// </summary>
        protected string[] ArgNames { private get; set; }

        /// <summary>
        /// Gets the property dictionary for this test
        /// </summary>
        public IPropertyBag Properties { get; private set; }

        #endregion

        /// <summary>
        /// Applies the encapsulated parameters to the test method.
        /// </summary>
        public void ApplyToTest(TestMethod test, TestNameGenerator defaultTestNameGenerator)
        {
            if (RunState != RunState.Runnable)
                test.RunState = RunState;

            foreach (string key in Properties.Keys)
                foreach (object value in Properties[key])
                    test.Properties.Add(key, value);

            if (TestName == null)
            {
                test.Name = defaultTestNameGenerator.GetDisplayName(test, ArgNames, ArgNames != null ? null : OriginalArguments);
            }
            else
            {
                test.Name = TestName.Contains("{")
                    ? new TestNameGenerator(TestName).GetDisplayName(test, ArgNames, ArgNames != null ? null : OriginalArguments)
                    : TestName;
            }
        }

        #region Other Public Properties

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object[] OriginalArguments { get; private set; }

        #endregion
    }
}
