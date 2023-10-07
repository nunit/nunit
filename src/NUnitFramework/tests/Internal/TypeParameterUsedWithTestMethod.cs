// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Internal
{
    [Category("Generics")]
    [TestFixture(typeof(double))]
    public class TypeParameterUsedWithTestMethod<T>
    {
        // TODO: NUnit.Analyzer doesn't handle generics.
        // It would need to have to check against the TestFixture parameter attribute.
#pragma warning disable NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        [TestCase(5)]
        [TestCase(1.23)]
#pragma warning restore NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        public void TestMyArgType(T x)
        {
            Assert.That(x, Is.TypeOf(typeof(T)));
        }
    }
}
