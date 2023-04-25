// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class UInt64ValueGenerator : ValueGenerator<ulong>
        {
            public override bool TryCreateStep(object value, [NotNullWhen(true)] out ValueGenerator.Step? step)
            {
                if (value is ulong uValue)
                {
                    step = new ComparableStep<ulong>(uValue, (prev, stepValue) => checked(prev + stepValue));
                    return true;
                }

                if (value is int iValue)
                {
                    step = new ComparableStep<int>(iValue, (prev, stepValue) => checked(prev + (ulong)stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}
