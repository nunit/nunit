// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    [TestFixture(typeof(List<int>))]
    [TestFixture(TypeArgs=new[] {typeof(List<object>)} )]
    [TestFixture(TypeArgs=new[] {typeof(ArrayList)} )]
    public class GenericTestFixture_IList<T> where T : IList, new()
    {
        [Test]
        public void TestCollectionCount()
        {
            IList list = new T();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            Assert.AreEqual(3, list.Count);
        }
    }

    [TestFixture(typeof(double))]
    public class GenericTestFixture_Numeric<T>
    {
        [TestCase(5)]
        [TestCase(1.23)]
        public void TestMyArgType(T x)
        {
            Assert.That(x, Is.TypeOf(typeof(T)));
        }
    }
}
