// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class DecimalValueGenerator : ValueGenerator<decimal>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is decimal)
                {
                    step = new ComparableStep<decimal>((decimal)value, (prev, stepValue) =>
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
