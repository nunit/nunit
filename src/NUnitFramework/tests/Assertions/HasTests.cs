// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Summary description for HasTests.
    /// </summary>
    [TestFixture]
    public class HasTests
    {
        [Test]
        public void HasMemberAnd()
        {
            var collection1 = new [] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Has.Member(7).And.Contain(1);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void HasMemberUsing()
        {
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var collection1 = new [] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Has.Member(7).Using(myIntComparer);
            Assert.That(collection1, constraint);
        }
    }
}