// ****************************************************************
// Copyright 2010, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************
using System;
using System.Collections;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Provide the context information of the current test
    /// </summary>
    public class TestContext
    {
        private const string contextKey = "NUnit.Framework.TestContext";
        private const string dictionaryKey = "NUnit.Framework.TestContext.ContextDictionary";
        private const string resultKey = "Result";

        private static TestContext _current;

        internal Test _test;
        internal TestResult _result;
        
        public static TestContext CurrentContext
        {
            get 
            {
                if (_current == null)
                    _current = new TestContext();

                return _current; 
            }
        }

        public TestRepresentation Test
        {
            get
            {
                return new TestRepresentation(_test);
            }
        }

        /// <summary>
        /// Gets a ResultState representing the outcome of the current test. 
        /// </summary>
        public ResultState Result
        {
            get
            {
                return _result.ResultState;
            }
        }

        #region Nested Classes
        public class TestRepresentation
        {
            private Test test;

            public TestRepresentation(Test test)
            {
                this.test = test;
            }

            /// <summary>
            /// The name of the currently executing test. If no
            /// test is running, the name of the last test run.
            /// </summary>
            public string Name
            {
                get
                {
                    return test.Name;
                }
            }

            /// <summary>
            /// The properties of the currently executing test
            /// or, if no test is running, of the last test run.
            /// </summary>
            public IDictionary Properties
            {
                get
                {
                    return test.Properties;
                }
            }
        }
        #endregion
    }
}
