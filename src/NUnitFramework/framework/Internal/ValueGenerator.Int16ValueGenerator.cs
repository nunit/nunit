// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class Int16ValueGenerator : ValueGenerator<short>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is short)
                {
                    step = new ComparableStep<short>((short)value, (prev, stepValue) => checked((short)(prev + stepValue)));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
