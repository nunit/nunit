// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class Int32ValueGenerator : ValueGenerator<int>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is int)
                {
                    step = new ComparableStep<int>((int)value, (prev, stepValue) => checked(prev + stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
