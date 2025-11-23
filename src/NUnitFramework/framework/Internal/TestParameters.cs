// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        internal static readonly object[] NoArguments = Array.Empty<object>();

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
                Properties[key] = data.Properties[key];
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

        /// <summary>
        /// A name to be used for this test case in lieu
        /// of the standard generated name containing
        /// the argument list.
        /// </summary>
        public string? TestName
        {
            get;
            set
            {
                Guard.OperationValid(ArgDisplayNames is null || value is null,
                    "TestName cannot be set when argument display names are set.");
                field = value;
            }
        }

        /// <summary>
        /// Gets the property dictionary for this test
        /// </summary>
        public IPropertyBag Properties { get; protected internal set; }

        /// <summary>
        /// Applies ParameterSet values to the test itself.
        /// </summary>
        /// <param name="test">A test.</param>
        public void ApplyToTest(Test test)
        {
            if (RunState != RunState.Runnable)
                test.RunState = RunState;

            foreach (string key in Properties.Keys)
            {
                foreach (object value in Properties[key])
                    test.Properties.Add(key, value);
            }
        }

        /// <summary>
        /// The original arguments provided by the user,
        /// used for display purposes.
        /// </summary>
        public object?[] OriginalArguments { get; protected internal set; }

        /// <summary>
        /// The list of display names to use as the parameters in the test name.
        /// </summary>
        internal string[]? ArgDisplayNames
        {
            get;
            set
            {
                Guard.OperationValid(TestName is null || value is null,
                    "Argument display names cannot be set when TestName is set.");
                field = value;
            }
        }
    }
}
