// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture(typeof(List<int>))]
    [TestFixture(TypeArgs = [typeof(List<object>)])]
    [TestFixture(TypeArgs = [typeof(ArrayList)])]
    public class GenericTestFixture_IList<T>
        where T : IList, new()
    {
        [Test]
        public void TestCollectionCount()
        {
            IList list = new T();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            Assert.That(list, Has.Count.EqualTo(3));
        }
    }

    // Deja-vu: Same test as TypeParameterUsedWithTestMethod
    [TestFixture(typeof(double))]
    public class GenericTestFixture_Numeric<T>
    {
        // TODO: NUnit.Analyzer doesn't handle generics.
        // It would need to have to check against the TestFixture parameter attribute.
        [TestCase(5)]
        [TestCase(1.23)]
        public void TestMyArgType(T x)
        {
            Assert.That(x, Is.TypeOf(typeof(T)));
        }
    }
}
