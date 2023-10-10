// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    public sealed class DerivedClassWithTestsInBaseClass : BaseClassWithTests
    {
        public override void VirtualTestInBaseClass()
        {
            Assert.Pass("Derived Class");
        }
    }

    public sealed class DerivedClassWithIgnoredTestsInBaseClass : BaseClassWithTests
    {
        [Ignore("Doesn't work in this derived class")]
        public override void VirtualTestInBaseClass()
        {
            Assert.Fail("Should not have been called");
        }
    }

    public class BaseClassWithTests : AbstractBaseClass
    {
        [Test]
        public void TestInBaseClass()
        {
            Assert.Pass("Base Class");
        }

        public override void VirtualTestInBaseClass()
        {
            Assert.Pass("Base Class");
        }
    }

    public abstract class AbstractBaseClass
    {
        [Test]
        public abstract void VirtualTestInBaseClass();
    }
}
