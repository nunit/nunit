// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class UInt64ValueGenerator : ValueGenerator<ulong>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is ulong)
                {
                    step = new ComparableStep<ulong>((ulong)value, (prev, stepValue) => checked(prev + stepValue));
                    return true;
                }

                if (value is int)
                {
                    step = new ComparableStep<int>((int)value, (prev, stepValue) => checked(prev + (ulong)stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
