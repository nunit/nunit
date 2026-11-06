// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Concurrent;

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

        // A method group would allocate a new delegate on every GetOrAdd call, and the overload that takes
        // an argument instead doesn't exist on .NET Framework.
        private readonly Func<Type, Func<object, bool>?> _createPredicate;

        // Items of a collection usually share their type, so remember the last lookup.
        // A class, so that type and predicate are replaced together by a single reference write.
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
            foreach (Registration registration in _registrations)
            {
                // A value of static type 'type' is of a type derived from the registered type, or the other way round.
                // An interface can also be implemented by a class that derives from the registered type.
                if (registration.Type.IsAssignableFrom(type) || type.IsAssignableFrom(registration.Type) ||
                    (type.IsInterface && !registration.Type.IsSealed))
                {
                    return true;
                }
            }

            return false;
        }

        private Func<object, bool>? CreatePredicate(Type type)
        {
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

        private sealed record class TypePredicate(Type Type, Func<object, bool>? Predicate);

        private readonly record struct Registration(Type Type, Func<object, bool> IsNullEquivalent);
    }
}
