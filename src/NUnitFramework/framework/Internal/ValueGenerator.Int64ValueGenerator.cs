// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal partial class ValueGenerator
    {
        private sealed class Int64ValueGenerator : ValueGenerator<long>
        {
            public override bool TryCreateStep(object value, [NotNullWhen(true)] out ValueGenerator.Step? step)
            {
                if (value is long lValue)
                {
                    step = new ComparableStep<long>(lValue, (prev, stepValue) => checked(prev + stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
