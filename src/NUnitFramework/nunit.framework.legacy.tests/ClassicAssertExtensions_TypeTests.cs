// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Legacy.Tests
{
    public class ClassicAssertExtensions_TypeTests
    {
        private class Base
        {
        }

        private class Derived : Base
        {
        }

        [Test]
        public void Type_Asserts_Work()
        {
            object s = "abc";
            Assert.IsInstanceOf<string>(s);
            Assert.IsNotInstanceOf<int>(s);

            // IsAssignableFrom expects the actual type to be assignable FROM the expected type.
            // So Base is assignable from Base, but not from Derived when actual is unrelated.
            Assert.IsAssignableFrom<Base>(new Base());
            Assert.IsNotAssignableFrom<Derived>("unrelated");
        }
    }
}
