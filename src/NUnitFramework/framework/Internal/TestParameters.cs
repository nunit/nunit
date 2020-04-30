// ***********************************************************************
// Copyright (c) 2015–2018 Charlie Poole, Rob Prouse
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

#nullable enable

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
        internal static readonly object[] NoArguments =
#if NET35 || NET40 || NET45 // New in net46
            new object[0];
#else
            Array.Empty<object>();
#endif

        /// <summary>
        /// Default Constructor creates an empty parameter set
        /// </summary>
        public TestParameters()
            : this(RunState.Runnable, NoArguments)
        {
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a parameter set with a list of arguments
        /// </summary>
        /// <param name="args"></param>
        public TestParameters(object?[] args)
            : this(RunState.Runnable, args)
        {
            Properties = new PropertyBag();
        }

        /// <summary>
        /// Construct a non-runnable ParameterSet, specifying
        /// the provider exception that made it invalid.
        /// </summary>
        public TestParameters(Exception exception)
            : this(RunState.NotRunnable, NoArguments)
        {
            Properties = new PropertyBag();
            Properties.Set(PropertyNames.SkipReason, ExceptionHelper.BuildMessage(exception));
            Properties.Set(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
        }

        /// <summary>
        /// Construct a ParameterSet from an object implementing ITestData
        /// </summary>
        /// <param name="data"></param>
        public TestParameters(ITestData data)
            : this(data.RunState, data.Arguments)
        {
            TestName = data.TestName;

            foreach (string key in data.Properties.Keys)
                this.Properties[key] = data.Properties[key];
        }

        private TestParameters(RunState runState, object?[] args)
        {
            RunState = runState;

            OriginalArguments = args;

            // We need to copy args, since we may change them
            var numArgs = args.Length;
            Arguments = new object?[numArgs];
            Array.Copy(args, Arguments, numArgs);

            Properties = new PropertyBag();
        }

        /// <summary>
        /// The RunState for this set of parameters.
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// The arguments to be used in running the test,
        /// which must match the method signature.
        /// </summary>
        public object?[] Arguments { get; internal set; }

        private string? _testName;

        /// <summary>
        /// A name to be used for this test case in lieu
        /// of the standard generated name containing
        /// the argument list.
        /// </summary>
        public string? TestName
        {
            get
            {
                return _testName;
            }
            set
            {
                Guard.OperationValid(ArgDisplayNames == null || value == null, "TestName cannot be set when argument display names are set.");
                _testName = value;
            }
        }

        /// <summary>
        /// Gets the property dictionary for this test
        /// </summary>
        public IPropertyBag Properties { get; }

        /// <summary>
        /// Applies ParameterSet values to the test itself.
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

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object?[] OriginalArguments { get; private set; }

        private string[]? _argDisplayNames;

        /// <summary>
        /// The list of display names to use as the parameters in the test name.
        /// </summary>
        internal string[]? ArgDisplayNames
        {
            get
            {
                return _argDisplayNames;
            }
            set
            {
                Guard.OperationValid(TestName == null || value == null, "Argument display names cannot be set when TestName is set.");
                _argDisplayNames = value;
            }
        }
    }
}
