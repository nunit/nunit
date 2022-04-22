// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class UInt16ValueGenerator : ValueGenerator<ushort>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is ushort uValue)
                {
                    step = new ComparableStep<ushort>(uValue, (prev, stepValue) => checked((ushort)(prev + stepValue)));
                    return true;
                }

                if (value is int iValue)
                {
                    step = new ComparableStep<int>(iValue, (prev, stepValue) => checked((ushort)(prev + stepValue)));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
