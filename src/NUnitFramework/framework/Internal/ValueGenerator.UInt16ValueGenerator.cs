// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class UInt16ValueGenerator : ValueGenerator<ushort>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is ushort)
                {
                    step = new ComparableStep<ushort>((ushort)value, (prev, stepValue) => checked((ushort)(prev + stepValue)));
                    return true;
                }

                if (value is int)
                {
                    step = new ComparableStep<int>((int)value, (prev, stepValue) => checked((ushort)(prev + stepValue)));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
