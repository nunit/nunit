// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class DoubleValueGenerator : ValueGenerator<double>
        {
            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is double)
                {
                    step = new ComparableStep<double>((double)value, (prev, stepValue) =>
                    {
                        var next = prev + stepValue;
                        if (stepValue > 0 ? next <= prev : prev <= next)
                            throw new ArithmeticException($"Not enough precision to represent the next step; {prev:r} + {stepValue:r} = {next:r}.");
                        return next;
                    });
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
