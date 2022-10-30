// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// When implemented by an attribute, this interface implemented to provide actions to execute before and after tests.
    /// </summary>
    public interface ITestAction
    {
        /// <summary>
        /// Executed before each test is run
        /// </summary>
        /// <param name="test">The test that is going to be run.</param>
        void BeforeTest(ITest test);

        /// <summary>
        /// Executed after each test is run
        /// </summary>
        /// <param name="test">The test that has just been run.</param>
        void AfterTest(ITest test);


        /// <summary>
        /// Provides the target for the action attribute
        /// </summary>
        /// <returns>The target for the action attribute</returns>
        ActionTargets Targets { get; }
    }
}
