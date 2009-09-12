// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.Reflection;
using NUnit.Framework;

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
        public static ITest Load(Assembly assembly)
        {
            TestSuite suite = new TestSuite(assembly.GetName().Name);

            foreach (Type type in assembly.GetTypes())
            {
                if (TestFixtureBuilder.CanBuildFrom(type))
                    suite.AddTest(TestFixtureBuilder.BuildFrom(type));
            }

            return suite;
        }

        /// <summary>
        /// Load a named fixture class from an assembly
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="className">The name of the test fixture class</param>
        /// <returns>A test representing the named fixture</returns>
        public static ITest Load(Assembly assembly, string className)
        {
            Type type = assembly.GetType(className);
            if (type == null && className.IndexOf(',') == -1)
                type = Type.GetType(className + "," + assembly.GetName().Name);

            if (type == null)
                throw new TestRunnerException("Unable to load class " + className);

            return Load(type);
        }

        /// <summary>
        /// Loads a test suite containing multiple tests from an assembly
        /// </summary>
        /// <param name="assembly">The assembly containing the tests</param>
        /// <param name="tests">String array containing the names of the test classes to load</param>
        /// <returns>A suite containing all the tests</returns>
        public static ITest Load(Assembly assembly, string[] tests)
        {
            TestSuite suite = new TestSuite("Test Fixtures");
            foreach (string name in tests)
                suite.AddTest(TestLoader.Load(assembly, name));

            return suite;
        }

        /// <summary>
        /// Loads a type as a test using either the test suite
        /// mechansm or the fixture mechanism.
        /// </summary>
        /// <param name="type">The type to be loaded</param>
        /// <returns>A test constructed on that type</returns>
        public static ITest Load(Type type)
        {
            ITest test = TestLoader.LoadAsSuite(type);
            if (test == null)
                test = new TestSuite(type);

            return test;
        }

        /// <summary>
        /// Loads a type as a suite if possible
        /// </summary>
        /// <param name="type">The type to load</param>
        /// <returns>A test constructed from the type</returns>
        public static ITest LoadAsSuite(Type type)
        {
            PropertyInfo suiteProperty = Reflect.GetSuiteProperty(type);
            if (suiteProperty != null)
                return (ITest)suiteProperty.GetValue(null, Reflect.EmptyTypes);

            return null;
        }
    }
}
