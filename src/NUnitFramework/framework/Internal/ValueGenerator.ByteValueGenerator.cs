// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class ByteValueGenerator : ValueGenerator<byte>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is byte bValue)
                {
                    step = new ComparableStep<byte>(bValue, (prev, stepValue) => checked((byte)(prev + stepValue)));
                    return true;
                }

                // ByteValueGenerator is unusual in this regard. We allow byte parameter ranges to start high and end low,
                // and internally the step is represented as the Int32 value -1 since it can’t be represented as a Byte.
                // -1 can be converted natively to Int16, SByte and Decimal, so we can fall back on the automatic conversion for them.
                if (value is int iValue)
                {
                    step = new ComparableStep<int>(iValue, (prev, stepValue) => checked((byte)(prev + stepValue)));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
