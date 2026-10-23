// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace NUnit.Framework.Internal.Abstractions
{
    internal class TupleTypeValuesSupplier
    {
        private static readonly ConcurrentDictionary<Type, TupleTypeValuesSupplierUsingReflection> TupleAccessorsCache = new();
        private static readonly ConcurrentDictionary<Type, ValueTupleTypeValuesSupplierUsingReflection> ValueTupleAccessors = new();

        public static ITupleTypeValuesSupplier? For(Type? type)
        {
            if (type is null)
                return null;

            if (type.IsValueTuple())
            {
                return ForValueTuple(type);
            }
            else if (type.IsTuple())
            {
                return ForTuple(type);
            }
            else
            {
                return null;
            }
        }

        private static TupleTypeValuesSupplierUsingReflection ForTuple(Type tupleType)
        {
            return TupleAccessorsCache.GetOrAdd(tupleType, static t => new TupleTypeValuesSupplierUsingReflection(t));
        }

        private static ValueTupleTypeValuesSupplierUsingReflection ForValueTuple(Type valueTupleType)
        {
            return ValueTupleAccessors.GetOrAdd(valueTupleType, static t => new ValueTupleTypeValuesSupplierUsingReflection(t));
        }

        /// <summary>
        /// Tuple uses Properties
        /// </summary>
        private sealed class TupleTypeValuesSupplierUsingReflection : ITupleTypeValuesSupplier
        {
            private readonly PropertyInfo[] _itemProperties;

            public TupleTypeValuesSupplierUsingReflection(Type tupleType)
            {
                _itemProperties = tupleType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(p => p.Name) // Order by name to ensure Item1, Item2, etc. are in the correct order
                    .ToArray();
            }

            public int Length => _itemProperties.Length;

            public object? GetValue(object tuple, int index) => _itemProperties[index].GetValue(tuple);
        }

        /// <summary>
        /// ValueTuple uses Fields
        /// </summary>
        private sealed class ValueTupleTypeValuesSupplierUsingReflection : ITupleTypeValuesSupplier
        {
            private readonly FieldInfo[] _itemFields;

            public ValueTupleTypeValuesSupplierUsingReflection(Type valueTupleType)
            {
                _itemFields = valueTupleType
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(f => f.Name) // Order by name to ensure Item1, Item2, etc. are in the correct order
                    .ToArray();
            }

            public int Length => _itemFields.Length;

            public object? GetValue(object tuple, int index) => _itemFields[index].GetValue(tuple);
        }
    }
}
