// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;

namespace NUnit.Framework.Internal.Abstractions
{
    internal static class TupleValuesSupplier
    {
        public static ITupleValuesSupplier? For(object? tuple)
        {
            if (tuple is null)
                return null;

            ITupleTypeValuesSupplier? tupleTypeValuesSupplier = TupleTypeValuesSupplier.For(tuple.GetType());
            if (tupleTypeValuesSupplier is null)
                return null;

            if (tuple is ITuple tupleInterface)
            {
                return new TupleValuesSupplierUsingITuple(tupleTypeValuesSupplier, tupleInterface);
            }
            else
            {
                return new TupleValuesSupplierUsingReflection(tupleTypeValuesSupplier, tuple);
            }
        }

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
