// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using NUnit.Framework.Builders;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Utility Class used to build NUnit tests for use as test data
    /// </summary>
    public class TestBuilder
    {
        private static NUnitTestFixtureBuilder fixtureBuilder = new NUnitTestFixtureBuilder();
        private static NUnitTestCaseBuilder testBuilder = new NUnitTestCaseBuilder();

#if !NUNITLITE
        static TestBuilder()
        {
            if (!CoreExtensions.Host.Initialized)
                CoreExtensions.Host.Initialize();
        }
#endif

        public static TestSuite MakeFixture(Type type)
        {
            return (TestSuite)fixtureBuilder.BuildFrom(type);
        }

        public static TestSuite MakeFixture(object fixture)
        {
            TestSuite suite = (TestSuite)fixtureBuilder.BuildFrom(fixture.GetType());
            suite.Fixture = fixture;
            return suite;
        }

        public static TestSuite MakeParameterizedMethodSuite(Type type, string methodName)
        {
            TestSuite suite = (TestSuite)MakeTestCase(type, methodName);
            suite.Fixture = Activator.CreateInstance(type);
            return suite;
        }

        public static Test MakeTestCase(Type type, string methodName)
        {
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
                Assert.Fail("Method not found: " + methodName);
            return testBuilder.BuildFrom(method);
        }

        public static Test MakeTestCase(object fixture, string methodName)
        {
            Test test = MakeTestCase(fixture.GetType(), methodName);
            test.Fixture = fixture;
            return test;
        }

        public static TestResult RunTestFixture(Type type)
        {
            return (TestResult)MakeFixture(type).Run(TestListener.NULL, TestFilter.Empty);
        }

        public static TestResult RunTestFixture(object fixture)
        {
            return (TestResult)MakeFixture(fixture).Run(TestListener.NULL, TestFilter.Empty);
        }

        public static ITestResult RunTestCase(Type type, string methodName)
        {
            Test test = MakeTestCase(type, methodName);
            if (!IsStaticClass(type))
                test.Fixture = Activator.CreateInstance(type);
            return test.Run(TestListener.NULL, TestFilter.Empty);
        }

        private static bool IsStaticClass(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        private TestBuilder() { }
    }
}
