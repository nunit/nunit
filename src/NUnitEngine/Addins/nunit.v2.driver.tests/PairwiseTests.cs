// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using NUnit.Framework;

namespace NUnit.Core.Tests
{
    [TestFixture]
    public class PairwiseTest
    {
        [TestFixture]
        public class LiveTest
        {
            public Hashtable pairsTested = new Hashtable();

            [TestFixtureSetUp]
            public void TestFixtureSetUp()
            {
                pairsTested = new Hashtable();
            }

            [TestFixtureTearDown]
            public void TestFixtureTearDown()
            {
                Assert.That(pairsTested.Count, Is.EqualTo(16));
            }

            [Test, Pairwise]
            public void Test(
                [Values("a", "b", "c")] string a,
                [Values("+", "-")] string b,
                [Values("x", "y")] string c)
            {
                Console.WriteLine("Pairwise: {0} {1} {2}", a, b, c);

                pairsTested[a + b] = null;
                pairsTested[a + c] = null;
                pairsTested[b + c] = null;
            }
        }
    }
}
