// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnit.TestData
{
    [ExceptionThrowingAction]
    public sealed class FailingActionAttributeOnFixtureFixture
    {
        [Test]
        public void Test1()
        {
            Assert.Pass("Test passed");
        }
    }

    public sealed class FailingActionAttributeOnTestFixture
    {
        [ExceptionThrowingAction]
        [Test]
        public void Test1()
        {
            Assert.Pass("Test passed");
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExceptionThrowingActionAttribute : TestActionAttribute
    {
        public override void AfterTest(ITest test)
        {
            throw new Exception("Intended Exception in AfterTest");
        }
    }
}
