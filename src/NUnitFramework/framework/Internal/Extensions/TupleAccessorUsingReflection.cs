// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !NET471_OR_GREATER && !NETCOREAPP2_0_OR_GREATER

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TupleAccessor
    {
        public static ITuple? For(object? tuple)
        {
            if (tuple is null)
                return null;

            Type? type = tuple.GetType();

            if (type.IsValueTuple())
            {
                return new TupleAccessorUsingTupleItemSupplier<ValueTupleItemSupplierUsingReflection>(GetValueTupleAccessorFor(type), tuple);
            }

            // Allow for types derived from Tuple
            while (type.IsClass && !type.IsTuple() && type.BaseType is Type baseType)
            {
                type = baseType;
            }

            if (type.IsTuple())
            {
                return new TupleAccessorUsingTupleItemSupplier<TupleItemSupplierUsingReflection>(GetTupleAccessorFor(type), tuple);
            }
            else
            {
                return null;
            }
        }

        private sealed class TupleAccessorUsingTupleItemSupplier<T> : ITuple
            where T : ITupleItemSupplier
        {
            private readonly T _tupleItemSupplier;
            private readonly object _tuple;

            private ITuple? _rest;
            private int? _length;

            public TupleAccessorUsingTupleItemSupplier(T tupleItemSupplier, object tuple)
            {
                _tupleItemSupplier = tupleItemSupplier;
                _tuple = tuple;
            }

            public int Length => _length ??= DetermineLength();

            public object? this[int index]
            {
                get
                {
                    object? value;
                    if (index < 7)
                    {
                        value = _tupleItemSupplier.GetItem(_tuple, index);
                    }
                    else
                    {
                        // This interface hides that the 8th element of a tuple is actually a nested tuple,
                        // We need to get the value of that nested tuple at the corrected index
                        value = Rest[index - 7];
                    }

                    return value;
                }
            }

            private ITuple Rest => _rest ??= GetTupleFor8thElement();

            private ITuple GetTupleFor8thElement()
            {
                object? rest = _tupleItemSupplier.GetItem(_tuple, 7);
                ITuple? accessor = For(rest);

                if (accessor is null)
                {
                    // I much rather throw an exception here to match the ValueTuple constructor.
                    //  throw new NotSupportedException($"The 8th element of the tuple is not a valid tuple type. Type: {rest?.GetType().FullName ?? "null"}");
                    // But Copilot insisted I didn't deal with this case and native .net10 does.
                    // The only time this would be hit is if the 8th element of a tuple declaration is not a valid tuple type
                    // and the only instance where this is possible is the default instance:
                    // 'default(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)'
                    // with TRest being a non-tuple type.
                    // as the ValueTuple construcor throws an ArgumentException in this case.
                    accessor = new SingleItemTupleAccessor(rest);
                }

                return accessor;
            }

            private int DetermineLength()
            {
                int length = _tupleItemSupplier.Length;
                if (length == 8)
                {
                    // This interface hides that the 8th element of a tuple is actually a nested tuple,
                    // We need to get the length of that nested tuple and add it to the length of this tuple.
                    length = 7 + Rest.Length;
                }

                return length;
            }

            private sealed class SingleItemTupleAccessor : ITuple
            {
                private readonly object? _item;

                public SingleItemTupleAccessor(object? item) => _item = item;

                public int Length => 1;

                public object? this[int index] => _item;
            }
        }

        /// <summary>
        /// Tuple uses Properties
        /// </summary>
        private sealed class TupleItemSupplierUsingReflection : ITupleItemSupplier
        {
            private readonly PropertyInfo[] _itemProperties;

            public TupleItemSupplierUsingReflection(Type tupleType)
            {
                _itemProperties = tupleType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }

            public int Length => _itemProperties.Length;

            public object? GetItem(object tuple, int index) => _itemProperties[index].GetValue(tuple);
        }

        /// <summary>
        /// ValueTuple uses Fields
        /// </summary>
        private sealed class ValueTupleItemSupplierUsingReflection : ITupleItemSupplier
        {
            private readonly FieldInfo[] _itemFields;

            public ValueTupleItemSupplierUsingReflection(Type valueTupleType)
            {
                _itemFields = valueTupleType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            public int Length => _itemFields.Length;

            public object? GetItem(object tuple, int index) => _itemFields[index].GetValue(tuple);
        }

        private interface ITupleItemSupplier
        {
            int Length { get; }
            object? GetItem(object tuple, int index);
        }

        private static readonly ConcurrentDictionary<Type, TupleItemSupplierUsingReflection> TupleAccessorsCache = new();

        private static TupleItemSupplierUsingReflection GetTupleAccessorFor(Type tupleType)
        {
            return TupleAccessorsCache.GetOrAdd(tupleType, static t => new TupleItemSupplierUsingReflection(t));
        }

        private static readonly ConcurrentDictionary<Type, ValueTupleItemSupplierUsingReflection> ValueTupleAccessors = new();

        private static ValueTupleItemSupplierUsingReflection GetValueTupleAccessorFor(Type valueTupleType)
        {
            return ValueTupleAccessors.GetOrAdd(valueTupleType, static t => new ValueTupleItemSupplierUsingReflection(t));
        }
    }
}

#endif
