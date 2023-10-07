// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Internal
{
    [Category("Generics")]
    [TestFixture(100.0, 42)]
    [TestFixture(42, 100.0)]
    public class DeduceTypeArgsFromArgs<T1, T2>
    {
        private readonly T1 _t1;
        private readonly T2 _t2;

        public DeduceTypeArgsFromArgs(T1 t1, T2 t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        [Test]
        public void TestMyArgTypes()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_t1, Is.TypeOf<T1>());
                Assert.That(_t2, Is.TypeOf<T2>());
                Assert.That((_t1 is int && _t2 is double) || (_t1 is double && _t2 is int));
            });
        }

        // TODO: NUnit.Analyzers doesn't handle generics
#pragma warning disable NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        [TestCase(5, 7)]
#pragma warning restore NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        public void TestMyArgTypes(T1 t1, T2 t2)
        {
            Assert.Multiple(() =>
            {
                Assert.That(t1, Is.TypeOf<T1>());
                Assert.That(t2, Is.TypeOf<T2>());
                Assert.That((t1 is int && t2 is double) || (t1 is double && t2 is int));
            });
        }
    }
}
