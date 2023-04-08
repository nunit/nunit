// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    [Category("Generics")]
    [TestFixture(typeof(double))]
    public class TypeParameterUsedWithTestMethod<T>
    {
        [TestCase(5)]
        [TestCase(1.23)]
        public void TestMyArgType(T x)
        {
            Assert.That(x, Is.TypeOf(typeof(T)));
        }
    }
}
