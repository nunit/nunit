// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract attribute providing actions to execute before and after tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class TestActionAttribute : Attribute, ITestAction
    {
        /// <summary>
        /// Executed before each test is run
        /// </summary>
        /// <param name="test">The test that is going to be run.</param>
        public virtual void BeforeTest(ITest test) { }

        /// <summary>
        /// Executed after each test is run
        /// </summary>
        /// <param name="test">The test that has just been run.</param>
        public virtual void AfterTest(ITest test) { }

        /// <summary>
        /// Provides the target for the action attribute
        /// </summary>
        public virtual ActionTargets Targets
        {
            get { return ActionTargets.Default; }
        }
    }
}
