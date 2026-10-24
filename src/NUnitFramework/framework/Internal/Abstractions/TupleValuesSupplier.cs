// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET471_OR_GREATER || NETCOREAPP2_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace NUnit.Framework.Internal.Abstractions
{
    /// <summary>
    /// Class to obtain a <see cref="ITupleValuesSupplier"/> for a given tuple instance.
    /// </summary>
    internal static class TupleValuesSupplier
    {
        /// <summary>
        /// Creates an <see cref="ITupleValuesSupplier"/> for a tuple instance.
        /// </summary>
        /// <remarks>Uses an <c>ITuple</c>-based supplier when available on the target framework;
        /// otherwise uses reflection.</remarks>
        /// <param name="value">Tuple instance to read values from.</param>
        /// <returns>An <see cref="ITupleValuesSupplier"/> for <paramref name="value"/>, or <see langword="null"/> when <paramref
        /// name="value"/> is <see langword="null"/> or its type is not a supported tuple type.</returns>
        public static ITupleValuesSupplier? For(object? value)
        {
            if (value is null)
                return null;

            ITupleTypeValuesSupplier? tupleTypeValuesSupplier = TupleTypeValuesSupplier.For(value.GetType());
            if (tupleTypeValuesSupplier is null)
                return null;

#if NET471_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            if (value is ITuple tupleInterface)
                return new TupleValuesSupplierUsingITuple(tupleTypeValuesSupplier, tupleInterface);
#endif

            return new TupleValuesSupplierUsingReflection(tupleTypeValuesSupplier, value);
        }

#if NET471_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        private sealed class TupleValuesSupplierUsingITuple : ITupleValuesSupplier
        {
            private readonly ITupleTypeValuesSupplier _tupleTypeValuesSupplier;
            private readonly ITuple _tuple;

            public TupleValuesSupplierUsingITuple(ITupleTypeValuesSupplier tupleTypeValuesSupplier, ITuple tuple)
            {
                _tupleTypeValuesSupplier = tupleTypeValuesSupplier;
                _tuple = tuple;
            }

            public int Length => _tupleTypeValuesSupplier.Length;

            /// <summary>
            /// Only the first 7 elements are 'normal' fields, the 8th (Rest) could be an aggregated field.
            /// </summary>
            /// <remarks>
            /// We need to use the ITupleTypeValuesSupplier to get the value of the 8th element (Rest)
            /// because it would be flattened by the ITuple interface and we need to preserve the original structure of the tuple.
            /// </remarks>
            /// <param name="index"></param>
            /// <returns></returns>
            public object? GetValue(int index) => index < 7 ? _tuple[index] : _tupleTypeValuesSupplier.GetValue(_tuple, index);
        }
#endif

        private sealed class TupleValuesSupplierUsingReflection : ITupleValuesSupplier
        {
            private readonly ITupleTypeValuesSupplier _tupleTypeValuesSupplier;
            private readonly object _tuple;

            public TupleValuesSupplierUsingReflection(ITupleTypeValuesSupplier tupleTypeValuesSupplier, object tuple)
            {
                _tupleTypeValuesSupplier = tupleTypeValuesSupplier;
                _tuple = tuple;
            }

            public int Length => _tupleTypeValuesSupplier.Length;

            public object? GetValue(int index) => _tupleTypeValuesSupplier.GetValue(_tuple, index);
        }
    }
}
