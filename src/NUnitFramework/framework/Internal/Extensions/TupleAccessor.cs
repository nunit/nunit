// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;

#if !NET471_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
using System;
using System.Reflection;
using System.Collections.Concurrent;
#endif

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TupleAccessor
    {
#if NET471_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public static ITuple? For(object? tuple)
        {
            return tuple as ITuple;
        }
#else
        public static ITuple? For(object? tuple)
        {
            if (tuple is null)
                return null;

            Type? type = tuple.GetType();

            if (type.IsValueTuple())
            {
                return new ReflectionTupleAccessor<ReflectionValueTupleAccessor>(GetValueTupleAccessorFor(type), tuple);
            }

            // Allow for types derived from Tuple
            while (type.IsClass && !type.IsTuple() && type.BaseType is Type baseType)
            {
                type = baseType;
            }

            if (type.IsTuple())
            {
                return new ReflectionTupleAccessor<ReflectionTupleAccessor>(GetTupleAccessorFor(type), tuple);
            }
            else
            {
                return null;
            }
        }

        private sealed class ReflectionTupleAccessor<T> : ITuple
            where T : IReflectionTupleAccessor
        {
            private readonly T _tupleAccessor;
            private readonly object _tuple;

            private ITuple? _rest;
            private int? _length;

            public ReflectionTupleAccessor(T tupleAccessor, object tuple)
            {
                _tupleAccessor = tupleAccessor;
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
                        value = _tupleAccessor.GetValue(_tuple, index);
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
                object? rest = _tupleAccessor.GetValue(_tuple, 7);
                return For(rest) ??
                    throw new NotSupportedException($"The 8th element of the tuple is not a valid tuple type. Type: {rest?.GetType().FullName ?? "null"}");
            }

            private int DetermineLength()
            {
                int length = _tupleAccessor.Length;
                if (length == 8)
                {
                    // This interface hides that the 8th element of a tuple is actually a nested tuple,
                    // We need to get the length of that nested tuple and add it to the length of this tuple.
                    length = 7 + Rest.Length;
                }

                return length;
            }
        }

        /// <summary>
        /// Tuple uses Properties
        /// </summary>
        private sealed class ReflectionTupleAccessor : IReflectionTupleAccessor
        {
            private readonly PropertyInfo[] _itemProperties;

            public ReflectionTupleAccessor(Type tupleType)
            {
                _itemProperties = tupleType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }

            public int Length => _itemProperties.Length;

            public object? GetValue(object tuple, int index) => _itemProperties[index].GetValue(tuple);
        }

        /// <summary>
        /// ValueTuple uses Fields
        /// </summary>
        private sealed class ReflectionValueTupleAccessor : IReflectionTupleAccessor
        {
            private readonly FieldInfo[] _itemFields;

            public ReflectionValueTupleAccessor(Type valueTupleType)
            {
                _itemFields = valueTupleType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            public int Length => _itemFields.Length;

            public object? GetValue(object tuple, int index) => _itemFields[index].GetValue(tuple);
        }

        private interface IReflectionTupleAccessor
        {
            int Length { get; }
            object? GetValue(object tuple, int index);
        }

        private static readonly ConcurrentDictionary<Type, ReflectionTupleAccessor> TupleAccessorsCache = new();

        private static ReflectionTupleAccessor GetTupleAccessorFor(Type tupleType)
        {
            return TupleAccessorsCache.GetOrAdd(tupleType, static t => new ReflectionTupleAccessor(t));
        }

        private static readonly ConcurrentDictionary<Type, ReflectionValueTupleAccessor> ValueTupleAccessors = new();

        private static ReflectionValueTupleAccessor GetValueTupleAccessorFor(Type valueTupleType)
        {
            return ValueTupleAccessors.GetOrAdd(valueTupleType, static t => new ReflectionValueTupleAccessor(t));
        }
#endif
    }
}
