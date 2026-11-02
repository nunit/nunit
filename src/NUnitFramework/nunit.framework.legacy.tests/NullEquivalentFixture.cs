// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class NullEquivalentFixture
    {
        private sealed class Handle
        {
            public static readonly Handle Invalid = new(0);

            public Handle(int id) => Id = id;

            public int Id { get; }
        }

        private static readonly Handle Valid = new(1);

        [SetUp]
        public void AddPredicate()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);
        }

        [Test]
        public void NullAsserts_TreatNullEquivalentAsNull()
        {
            ClassicAssert.IsNull(Handle.Invalid);
            ClassicAssert.Null(Handle.Invalid);
            Assert.Throws<AssertionException>(() => ClassicAssert.IsNotNull(Handle.Invalid));
        }

        [Test]
        public void AreEqual_TreatsNullEquivalentAsNull()
        {
            ClassicAssert.AreEqual(null, Handle.Invalid);
            ClassicAssert.AreEqual(Handle.Invalid, null);
        }

        [Test]
        public void AreNotEqual_FailsForNullEquivalent()
        {
            Assert.Throws<AssertionException>(() => ClassicAssert.AreNotEqual(null, Handle.Invalid));
            ClassicAssert.AreNotEqual(null, Valid);
        }

        [Test]
        public void CollectionAsserts_TreatNullEquivalentItemsAsNull()
        {
            var items = new List<Handle> { Valid, Handle.Invalid };

            CollectionAssert.Contains(items, null);
            CollectionAssert.AreEqual(new Handle?[] { Valid, null }, items);
            Assert.Throws<AssertionException>(() => CollectionAssert.AllItemsAreNotNull(items));
        }
    }
}
