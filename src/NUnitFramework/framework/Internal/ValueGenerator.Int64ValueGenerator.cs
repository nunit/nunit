// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class Int64ValueGenerator : ValueGenerator<long>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is long)
                {
                    step = new ComparableStep<long>((long)value, (prev, stepValue) => checked(prev + stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
