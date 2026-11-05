// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Concurrent;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The null-equivalent predicates added to a <see cref="TestExecutionContext"/>, by the type they apply to.
    /// Instances are immutable: adding a predicate creates a new instance, so a child context can share its parent's.
    /// </summary>
    internal sealed class NullEquivalents
    {
        private readonly Registration[] _registrations;

        // Equality constraints check every compared value, so the predicates that apply to a runtime type
        // are combined once. Types that no predicate applies to map to null and cost a single lookup.
        private readonly ConcurrentDictionary<Type, Func<object, bool>?> _predicateByType = new();
        private readonly Func<Type, Func<object, bool>?> _createPredicate;

        // Items of a collection usually share their type, so remember the last lookup.
        private TypePredicate? _last;

        private NullEquivalents(Registration[] registrations)
        {
            _registrations = registrations;
            _createPredicate = CreatePredicate;
        }

        public static NullEquivalents Add(NullEquivalents? existing, Type type, Func<object, bool> isNullEquivalent)
        {
            Registration[] previous = existing?._registrations ?? Array.Empty<Registration>();
            var registrations = new Registration[previous.Length + 1];
            Array.Copy(previous, registrations, previous.Length);
            registrations[previous.Length] = new Registration(type, isNullEquivalent);
            return new NullEquivalents(registrations);
        }

        public bool IsNullEquivalent(object value)
        {
            Type type = value.GetType();
            TypePredicate? last = _last;
            Func<object, bool>? predicate;
            if (last is not null && last.Type == type)
            {
                predicate = last.Predicate;
            }
            else
            {
                if (!_predicateByType.TryGetValue(type, out predicate))
                    predicate = _predicateByType.GetOrAdd(type, _createPredicate);
                _last = new TypePredicate(type, predicate);
            }

            return predicate is not null && predicate(value);
        }

        /// <summary>
        /// Whether a value of static type <paramref name="type"/> may be null-equivalent,
        /// so callers can skip the check for collections of unrelated types.
        /// </summary>
        public bool MayApplyTo(Type type)
        {
            if (!type.CanBeNullEquivalent())
                return false;

            foreach (Registration registration in _registrations)
            {
                // The value's runtime type derives from both type and the registered type.
                // An array is sealed, but a reference-type array can hold an array of a derived type.
                if (registration.Type.IsAssignableFrom(type) || type.IsAssignableFrom(registration.Type) ||
                    (registration.Type.IsInterface && (!type.IsSealed || type.IsArray)) || (type.IsInterface && !registration.Type.IsSealed))
                {
                    return true;
                }
            }

            return false;
        }

        private Func<object, bool>? CreatePredicate(Type type)
        {
            if (!type.CanBeNullEquivalent())
                return null;

            Func<object, bool>? combined = null;
            foreach (Registration registration in _registrations)
            {
                if (!registration.Type.IsAssignableFrom(type))
                    continue;

                Func<object, bool>? previous = combined;
                Func<object, bool> current = registration.IsNullEquivalent;
                combined = previous is null ? current : value => previous(value) || current(value);
            }

            return combined;
        }

        private sealed class TypePredicate
        {
            public TypePredicate(Type type, Func<object, bool>? predicate)
            {
                Type = type;
                Predicate = predicate;
            }

            public Type Type { get; }

            public Func<object, bool>? Predicate { get; }
        }

        private readonly struct Registration
        {
            public Registration(Type type, Func<object, bool> isNullEquivalent)
            {
                Type = type;
                IsNullEquivalent = isNullEquivalent;
            }

            public Type Type { get; }

            public Func<object, bool> IsNullEquivalent { get; }
        }
    }
}
