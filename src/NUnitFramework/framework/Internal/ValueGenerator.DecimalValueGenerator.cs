// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class DecimalValueGenerator : ValueGenerator<decimal>
        {
            public override IEnumerable<decimal> GenerateRange(decimal start, decimal end, Step step)
            {
                if (start == end)
                {
                    yield return start;
                }
                else if ((start < end && !step.IsPositive) || (end < start && !step.IsNegative))
                {
                    throw new ArgumentException("Step must be in the direction of the end.");
                }
                else
                {
                    for (var current = start;;)
                    {
                        yield return current;

                        decimal next;
                        try
                        {
                            next = step.Apply(current);
                        }
                        catch (OverflowException)
                        {
                            break;
                        }

                        if (start < end)
                        {
                            if (end < next) break; // We stepped past the end of the range.
                            if (next <= current) throw new InvalidOperationException("The step must monotonically increase.");
                        }
                        else
                        {
                            if (next < end) break; // We stepped past the end of the range.
                            if (current <= next) throw new InvalidOperationException("The step must monotonically increase.");
                        }

                        current = next;
                    }
                }
            }

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
