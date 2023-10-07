// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal partial class ValueGenerator
    {
        private sealed class Int16ValueGenerator : ValueGenerator<short>
        {
            public override bool TryCreateStep(object value, [NotNullWhen(true)] out ValueGenerator.Step? step)
            {
                if (value is short sValue)
                {
                    step = new ComparableStep<short>(sValue, (prev, stepValue) => checked((short)(prev + stepValue)));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
