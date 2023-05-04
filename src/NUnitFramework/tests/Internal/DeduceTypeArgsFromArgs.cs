// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    [Category("Generics")]
    [TestFixture(100.0, 42)]
    [TestFixture(42, 100.0)]
    public class DeduceTypeArgsFromArgs<T1, T2>
    {
        private T1 t1;
        private T2 t2;

        public DeduceTypeArgsFromArgs(T1 t1, T2 t2)
        {
            this.t1 = t1;
            this.t2 = t2;
        }

        // TODO: NUnit.Analyzers doesn't handle generics
#pragma warning disable NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        [TestCase(5, 7)]
#pragma warning restore NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
        public void TestMyArgTypes(T1 t1, T2 t2)
        {
            Assert.That(t1, Is.TypeOf<T1>());
            Assert.That(t2, Is.TypeOf<T2>());
            Assert.That(t1 is int || t2 is int);
            Assert.That(t1 is double || t2 is double);
        }
    }
}
