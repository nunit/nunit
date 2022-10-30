// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Api
{
    /// <summary>
    /// The ITestAssemblyBuilder interface is implemented by a class
    /// that is able to build a suite of tests given an assembly or 
    /// an assembly filename.
    /// </summary>
    public interface ITestAssemblyBuilder
    {
        /// <summary>
        /// Build a suite of tests from a provided assembly
        /// </summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        ITest Build(Assembly assembly, IDictionary<string, object> options);

        /// <summary>
        /// Build a suite of tests given the filename of an assembly
        /// </summary>
        /// <param name="assemblyName">The filename of the assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        ITest Build(string assemblyName, IDictionary<string, object> options);
    }
}
