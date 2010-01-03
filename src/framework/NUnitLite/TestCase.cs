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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// TestCase represents a single executable test
    /// </summary>
    public class TestCase : ITest
    {
        #region Instance Variables
        private string name;
        private string fullName;

        private object fixture;
        private MethodInfo method;

        private MethodInfo setup;
        private MethodInfo teardown;

        private RunState runState = RunState.Runnable;
        private string ignoreReason;

        private IDictionary properties;

        private object[] args;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="name">The name of the test.</param>
        public TestCase(string name)
        {
            this.name = this.fullName = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="method">The method implementing this test case.</param>
        public TestCase(MethodInfo method)
        {
            Initialize(method, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="fixture">The fixture.</param>
        public TestCase(string name, object fixture)
        {
            Initialize(fixture.GetType().GetMethod(name), fixture, null);
        }

        private void Initialize(MethodInfo method, object fixture, object[] args)
        {
            this.method = method;
            this.args = args;

            this.name = MethodHelper.GetDisplayName(method, args);

            this.fullName = method.ReflectedType.FullName + "." + this.name;
            this.fixture = fixture;
            if ( fixture == null )
                this.fixture = Reflect.Construct(method.ReflectedType, null);

            if (HasValidSignature(method, args))
            {
                IgnoreAttribute ignore = (IgnoreAttribute)Reflect.GetAttribute(this.method, typeof(IgnoreAttribute));

                if (ignore != null)
                {
                    this.runState = RunState.Ignored;
                    this.ignoreReason = ignore.GetReason();
                }
            }

            foreach (MethodInfo m in method.ReflectedType.GetMethods())
            {
                if (m.IsDefined(typeof(SetUpAttribute), true))
                    this.setup = m;

                if (m.IsDefined(typeof(TearDownAttribute), true))
                    this.teardown = m;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="method">The method implementing the test case.</param>
        /// <param name="args">The args to be used in calling the method.</param>
        public TestCase(MethodInfo method, object[] args)
        {
            Initialize(method, null, args);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the test.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the full name of the test.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// Gets or sets the run state of the test.
        /// </summary>
        /// <value>The state of the run.</value>
        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

        /// <summary>
        /// Gets or sets the ignore reason.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return ignoreReason; }
            set { ignoreReason = value; }
        }

        /// <summary>
        /// Gets the properties of the test.
        /// </summary>
        /// <value>The properties dictionary.</value>
        public System.Collections.IDictionary Properties
        {
            get 
            {
                if (properties == null)
                {
                    properties = new Hashtable();

                    object[] attrs = this.method.GetCustomAttributes(typeof(PropertyAttribute), true);
                    foreach (PropertyAttribute attr in attrs)
                        foreach( DictionaryEntry entry in attr.Properties )
                            this.Properties[entry.Key] = entry.Value;
                }

                return properties; 
            }
        }

        /// <summary>
        /// Gets the test case count.
        /// </summary>
        /// <value>The test case count.</value>
        public int TestCaseCount
        {
            get { return 1; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Runs this test.
        /// </summary>
        /// <returns>A TestResult</returns>
        public TestResult Run()
        {
            return Run( new NullListener() );
        }

        /// <summary>
        /// Runs this test
        /// </summary>
        /// <param name="listener">A TestListener to handle test events</param>
        /// <returns>A TestResult</returns>
        public TestResult Run(TestListener listener)
        {
            listener.TestStarted(this);

            TestResult result = new TestResult(this);
            Run(result, listener);

            listener.TestFinished(result);

            return result;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Performs SetUp for the test.
        /// </summary>
        protected virtual void SetUp() 
        {
            if (setup != null)
            {
                Assert.That(HasValidSetUpTearDownSignature(setup), "Invalid SetUp method: must return void and have no arguments");
                InvokeMethod(setup);
            }
        }

        /// <summary>
        /// Performs TearDown for the test.
        /// </summary>
        protected virtual void TearDown() 
        {
            if (teardown != null)
            {
                Assert.That(HasValidSetUpTearDownSignature(teardown), "Invalid TearDown method: must return void and have no arguments");
                InvokeMethod(teardown);
            }
        }

        /// <summary>
        /// Runs the test and handles any exceptions.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="listener">The listener.</param>
        protected virtual void Run(TestResult result, TestListener listener)
        {
            IgnoreAttribute ignore = (IgnoreAttribute)Reflect.GetAttribute(method, typeof(IgnoreAttribute));
            if (this.RunState == RunState.NotRunnable)
                result.Failure(this.ignoreReason);
            else if ( ignore != null )
                result.Ignore(ignore.GetReason());
            else
            {
                try
                {
                    RunBare();
                    result.Success();
                }
                catch (NUnitLiteException nex)
                {
                    result.RecordException(nex.InnerException);
                }
#if !NETCF_1_0
                catch (System.Threading.ThreadAbortException)
                {
                    throw;
                }
#endif
                catch (Exception ex)
                {
                    result.RecordException(ex);
                }
            }
        }

        /// <summary>
        /// Runs SetUp, invokes the test and runs TearDown.
        /// </summary>
        protected void RunBare()
        {
            SetUp();
            try
            {
                RunTest();
            }
            finally
            {
                TearDown();
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        protected virtual void RunTest()
        {
            try
            {
                InvokeMethod( this.method, this.args );
                ProcessNoException(this.method);
            }
            catch (NUnitLiteException ex)
            {
                ProcessException(this.method, ex.InnerException);
            }
        }

        /// <summary>
        /// Invokes a method on the test fixture.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        protected void InvokeMethod(MethodInfo method, params object[] args)
        {
            Reflect.InvokeMethod(method, this.fixture, args);
        }
        #endregion

        #region Private Methods       
        /// <summary>
        /// Determines whether the method has a valid signature and sets
        /// the RunState to NotRunnable if it does not.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <returns>
        /// 	<c>true</c> if the signature is valid; otherwise, <c>false</c>.
        /// </returns>
        private bool HasValidSignature(MethodInfo method, object[] args)
        {
            if (method.ReturnType != typeof(void))
            {
                this.runState = RunState.NotRunnable;
                this.ignoreReason = "A TestMethod must return void";
                return false;
            }
            
            int argsNeeded = method.GetParameters().Length;
            int argsPassed = args == null ? 0 : args.Length;

            if (argsNeeded == 0 && argsPassed > 0)
            {
                this.runState = RunState.NotRunnable;
                this.ignoreReason = "Arguments may not be specified for a method with no parameters";
                return false;
            }

            if (argsNeeded > 0 && argsPassed == 0)
            {
                this.runState = RunState.NotRunnable;
                this.ignoreReason = "No arguments provided for a method requiring them";
                return false;
            }

            if (argsNeeded != argsPassed)
            {
                this.runState = RunState.NotRunnable;
                this.ignoreReason = string.Format("Expected {0} arguments, but received {1}", argsNeeded, argsPassed);
                return false;
            }

            return true;
        }

        private static bool HasValidSetUpTearDownSignature(MethodInfo method)
        {
            return method.ReturnType == typeof(void)
                && method.GetParameters().Length == 0; ;
        }

        private static void ProcessNoException(MethodInfo method)
        {
            ExpectedExceptionAttribute exceptionAttribute =
                (ExpectedExceptionAttribute)Reflect.GetAttribute(method, typeof(ExpectedExceptionAttribute));

            if (exceptionAttribute != null)
                Assert.Fail("Expected Exception of type <{0}>, but none was thrown", exceptionAttribute.ExpectedException);
        }

        private void ProcessException(MethodInfo method, Exception caughtException)
        {
            ExpectedExceptionAttribute exceptionAttribute =
                (ExpectedExceptionAttribute)Reflect.GetAttribute(method, typeof(ExpectedExceptionAttribute));

            if (exceptionAttribute == null)
                throw new NUnitLiteException("", caughtException);

            Type expectedType = exceptionAttribute.ExpectedException;
            if ( expectedType != null && expectedType != caughtException.GetType() )
                Assert.Fail("Expected Exception of type <{0}>, but was <{1}>", exceptionAttribute.ExpectedException, caughtException.GetType());

            MethodInfo handler = GetExceptionHandler(method.ReflectedType, exceptionAttribute.Handler);

            if (handler != null)
                InvokeMethod( handler, caughtException );
        }

        private MethodInfo GetExceptionHandler(Type type, string handlerName)
        {
            if (handlerName == null && Reflect.HasInterface( type, typeof(IExpectException) ) )
                handlerName = "HandleException";

            if (handlerName == null)
                return null;

            MethodInfo handler = Reflect.GetMethod( type, handlerName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                new Type[] { typeof(Exception) });

            if (handler == null)
                Assert.Fail("The specified exception handler {0} was not found", handlerName);

            return handler;
        }
        #endregion
    }
}
