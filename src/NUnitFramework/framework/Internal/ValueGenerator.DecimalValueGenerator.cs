// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class DecimalValueGenerator : ValueGenerator<decimal>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is decimal dValue)
                {
                    step = new ComparableStep<decimal>(dValue, (prev, stepValue) =>
                    {
                        var next = prev + stepValue;
                        if (stepValue > 0 ? next <= prev : prev <= next)
                            throw new ArithmeticException($"Not enough precision to represent the next step; {prev} + {stepValue} = {next}.");
                        return next;
                    });
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
