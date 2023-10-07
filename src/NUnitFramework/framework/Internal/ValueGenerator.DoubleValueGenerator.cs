// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal partial class ValueGenerator
    {
        private sealed class DoubleValueGenerator : ValueGenerator<double>
        {
            public override bool TryCreateStep(object value, [NotNullWhen(true)] out ValueGenerator.Step? step)
            {
                if (value is double dValue)
                {
                    step = new ComparableStep<double>(dValue, (prev, stepValue) =>
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
