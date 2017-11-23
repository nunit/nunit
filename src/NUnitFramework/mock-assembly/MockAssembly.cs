// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Tests
{
    namespace Assemblies
    {
        /// <summary>
        /// MockAssembly is intended for those few tests that can only
        /// be made to work by loading an entire assembly. Please don't
        /// add any other entries or use it for other purposes.
        ///
        /// Most tests used as data for NUnit's own tests should be
        /// in the testdata assembly.
        /// </summary>
        public class MockAssembly
        {
            /// <summary>
            /// Constant definitions used by tests that both reference the
            /// mock-assembly and load it in order to verify counts.
            /// </summary>
            public const string FileName = "mock.nunit.assembly.exe";

            public const int Classes = 9;
            public const int NamespaceSuites = 6; // assembly, NUnit, Tests, Assemblies, Singletons, TestAssembly

            public const int Tests = MockTestFixture.Tests
                        + Singletons.OneTestCase.Tests
                        + TestAssembly.MockTestFixture.Tests
                        + IgnoredFixture.Tests
                        + ExplicitFixture.Tests
                        + BadFixture.Tests
                        + FixtureWithTestCases.Tests
                        + ParameterizedFixture.Tests
                        + GenericFixtureConstants.Tests;

            public const int Suites = MockTestFixture.Suites
                        + Singletons.OneTestCase.Suites
                        + TestAssembly.MockTestFixture.Suites
                        + IgnoredFixture.Suites
                        + ExplicitFixture.Suites
                        + BadFixture.Suites
                        + FixtureWithTestCases.Suites
                        + ParameterizedFixture.Suites
                        + GenericFixtureConstants.Suites
                        + NamespaceSuites;

            public const int TestStartedEvents = Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests;
            public const int TestFinishedEvents = Tests;
            public const int TestOutputEvents = 1;

            public const int Nodes = Tests + Suites;

            public const int ExplicitFixtures = 1;
            public const int SuitesRun = Suites - ExplicitFixtures;

            public const int Passed = MockTestFixture.Passed
                        + Singletons.OneTestCase.Tests
                        + TestAssembly.MockTestFixture.Tests
                        + FixtureWithTestCases.Tests
                        + ParameterizedFixture.Tests
                        + GenericFixtureConstants.Tests;

            public const int Skipped_Ignored = MockTestFixture.Skipped_Ignored + IgnoredFixture.Tests;
            public const int Skipped_Explicit = MockTestFixture.Skipped_Explicit + ExplicitFixture.Tests;
            public const int Skipped = Skipped_Ignored + Skipped_Explicit;

            public const int Warnings = MockTestFixture.Warnings;

            public const int Failed_Error = MockTestFixture.Failed_Error;
            public const int Failed_Other = MockTestFixture.Failed_Other;
            public const int Failed_NotRunnable = MockTestFixture.Failed_NotRunnable + BadFixture.Tests;
            public const int Failed = Failed_Error + Failed_Other + Failed_NotRunnable;

            public const int Inconclusive = MockTestFixture.Inconclusive;

#if NETCOREAPP1_1
            public static readonly Assembly ThisAssembly = typeof(MockAssembly).GetTypeInfo().Assembly;
#else
            public static readonly Assembly ThisAssembly = typeof(MockAssembly).Assembly;
#endif
            public static readonly string AssemblyPath = AssemblyHelper.GetAssemblyPath(ThisAssembly);

            public static void Main(string[] args)
            {
                new AutoRun(ThisAssembly).Execute(args);
            }
        }

        [TestFixture(Description="Fake Test Fixture")]
        [Category("FixtureCategory")]
        public class MockTestFixture
        {
            public const int Tests = 10;
            public const int Suites = 1;

            public const int Passed = 2;

            public const int Skipped_Ignored = 1;
            public const int Skipped_Explicit = 1;
            public const int Skipped = Skipped_Ignored + Skipped_Explicit;

            public const int Failed_Other = 1;
            public const int Failed_Error = 1;
            public const int Failed_NotRunnable = 2;
            public const int Failed = Failed_Error + Failed_Other + Failed_NotRunnable;

            public const int Warnings = 1;

            public const int Inconclusive = 1;

            [Test(Description="Mock Test #1")]
            [Category("MockCategory")]
            [Property("Severity", "Critical")]
            public void TestWithDescription() { }

            [Test]
            protected static void NonPublicTest() { }

            [Test]
            public void FailingTest()
            {
                Console.Error.WriteLine("Immediate Error Message");
                Assert.Fail("Intentional failure");
            }

            [Test]
            public void WarningTest()
            {
                Assert.Warn("Warning Message");
            }

            [Test, Ignore("Ignore Message")]
            public void IgnoreTest() { }

            [Test, Explicit]
            public void ExplicitTest() { }

            [Test]
            public void NotRunnableTest( int a, int b) { }

            [Test]
            public void InconclusiveTest()
            {
                Assert.Inconclusive("No valid data");
            }

            [Test]
            public void DisplayRunParameters()
            {
                foreach (string name in TestContext.Parameters.Names)
                    Console.WriteLine("Parameter {0} = {1}", name, TestContext.Parameters[name]);
            }

            [Test]
            public void TestWithException()
            {
                MethodThrowsException();
            }

            private void MethodThrowsException()
            {
                throw new Exception("Intentional Exception");
            }
        }
    }

    namespace Singletons
    {
        [TestFixture]
        public class OneTestCase
        {
            public const int Tests = 1;
            public const int Suites = 1;

            [Test]
            public virtual void TestCase()
            {}
        }
    }

    namespace TestAssembly
    {
        [TestFixture]
        public class MockTestFixture
        {
            public const int Tests = 1;
            public const int Suites = 1;

            [Test]
            public void MyTest()
            {
            }
        }
    }

    [TestFixture, Ignore("BECAUSE")]
    public class IgnoredFixture
    {
        public const int Tests = 3;
        public const int Suites = 1;

        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }

        [Test]
        public void Test3() { }
    }

    [TestFixture,Explicit]
    public class ExplicitFixture
    {
        public const int Tests = 2;
        public const int Suites = 1;
        public const int Nodes = Tests + Suites;

        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }
    }

    [TestFixture]
    public class BadFixture
    {
        public const int Tests = 1;
        public const int Suites = 1;

        public BadFixture(int val) { }

        [Test]
        public void SomeTest() { }
    }

    [TestFixture]
    public class FixtureWithTestCases
    {
        public const int Tests = 4;
        public const int Suites = 3;

        [TestCase(2, 2, ExpectedResult=4)]
        [TestCase(9, 11, ExpectedResult=20)]
        public int MethodWithParameters(int x, int y)
        {
            return x+y;
        }

        [TestCase(2, 4)]
        [TestCase(9.2, 11.7)]
        public void GenericMethod<T>(T x, T y)
        {
        }
    }

    [TestFixture(5)]
    [TestFixture(42)]
    public class ParameterizedFixture
    {
        public const int Tests = 4;
        public const int Suites = 3;

        public ParameterizedFixture(int num) { }

        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }
    }

    public class GenericFixtureConstants
    {
        public const int Tests = 4;
        public const int Suites = 3;
    }

    [TestFixture(5)]
    [TestFixture(11.5)]
    public class GenericFixture<T>
    {
        public GenericFixture(T num){ }

        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }
    }
}
