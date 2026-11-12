// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// These tests call Is.EqualTo(null) and Is.Not.EqualTo(null) on purpose: null binds to the EqualTo(string?) overload.
#pragma warning disable NUnit4002 // Use Specific constraint

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NullEquivalentTests
    {
        private static readonly Handle Valid = new(1);

        [Test]
        public void WithoutPredicate_SentinelIsNotNull()
        {
            Assert.That(Handle.Invalid, Is.Not.Null);
            Assert.That(Handle.Invalid, Is.Not.EqualTo((object?)null));
        }

        [Test]
        public void NullConstraint_UsesPredicate()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(Handle.Invalid, Is.Null);
            Assert.That(Valid, Is.Not.Null);
            Assert.That(new[] { Valid }, Is.All.Not.Null);
            Assert.That(new[] { Handle.Invalid }, Has.None.Not.Null);
        }

        [Test]
        public void EqualConstraint_TreatsNullEquivalentAsNull()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(Handle.Invalid, Is.EqualTo((object?)null));
            Assert.That(Handle.Invalid, Is.EqualTo(null)); // binds to EqualTo(string?)
            Assert.That(Valid, Is.Not.EqualTo(null));
            Assert.That(new Handle(0), Is.EqualTo(Handle.Invalid));
            Assert.That(Valid, Is.Not.EqualTo(Handle.Invalid));
        }

        [Test]
        public void CollectionConstraints_TreatNullEquivalentItemsAsNull()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);
            var items = new List<Handle> { Valid, Handle.Invalid };

            Assert.That(items, Has.Member(null));
            Assert.That(items, Contains.Item(null));
            Assert.That(items, Has.Some.Null);
            Assert.That(items, Is.EqualTo(new Handle?[] { Valid, null }));
            Assert.That(items, Is.EquivalentTo(new Handle?[] { null, Valid }));
        }

        [Test]
        public void UniqueConstraint_TreatsNullEquivalentItemsAsDuplicateNulls()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(new List<Handle?> { Handle.Invalid, new Handle(0) }, Is.Not.Unique);
            Assert.That(new List<Handle?> { Handle.Invalid, null }, Is.Not.Unique);
            Assert.That(new List<Handle?> { Valid, Handle.Invalid }, Is.Unique);
            Assert.That(new List<Handle?> { Valid, new Handle(2), Handle.Invalid }, Is.Unique);
            Assert.That(new List<Handle> { Valid, Valid }, Is.Not.Unique);
            Assert.That(new ArrayList { Handle.Invalid, new Handle(0) }, Is.Not.Unique);
            Assert.That(new List<(Handle, int)> { (Handle.Invalid, 1), (new Handle(0), 1) }, Is.Not.Unique);
            Assert.That(new List<(Handle, int)> { (Handle.Invalid, 1), (Handle.Invalid, 2) }, Is.Unique);
            Assert.That(new List<(int, string)> { (1, "a"), (1, "a") }, Is.Not.Unique);
            Assert.That(new List<(Handle, int)?> { (Handle.Invalid, 1), (new Handle(0), 1) }, Is.Not.Unique);
            Assert.That(new List<KeyValuePair<Handle, int>> { new(Handle.Invalid, 1), new(new Handle(0), 1) }, Is.Not.Unique);
            Assert.That(new List<KeyValuePair<Handle, int>> { new(Handle.Invalid, 1), new(Handle.Invalid, 2) }, Is.Unique);
            Assert.That(new List<DictionaryEntry> { new(Handle.Invalid, 1), new(new Handle(0), 1) }, Is.Not.Unique);
            Assert.That(new List<DictionaryEntry> { new(Handle.Invalid, 1), new(Handle.Invalid, 2) }, Is.Unique);
            Assert.That(new List<((Handle, int), int)> { ((Handle.Invalid, 1), 1), ((new Handle(0), 1), 1) }, Is.Not.Unique);
            Assert.That(
                new List<(int, int, int, int, int, int, int, Handle)> { (1, 2, 3, 4, 5, 6, 7, Handle.Invalid), (1, 2, 3, 4, 5, 6, 7, new Handle(0)) },
                Is.Not.Unique);
            Assert.That(new ArrayList { Valid, (Handle.Invalid, 1), (new Handle(0), 1) }, Is.Not.Unique);
            Assert.That(new ArrayList { Valid, (Handle.Invalid, 1), (Valid, 1) }, Is.Unique);

            // Tuple.Equals accepts a subclass, so its parts are compared too.
            Assert.That(new List<(Tuple<Handle>, int)> { (new Tuple<Handle>(Valid), 1), (new HandleTuple(Valid), 1) }, Is.Not.Unique);
            Assert.That(new List<(Tuple<Handle>, int)> { (new Tuple<Handle>(Handle.Invalid), 1), (new HandleTuple(new Handle(0)), 1) }, Is.Not.Unique);
            Assert.That(new List<HandleTuple> { new(Handle.Invalid), new(new Handle(0)) }, Is.Not.Unique);
            Assert.That(new List<(HandleTuple, int)> { (new(Handle.Invalid), 1), (new(new Handle(0)), 1) }, Is.Not.Unique);
        }

        [Test]
        public void UniqueConstraint_ChecksCovariantArrayItems()
        {
            TestContext.AddNullEquivalent<string[]>(l => l.Length == 0);

            // An object[] item can hold a string[], which the predicate applies to.
            Assert.That(new List<(object[]?, int)> { (Array.Empty<string>(), 1), (null, 1) }, Is.Not.Unique);
        }

        [Test]
        public void DictionaryKeys_AreNotNullEquivalent()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            // A dictionary key can't be null, so a null-equivalent key only matches the same key.
            var key = new Handle(0);
            Assert.That(new Dictionary<Handle, int> { [key] = 1 }, Is.EqualTo(new Dictionary<Handle, int> { [key] = 1 }));
            Assert.That(
                new Dictionary<Handle, int> { [new Handle(0)] = 1 },
                Is.Not.EqualTo(new Dictionary<Handle, int> { [Handle.Invalid] = 1 }));

            // Values are still null-equivalent.
            Assert.That(new Dictionary<int, Handle?> { [1] = Handle.Invalid }, Is.EqualTo(new Dictionary<int, Handle?> { [1] = null }));

            // Each dictionary's own key equality decides, also when their key comparers differ.
            Assert.That(
                new Dictionary<Handle, int> { [key] = 1 },
                Is.EqualTo(new Dictionary<Handle, int>(HandleIdComparer.Instance) { [key] = 1 }));
            TestContext.AddNullEquivalent<Handle>(h => h.Id < 0);
            var byReference = new Dictionary<Handle, int> { [new Handle(-1)] = 10, [new Handle(-1)] = 10 };
            var byId = new Dictionary<Handle, int>(HandleIdComparer.Instance) { [new Handle(-1)] = 10, [new Handle(-2)] = 20 };
            Assert.That(byReference, Is.Not.EqualTo(byId));
            Assert.That(byId, Is.Not.EqualTo(byReference));

            // A real null key in a custom dictionary isn't looked up in the other one.
            Assert.That(new NullKeyDictionary(), Is.Not.EqualTo(new Dictionary<string, int> { ["a"] = 1 }));
        }

        [Test]
        public void FailureMessage_DoesNotEnumerateNullEquivalent()
        {
            TestContext.AddNullEquivalent<DetachedRange>(r => ReferenceEquals(r, DetachedRange.Detached));

            var notNull = Assert.Throws<AssertionException>(() => Assert.That(DetachedRange.Detached, Is.Not.Null));
            Assert.That(notNull.Message, Does.Contain("Expected: not null").And.Contain("But was:  null NullEquivalentTests+DetachedRange"));

            var notEqual = Assert.Throws<AssertionException>(() => Assert.That(DetachedRange.Detached, Is.Not.EqualTo(null)));
            Assert.That(notEqual.Message, Does.Contain("Expected: not equal to null").And.Contain("But was:  null NullEquivalentTests+DetachedRange"));
        }

        [Test]
        public void FailureMessage_UsesFormattedValue()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0, "<released handle>");

            var ex = Assert.Throws<AssertionException>(() => Assert.That(Handle.Invalid, Is.Not.Null));
            Assert.That(ex.Message, Does.Contain("But was:  <released handle>"));

            ex = Assert.Throws<AssertionException>(() => Assert.That(Valid, Is.Null));
            Assert.That(ex.Message, Does.Contain("But was:  <Handle(1)>"));
        }

        [Test]
        public void UsingComparer_ReceivesOriginalValues()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(Handle.Invalid, Is.Not.EqualTo((object?)null).Using<object>((a, b) => ReferenceEquals(a, b)));
        }

        [Test]
        public void PredicatesCombine()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);
            TestContext.AddNullEquivalent<Handle>(h => h.Id < 0);

            Assert.That(Handle.Invalid, Is.Null);
            Assert.That(new Handle(-1), Is.Null);
            Assert.That(Valid, Is.Not.Null);
        }

        [Test]
        public void NotEqualToNull_FailsForNullEquivalent()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.Throws<AssertionException>(() => Assert.That(Handle.Invalid, Is.Not.EqualTo((object?)null)));
            Assert.Throws<AssertionException>(() => Assert.That(Handle.Invalid, Is.Not.EqualTo(null)));
            Assert.Throws<AssertionException>(() => Assert.That(Handle.Invalid, Is.Not.Null));
        }

        [Test]
        public void EqualConstraint_NullEquivalentAsExpectedValue()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That((Handle?)null, Is.EqualTo(Handle.Invalid));
            Assert.That(Valid, Is.Not.EqualTo(Handle.Invalid));
        }

        [Test]
        public void Assume_TreatsNullEquivalentAsNull()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            // A failed assumption makes the test Inconclusive instead of Failed, so check that none throws.
            Assert.DoesNotThrow(() => Assume.That(Handle.Invalid, Is.Null));
            Assert.DoesNotThrow(() => Assume.That(Handle.Invalid, Is.EqualTo(null)));
            Assert.DoesNotThrow(() => Assume.That((Handle?)null, Is.EqualTo(Handle.Invalid)));
        }

        [Test]
        public void EnumerableNullEquivalent_IsNotEnumeratedWhenComparedWithNull()
        {
            TestContext.AddNullEquivalent<DetachedRange>(r => ReferenceEquals(r, DetachedRange.Detached));

            Assert.That(DetachedRange.Detached, Is.Null);
            Assert.That(DetachedRange.Detached, Is.EqualTo((object?)null));
            Assert.That(new List<object?> { DetachedRange.Detached }, Is.EqualTo(new object?[] { null }));
        }

        [Test]
        public void DefaultEqualityTypes_CanNotBeRegistered()
        {
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<object>(o => o is Handle { Id: 0 }));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<ValueType>(v => v is Slot { Index: 0 }));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<Enum>(e => e.Equals(DayOfWeek.Sunday)));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<IComparable>(c => c.CompareTo(0) == 0));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<string>(s => s.Length == 0));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<int>(i => i == 0));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DayOfWeek>(d => d == DayOfWeek.Sunday));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<int?>(i => i == int.MinValue));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DayOfWeek?>(d => d == DayOfWeek.Sunday));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DateTime?>(d => d == DateTime.MinValue));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<TimeSpan>(t => t == TimeSpan.Zero));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DateTimeOffset>(d => d == DateTimeOffset.MinValue));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<ArraySegment<int>>(a => a.Array is null));
        }

        [Test]
        public void NullableTypes_CanNotBeRegistered()
        {
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<Slot?>(s => s?.Index < 0));
        }

        [Test]
        public void ValueTypes_CanBeNullEquivalent()
        {
            TestContext.AddNullEquivalent<Slot>(s => s.Index < 0);

            // The analyzer doesn't know about null-equivalent value types.
#pragma warning disable NUnit2023 // Invalid NullConstraint usage
            Assert.That(new Slot { Index = -1 }, Is.Null);
            Assert.That(new Slot { Index = 1 }, Is.Not.Null);
#pragma warning restore NUnit2023 // Invalid NullConstraint usage
            Assert.That(new Slot { Index = -1 }, Is.EqualTo((object?)null));
        }

        [Test]
        public void DefaultEqualityTypes_AreNotAffectedByPredicates()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(string.Empty, Is.Not.Null);
            Assert.That(string.Empty, Is.Not.EqualTo(null));
            Assert.That(0, Is.Not.EqualTo((object?)null));
            Assert.That(TimeSpan.Zero, Is.Not.EqualTo((object?)null));
#pragma warning disable NUnit2023 // Invalid NullConstraint usage
            Assert.That(TimeSpan.Zero, Is.Not.Null);
#pragma warning restore NUnit2023 // Invalid NullConstraint usage
            Assert.That(new List<string> { "x", string.Empty }, Is.Not.EquivalentTo(new string?[] { "x", null }));
            Assert.That(new List<object> { "x", string.Empty }, Is.Not.EquivalentTo(new object?[] { "x", null }));
        }

        [Test]
        public void PredicateIsInheritedByChildContextsAndNotLeakedToParent()
        {
            using (new TestExecutionContext.IsolatedContext())
            {
                TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

                using (new TestExecutionContext.IsolatedContext())
                {
                    Assert.That(Handle.Invalid, Is.Null, "child context");
                }

                Assert.That(Handle.Invalid, Is.Null, "context that added the predicate");
            }

            Assert.That(Handle.Invalid, Is.Not.Null, "parent context");
        }

        [Test]
        public async Task PredicateFlowsToTasks()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            await Task.Run(() => Assert.That(Handle.Invalid, Is.Null));
        }

        [Test]
        public void PredicateIsNotCalledForOtherTypesOrNull()
        {
            int calls = 0;
            TestContext.AddNullEquivalent<Handle>(h =>
            {
                calls++;
                return false;
            });

            Assert.That((object?)null, Is.Null);
            Assert.That("text", Is.Not.Null);
            Assert.That((object)0, Is.Not.Null);
            Assert.That(calls, Is.Zero);
        }

        // A type that represents "no value" with a sentinel instance, like a released handle.
        private sealed class Handle
        {
            public static readonly Handle Invalid = new(0);

            public Handle(int id) => Id = id;

            public int Id { get; }

            public override string ToString() => $"Handle({Id})";
        }

        // A dictionary with a single null key, which the BCL dictionaries don't allow.
        private sealed class NullKeyDictionary : IDictionary
        {
            public object? this[object key]
            {
                get => key is null ? 1 : null;
                set => throw new NotSupportedException();
            }

            public ICollection Keys => new object?[] { null };
            public ICollection Values => new object[] { 1 };
            public bool IsReadOnly => true;
            public bool IsFixedSize => true;
            public int Count => 1;
            public object SyncRoot => this;
            public bool IsSynchronized => false;

            public bool Contains(object key) => key is null;
            public IDictionaryEnumerator GetEnumerator() => new Hashtable().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public void Add(object key, object? value) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public void Remove(object key) => throw new NotSupportedException();
            public void CopyTo(Array array, int index) => throw new NotSupportedException();
        }

        private sealed class HandleIdComparer : IEqualityComparer<Handle>
        {
            public static readonly HandleIdComparer Instance = new();

            public bool Equals(Handle? x, Handle? y) => x?.Id == y?.Id;

            public int GetHashCode(Handle obj) => obj.Id;
        }

        // A sentinel that is also a collection and can't be enumerated, like a destroyed native object.
        private sealed class DetachedRange : IEnumerable
        {
            public static readonly DetachedRange Detached = new();

            public IEnumerator GetEnumerator() => throw new InvalidOperationException("The range is detached.");
        }

        private sealed class HandleTuple : Tuple<Handle>
        {
            public HandleTuple(Handle handle)
                : base(handle)
            {
            }
        }

        private struct Slot
        {
            public int Index;
        }
    }

    // Large collections with a predicate registered stay on the hash-based paths instead of comparing all pairs.
    [TestFixture]
    public class NullEquivalentPerformanceTests
    {
        private const int Size = 10_000;
        private const int WarnTime = 50;
        private const int FailTime = 250;

        [SetUp]
        public void AddNullEquivalent() => TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

        [Test, MaxTime(FailTime, WarningTime = WarnTime)]
        public void Unique_ItemsOfTheRegisteredType()
        {
            var items = new List<Handle>(Size);
            for (int i = 1; i <= Size; i++)
                items.Add(new Handle(i));

            Assert.That(items, Is.Unique);
        }

        [Test, MaxTime(FailTime, WarningTime = WarnTime)]
        public void Unique_TuplesWithAPartOfTheRegisteredType()
        {
            var items = new List<(Handle, int)>(Size);
            for (int i = 1; i <= Size; i++)
                items.Add((new Handle(i), i));

            Assert.That(items, Is.Unique);
        }

        [Test, MaxTime(FailTime, WarningTime = WarnTime)]
        public void EqualTo_ListsOfTheRegisteredType()
        {
            var expected = new List<Handle>(Size);
            var actual = new List<Handle>(Size);
            for (int i = 1; i <= Size; i++)
            {
                var handle = new Handle(i);
                expected.Add(handle);
                actual.Add(handle);
            }

            Assert.That(actual, Is.EqualTo(expected));
        }

        private sealed class Handle
        {
            public Handle(int id) => Id = id;

            public int Id { get; }
        }
    }

    [TestFixture]
    public class NullEquivalentOneTimeSetUpTests
    {
        [OneTimeSetUp]
        public void AddPredicate()
        {
            TestContext.AddNullEquivalent<Handle>(h => ReferenceEquals(h, Handle.Invalid));
        }

        [Test]
        public void PredicateAddedInOneTimeSetUp_AppliesToTests()
        {
            Assert.That(Handle.Invalid, Is.Null);
            Assert.That(Handle.Invalid, Is.EqualTo((object?)null));
        }

        private sealed class Handle
        {
            public static readonly Handle Invalid = new();
        }
    }
}
