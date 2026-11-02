// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NullEquivalentTests
    {
        // A type that represents "no value" with a sentinel instance, like a released handle.
        private sealed class Handle
        {
            public static readonly Handle Invalid = new(0);

            public Handle(int id) => Id = id;

            public int Id { get; }

            public override string ToString() => $"Handle({Id})";
        }

        private static readonly Handle Valid = new(1);

        // Resolves the whole expression: Is.Not.EqualTo(x).ApplyTo would apply only the inner EqualConstraint.
        private static bool Matches(IResolveConstraint constraint, object? actual) => constraint.Resolve().ApplyTo(actual).IsSuccess;

        // A sentinel that is also a collection and can't be enumerated, like a destroyed native object.
        private sealed class DetachedRange : IEnumerable
        {
            public static readonly DetachedRange Detached = new();

            public IEnumerator GetEnumerator() => throw new InvalidOperationException("The range is detached.");
        }

        [Test]
        public void WithoutPredicate_SentinelIsNotNull()
        {
            Assert.That(Is.Null.ApplyTo(Handle.Invalid).IsSuccess, Is.False);
            Assert.That(Is.EqualTo((object?)null).ApplyTo(Handle.Invalid).IsSuccess, Is.False);
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
            Assert.That(items, Is.EqualTo(new Handle?[] { Valid, null }));
            Assert.That(items, Is.EquivalentTo(new Handle?[] { null, Valid }));
        }

        [Test]
        public void UniqueConstraint_TreatsNullEquivalentItemsAsDuplicateNulls()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That(Matches(Is.Unique, new List<Handle?> { Handle.Invalid, new Handle(0) }), Is.False);
            Assert.That(Matches(Is.Unique, new List<Handle?> { Handle.Invalid, null }), Is.False);
            Assert.That(new List<Handle?> { Valid, Handle.Invalid }, Is.Unique);
            Assert.That(new List<Handle?> { Valid, new Handle(2), Handle.Invalid }, Is.Unique);
            Assert.That(Matches(Is.Unique, new List<Handle> { Valid, Valid }), Is.False);
            Assert.That(Matches(Is.Unique, new ArrayList { Handle.Invalid, new Handle(0) }), Is.False);
            Assert.That(Matches(Is.Unique, new List<(Handle, int)> { (Handle.Invalid, 1), (new Handle(0), 1) }), Is.False);
            Assert.That(new List<(Handle, int)> { (Handle.Invalid, 1), (Handle.Invalid, 2) }, Is.Unique);
            Assert.That(Matches(Is.Unique, new List<(int, string)> { (1, "a"), (1, "a") }), Is.False);
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

            Assert.That(Matches(Is.Not.EqualTo((object?)null), Handle.Invalid), Is.False);
            Assert.That(Matches(Is.Not.EqualTo(null), Handle.Invalid), Is.False);
            Assert.That(Matches(Is.Not.Null, Handle.Invalid), Is.False);
        }

        [Test]
        public void EqualConstraint_NullEquivalentAsExpectedValue()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            Assert.That((Handle?)null, Is.EqualTo(Handle.Invalid));
            Assert.That(Is.EqualTo(Handle.Invalid).ApplyTo(Valid).IsSuccess, Is.False);
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
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<string>(s => s.Length == 0));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<int>(i => i == 0));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DayOfWeek>(d => d == DayOfWeek.Sunday));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<int?>(i => i == int.MinValue));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DayOfWeek?>(d => d == DayOfWeek.Sunday));
            Assert.Throws<ArgumentException>(() => TestContext.AddNullEquivalent<DateTime?>(d => d == DateTime.MinValue));
        }

        private struct Slot
        {
            public int Index;
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

            // Assert.That(aStruct, Is.Null) is rejected by the NUnit analyzer, so apply the constraints directly.
            Assert.That(Matches(Is.Null, new Slot { Index = -1 }), Is.True);
            Assert.That(Matches(Is.EqualTo((object?)null), new Slot { Index = -1 }), Is.True);
            Assert.That(Matches(Is.Null, new Slot { Index = 1 }), Is.False);
        }

        [Test]
        public void DefaultEqualityTypes_AreNeverNullEquivalent()
        {
            // A predicate on object would match strings and numbers, but the collection constraints
            // compare those with EqualityComparer<T>.Default, so every constraint ignores them.
            TestContext.AddNullEquivalent<object>(v => v is string { Length: 0 } or 0);

            Assert.That(string.Empty, Is.Not.Null);
            Assert.That(string.Empty, Is.Not.EqualTo(null));
            Assert.That(0, Is.Not.EqualTo((object?)null));
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
                    Assert.That(Is.Null.ApplyTo(Handle.Invalid).IsSuccess, Is.True, "child context");
                }

                Assert.That(Is.Null.ApplyTo(Handle.Invalid).IsSuccess, Is.True, "context that added the predicate");
            }

            Assert.That(Is.Null.ApplyTo(Handle.Invalid).IsSuccess, Is.False, "parent context");
        }

        [Test]
        public async Task PredicateFlowsToTasks()
        {
            TestContext.AddNullEquivalent<Handle>(h => h.Id == 0);

            bool isNull = await Task.Run(() => Is.Null.ApplyTo(Handle.Invalid).IsSuccess);

            Assert.That(isNull, Is.True);
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
    }

    [TestFixture]
    public class NullEquivalentOneTimeSetUpTests
    {
        private sealed class Handle
        {
            public static readonly Handle Invalid = new();
        }

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
    }
}
