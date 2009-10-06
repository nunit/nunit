// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if CLR_2_0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Core.Tests
{
    [TestFixture(typeof(List<int>))]
    [TestFixture(typeof(ArrayList))]
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
#endif