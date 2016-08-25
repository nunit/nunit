// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Attributes;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Api 
{
    /// <summary>
    /// Represents an <see cref="ITestAssemblyBuilder"/> which will instantiate the correct <see cref="ITestAssemblyBuilder"/> at runtime
    /// </summary>
    public sealed class RuntimeAssemblyBuilder : ITestAssemblyBuilder 
    {
        static Logger log = InternalTrace.GetLogger(typeof(RuntimeAssemblyBuilder));

        /// <summary>
        /// Build a suite of tests from a provided assembly
        /// </summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(Assembly assembly, IDictionary<string, object> options) 
        {
#if PORTABLE
            log.Debug("Loading {0}", assembly.FullName);
#else
            log.Debug("Loading {0} in AppDomain {1}", assembly.FullName, AppDomain.CurrentDomain.FriendlyName);
#endif

            return BuildUsingInnerBuilder(assembly, options);
        }

        /// <summary>
        /// Build a suite of tests given the filename of an assembly
        /// </summary>
        /// <param name="assemblyName">The filename of the assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(string assemblyName, IDictionary<string, object> options) 
        {
#if PORTABLE
            log.Debug("Loading {0}", assemblyName);
#else
            log.Debug("Loading {0} in AppDomain {1}", assemblyName, AppDomain.CurrentDomain.FriendlyName);
#endif

            ITest test = null;

            try 
            {
                var assembly = AssemblyHelper.Load(assemblyName);
                test = BuildUsingInnerBuilder(assembly, assemblyName, options);
            }
            catch (Exception ex) 
            {
                var testAssembly = new TestAssembly(assemblyName);
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);

                test = testAssembly;
            }

            return test;
        }

        private static ITest BuildUsingInnerBuilder(Assembly assembly, IDictionary<string, object> options)
        {
            ITestAssemblyBuilder testAssemblyBuilder;

            try 
            {
                testAssemblyBuilder = ConstructTestAssemblyBuilder(assembly);
            }
            catch (Exception ex) 
            {
                var testAssembly = new TestAssembly(assembly, AssemblyHelper.GetAssemblyPath(assembly));
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);

                return testAssembly;
            }

            return testAssemblyBuilder.Build(assembly, options);
        }

        private static ITest BuildUsingInnerBuilder(Assembly assembly, string assemblyName, IDictionary<string, object> options)
        {
            ITestAssemblyBuilder testAssemblyBuilder;

            try 
            {
                testAssemblyBuilder = ConstructTestAssemblyBuilder(assembly);
            }
            catch (Exception ex) 
            {
                var testAssembly = new TestAssembly(assembly, assemblyName);
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);

                return testAssembly;
            }

            return testAssemblyBuilder.Build(assemblyName, options);
        }

        private static ITestAssemblyBuilder ConstructTestAssemblyBuilder(Assembly assembly) 
        {
            log.Debug("Looking up ITestAssemblyBuilder for assembly");

#if PORTABLE
            var attributes = assembly.GetCustomAttributes<TestAssemblyBuilderAttribute>().ToArray();
#else
            var attributes = assembly.GetCustomAttributes(typeof(TestAssemblyBuilderAttribute), false /*unused*/);
#endif

            TestAssemblyBuilderAttribute testAssemblyBuilderAttribute;
            if (attributes.Length == 1 && (testAssemblyBuilderAttribute = attributes[0] as TestAssemblyBuilderAttribute) != null) 
            {
                log.Debug("Constructing ITestAssemblyBuilder {0}", testAssemblyBuilderAttribute.AssemblyBuilderType);

                return (ITestAssemblyBuilder) Reflect.Construct(testAssemblyBuilderAttribute.AssemblyBuilderType);
            }

            // Fallback to default implementation
            return new DefaultTestAssemblyBuilder();
        }
    }
}