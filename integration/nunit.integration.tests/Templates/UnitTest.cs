namespace nunit.integration.tests.Templates
{
    using System;

    using NUnit.Framework;

    /// <summary>
    /// Used as template for test methods.
    /// The name of the methods should be equal to the TestType members.
    /// </summary>
    [TestFixture]
    internal class UnitTest
    {
        [SetUp]
        public void FailedSetup()
        {
            throw new Exception("Exception during setup");
        }

        [TearDown]
        public void FailedTearDown()
        {
            throw new Exception("Exception during tear down");
        }

        [Test]
        public void Successful()
        {            
            Console.Write("output");
        }

        [Test]
        public void Failed()
        {
            Assert.Fail("Reason");
        }

        [Test]
        public void Ignored()
        {
            Assert.Ignore("Reason");
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("Reason");
        }

        [Test, Category("CatA")]
        public void SuccessfulCatA()
        {
        }        

        [Test, Parallelizable]
        public void SuccessfulParallelizable()
        {
            Console.WriteLine($"!!! ManagedThreadId = {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            System.Threading.Thread.Sleep(100);
        }

        [Test]
        public void FailedStackOverflow()
        {
            Action[] infiniteRecursion = null;
            infiniteRecursion = new Action[1] { () => { infiniteRecursion[0](); } };
            infiniteRecursion[0]();
        }

        [Test]
        public void FailedOutOfMemory()
        {
            System.Collections.Generic.List<byte[]> bytes = new System.Collections.Generic.List<byte[]>();
            while (true)
            {
                bytes.Add(new byte[0xffffff]);
            }
        }
    }
}