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

            Type type = tuple.GetType();

            if (type.IsTuple())
            {
                return new ReflectionTupleAccessor<ReflectionTupleAccessor>(GetTupleAccessorFor(type), tuple);
            }
            else if (type.IsValueTuple())
            {
                return new ReflectionTupleAccessor<ReflectionValueTupleAccessor>(GetValueTupleAccessorFor(type), tuple);
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

            public ReflectionTupleAccessor(T tupleAccessor, object tuple)
            {
                _tupleAccessor = tupleAccessor;
                _tuple = tuple;
            }

            public int Length
            {
                get
                {
                    int length = _tupleAccessor.Length;
                    if (length == 8)
                    {
                        // This interface hides that the 8th element of a tuple is actually a nested tuple,
                        // We need to get the length of that nested tuple and add it to the length of this tuple.
                        object? rest = _tupleAccessor.GetValue(_tuple, 7);
                        ITuple? restTuple = For(rest);
                        if (restTuple is not null)
                        {
                            length = 7 + restTuple.Length;
                        }
                    }

                    return length;
                }
            }

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
                        value = _tupleAccessor.GetValue(_tuple, 7);
                        ITuple? rest = For(value);
                        if (rest is not null)
                        {
                            value = rest[index - 7];
                        }
                    }

                    return value;
                }
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
            return TupleAccessorsCache.GetOrAdd(tupleType, t => new ReflectionTupleAccessor(t));
        }

        private static readonly ConcurrentDictionary<Type, ReflectionValueTupleAccessor> ValueTupleAccessors = new();

        private static ReflectionValueTupleAccessor GetValueTupleAccessorFor(Type valueTupleType)
        {
            return ValueTupleAccessors.GetOrAdd(valueTupleType, t => new ReflectionValueTupleAccessor(t));
        }
#endif
    }
}
