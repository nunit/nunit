// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************
using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Tests
{
    namespace Assemblies
    {
        /// <summary>
        /// Constant definitions for the mock-assembly dll.
        /// </summary>
        public class MockAssembly
        {
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
            
            public const int Nodes = Tests + Suites;
            
            public const int ExplicitFixtures = 1;
            public const int SuitesRun = Suites - ExplicitFixtures;

            public const int Ignored = MockTestFixture.Ignored + IgnoredFixture.Tests;
            public const int Explicit = MockTestFixture.Explicit + ExplicitFixture.Tests;
            public const int NotRun = Ignored + Explicit + NotRunnable;
            public const int TestsRun = Tests - NotRun;
            public const int ResultCount = Tests - Explicit;

            public const int Errors = MockTestFixture.Errors;
            public const int Failures = MockTestFixture.Failures;
            public const int NotRunnable = MockTestFixture.NotRunnable + BadFixture.Tests;
            public const int ErrorsAndFailures = Errors + Failures + NotRunnable;
            public const int Inconclusive = MockTestFixture.Inconclusive;
            public const int Success = TestsRun - Errors - Failures - Inconclusive;

            public const int Categories = MockTestFixture.Categories;

#if !NETCF && !SILVERLIGHT
            public static readonly string AssemblyPath = AssemblyHelper.GetAssemblyPath(typeof(MockAssembly).Assembly);
#endif
        }

        [TestFixture(Description="Fake Test Fixture")]
        [Category("FixtureCategory")]
        public class MockTestFixture
        {
            public const int Tests = 11;
            public const int Suites = 1;

            public const int Ignored = 1;
            public const int Explicit = 1;

            public const int NotRun = Ignored + Explicit;
            public const int TestsRun = Tests - NotRun;
            public const int ResultCount = Tests - Explicit;

            public const int Failures = 1;
            public const int Errors = 1;
            public const int NotRunnable = 2;
            public const int ErrorsAndFailures = Errors + Failures + NotRunnable;

            public const int Inconclusive = 1;

            public const int Categories = 5;
            public const int MockCategoryTests = 2;

            [Test(Description="Mock Test #1")]
            public void MockTest1()
            {}

            [Test]
            [Category("MockCategory")]
            [Property("Severity","Critical")]
            [Description("This is a really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really, really long description")]
            public void MockTest2()
            {}

            [Test]
            [Category("MockCategory")]
            [Category("AnotherCategory")]
            public void MockTest3()
            { Assert.Pass("Succeeded!"); }

            [Test]
            protected static void MockTest5()
            {}

            [Test]
            public void FailingTest()
            {
                Assert.Fail("Intentional failure");
            }

            [Test, Property("TargetMethod", "SomeClassName"), Property("Size", 5), /*Property("TargetType", typeof( System.Threading.Thread ))*/]
            public void TestWithManyProperties()
            {}

            [Test]
            [Ignore("ignoring this test method for now")]
            [Category("Foo")]
            public void MockTest4()
            {}

            [Test, Explicit]
            [Category( "Special" )]
            public void ExplicitlyRunTest()
            {}

            [Test]
            public void NotRunnableTest( int a, int b)
            {
            }

            [Test]
            public void InconclusiveTest()
            {
                Assert.Inconclusive("No valid data");
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
