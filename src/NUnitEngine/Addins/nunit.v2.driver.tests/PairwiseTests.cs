// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using NUnit.Framework;

namespace NUnit.Engine.Drivers.Tests
{
    [TestFixture]
    public class PairwiseTest
    {
        [TestFixture]
        public class LiveTest
        {
            public Hashtable PairsTested = new Hashtable();

            [TestFixtureSetUp]
            public void TestFixtureSetUp()
            {
                PairsTested = new Hashtable();
            }

            [TestFixtureTearDown]
            public void TestFixtureTearDown()
            {
                Assert.That(PairsTested.Count, Is.EqualTo(16));
            }

            [Test, Pairwise]
            public void Test(
                [Values("a", "b", "c")] string a,
                [Values("+", "-")] string b,
                [Values("x", "y")] string c)
            {
                Console.WriteLine("Pairwise: {0} {1} {2}", a, b, c);

                PairsTested[a + b] = null;
                PairsTested[a + c] = null;
                PairsTested[b + c] = null;
            }
        }
    }
}
