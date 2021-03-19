// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData.ParallelExecutionData
{
    [SetUpFixture]
    public class SetUpFixture1
    {
        [OneTimeSetUpAttribute]
        public void RunOneTimeSetUp()
        {
            Thread.Sleep(100);
        }

        [OneTimeTearDown]
        public void RunOneTimeTearDown()
        {
            Thread.Sleep(100);
        }
    }

    [SetUpFixture]
    public class SetUpFixture2
    {
        [OneTimeSetUpAttribute]
        public void RunOneTimeSetUp()
        {
            Thread.Sleep(100);
        }

        [OneTimeTearDown]
        public void RunOneTimeTearDown()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture1
    {
        [Test]
        public void TestFixture1_Test()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture2
    {
        [Test]
        public void TestFixture2_Test()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixture3
    {
        [Test]
        public void TestFixture3_Test()
        {
            Thread.Sleep(100);
        }
    }

    [Apartment(ApartmentState.STA)]
    public class STAFixture
    {
        [Test]
        public void STAFixture_Test()
        {
            Thread.Sleep(100);
        }
    }

    public class TestFixtureWithParallelParameterizedTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Parallelizable(ParallelScope.Children)]
        public void ParameterizedTest(int i)
        {
            Thread.Sleep(100);
        }
    }
}
