#if NET6_0_OR_GREATER
// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    /// <summary>
    /// Custom InterpolatedStringHandler which allows us to
    /// delay formatting error messages only if needed, aka the test fails.
    /// </summary>
    /// <remarks>
    /// The only way we can do this is using an `out bool isEnabled` parameter.
    /// This implies that we need to evaluate the Assertion in the constructor.
    /// https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/improved-interpolated-strings.md
    /// </remarks>
    /// <remarks>
    /// This code will only be called by compiler generated code.
    /// </remarks>
    [InterpolatedStringHandler]
    public ref struct AssertingInterpolatedStringHandler<TActual>
    {
        // Do not make this readonly as then the struct cannot be updated.
#pragma warning disable IDE0044 // Add readonly modifier
        private DefaultInterpolatedStringHandler _builder;
#pragma warning restore IDE0044 // Add readonly modifier

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public AssertingInterpolatedStringHandler(int literalLength, int formattedCount, TActual actual, out bool isEnabled)
            : this(literalLength, formattedCount, null, actual, Is.True, out isEnabled)
        {
        }

        public AssertingInterpolatedStringHandler(int literalLength, int formattedCount, Func<TActual> actual, out bool isEnabled)
            : this(literalLength, formattedCount, null, actual.Invoke(), Is.True, out isEnabled)
        {
        }

        public AssertingInterpolatedStringHandler(int literalLength, int formattedCount, TActual? actual, IResolveConstraint expression, out bool isEnabled)
            : this(literalLength, formattedCount, null, actual, expression, out isEnabled)
        {
        }

        public AssertingInterpolatedStringHandler(int literalLength, int formattedCount, Func<TActual> actual, IResolveConstraint expression, out bool isEnabled)
            : this(literalLength, formattedCount, null, actual.Invoke(), expression, out isEnabled)
        {
        }

        public AssertingInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider? provider, TActual? actual, IResolveConstraint expression, out bool isEnabled)
        {
            var constraint = expression.Resolve();

            Result = constraint.ApplyTo(actual);
            isEnabled = !Result.IsSuccess;
            if (isEnabled)
                _builder = new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider);
        }

        public ConstraintResult Result { get; }

        public void AppendFormatted(ReadOnlySpan<char> value)
            => _builder.AppendFormatted(value);

        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
            => _builder.AppendFormatted(value, alignment, format);

        public void AppendFormatted<T>(T value)
            => _builder.AppendFormatted(value);

        public void AppendFormatted<T>(T value, string? format)
            => _builder.AppendFormatted(value, format);

        public void AppendFormatted<T>(T value, int alignment)
            => _builder.AppendFormatted(value, alignment);

        public void AppendFormatted<T>(T value, int alignment, string? format)
            => _builder.AppendFormatted(value, alignment, format);

        public void AppendFormatted(object? value, int alignment = 0, string? format = null)
            => _builder.AppendFormatted(value, alignment, format);

        public void AppendFormatted(string? value)
            => _builder.AppendFormatted(value);

        public void AppendFormatted(string? value, int alignment = 0, string? format = null)
            => _builder.AppendFormatted(value, alignment, format);

        public void AppendLiteral(string value)
            => _builder.AppendLiteral(value);

        public override string ToString()
            => _builder.ToStringAndClear();

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
#endif
