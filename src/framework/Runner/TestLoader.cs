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
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnitLite.Runner
{
    /// <summary>
    /// Static class used to load tests from an assembly
    /// </summary>
    public class TestLoader
    {
        /// <summary>
        /// Loads all the test fixtures in an assembly
        /// </summary>
        /// <param name="assembly">The assembly to be loaded</param>
        /// <returns>A Test containing all fixtures found</returns>
        public static Test Load(Assembly assembly)
        {
            TestSuite suite = new TestSuite(assembly.GetName().Name);

            foreach (Type type in assembly.GetTypes())
            {
                if (NUnitLiteTestFixtureBuilder.CanBuildFrom(type))
                    suite.Add(NUnitLiteTestFixtureBuilder.BuildFrom(type));
            }

            return suite;
        }

        /// <summary>
        /// Loads a test suite containing multiple tests from an assembly
        /// </summary>
        /// <param name="assembly">The assembly containing the tests</param>
        /// <param name="tests">String array containing the names of the test classes to load</param>
        /// <returns>A suite containing all the tests</returns>
        public static Test Load(Assembly assembly, string[] tests)
        {
            TestSuite suite = new TestSuite("Test Fixtures");
            foreach (string name in tests)
                suite.Add(TestLoader.Load(assembly, name));

            return suite;
        }

        /// <summary>
        /// Load a named fixture class from an assembly
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="className">The name of the test fixture class</param>
        /// <returns>A test representing the named fixture</returns>
        private static Test Load(Assembly assembly, string className)
        {
            Type type = assembly.GetType(className);

            if (type == null)
                throw new TestRunnerException("Unable to load class " + className);

            return NUnitLiteTestFixtureBuilder.BuildFrom(type);
        }
    }
}
