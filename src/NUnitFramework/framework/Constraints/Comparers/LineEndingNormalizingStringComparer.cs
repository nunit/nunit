using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints.Comparers;

/// <summary>
/// Normalizes different line endings (\r, \n, and \r\n) to \n before comparig strings.
/// </summary>
internal sealed class LineEndingNormalizingStringComparer : IEqualityComparer<string>
{
    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs case-sensitive comparison using the rules
    /// of the current culture after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer CurrentCulture = new(StringComparison.CurrentCulture);

    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs case-insensitive comparison using the rules
    /// of the current culture after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer CurrentCultureIgnoreCase = new(StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs case-sensitive comparison using the rules
    /// of the invariant culture after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer InvariantCulture = new(StringComparison.InvariantCulture);

    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs case-insensitive comparison using the rules
    /// of the invariant culture after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer InvariantCultureIgnoreCase = new(StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs a case-sensitive ordinal comparison
    /// after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer Ordinal = new(StringComparison.Ordinal);

    /// <summary>
    /// Gets a <see cref="LineEndingNormalizingStringComparer"/> that performs a case-insensitive ordinal comparison
    /// after normalizing line endings.
    /// </summary>
    public static readonly LineEndingNormalizingStringComparer OrdinalIgnoreCase = new(StringComparison.OrdinalIgnoreCase);

    private readonly StringComparison _comparisonType;

    private LineEndingNormalizingStringComparer(StringComparison comparisonType)
    {
        _comparisonType = comparisonType;
    }

    public bool Equals(string? x, string? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;

        int xIndex = 0;
        int yIndex = 0;

        while (xIndex < x.Length && yIndex < y.Length)
        {
            // handle x first
            char xChar = x[xIndex];
            bool xIsNewLine = false;
            if (xChar == '\r')
            {
                xIsNewLine = true;
                // also handle \r\n
                if (xIndex + 1 < x.Length && x[xIndex + 1] == '\n')
                    xIndex++;
            }
            else if (xChar == '\n')
            {
                xIsNewLine = true;
            }

            // handle y next
            char yChar = y[yIndex];
            bool yIsNewLine = false;
            if (yChar == '\r')
            {
                yIsNewLine = true;
                // also handle \r\n
                if (yIndex + 1 < y.Length && y[yIndex + 1] == '\n')
                    yIndex++;
            }
            else if (yChar == '\n')
            {
                yIsNewLine = true;
            }

            if (xIsNewLine && yIsNewLine)
            {
                // Both are newlines, continue
            }
            else if (xIsNewLine || yIsNewLine)
            {
                return false; // One is newline, other is not
            }
            else
            {
                // Neither is newline, compare actual chars using the specified comparison type
                // string.Compare is efficient for comparing segments (even single chars)
                if (string.Compare(x, xIndex, y, yIndex, length: 1, _comparisonType) != 0)
                {
                    return false;
                }
            }

            // Advance indices past the processed character(s)
            xIndex++;
            yIndex++;
        }

        // Strings are equal only if both iterators reached the end of their respective strings simultaneously.
        return xIndex == x.Length && yIndex == y.Length;
    }

    public int GetHashCode(string? obj)
    {
        if (obj is null)
            return 0;

        // Calculate hash code iteratively, normalizing newlines on the fly.
        // For case-insensitivity, we need consistency with the underlying comparer.
        // Re-normalizing the string *only for hashing* might be the simplest
        // way to guarantee consistency with StringComparer.CurrentCultureIgnoreCase.GetHashCode,
        // although it introduces allocation *only* for GetHashCode.
        // An alternative is a complex iterative hash matching the specific comparer's logic,
        // but that's prone to errors and subtle mismatches.
        var normalizedForHashing = NormalizeForHashing(obj);
        var underlyingComparer = GetUnderlyingComparer(_comparisonType);
        return underlyingComparer.GetHashCode(normalizedForHashing);
    }

    private static StringComparer GetUnderlyingComparer(StringComparison comparisonType) => comparisonType switch
    {
        StringComparison.CurrentCulture => StringComparer.CurrentCulture,
        StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
        StringComparison.InvariantCulture => StringComparer.InvariantCulture,
        StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
        StringComparison.Ordinal => StringComparer.Ordinal,
        StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
        _ => throw new ArgumentOutOfRangeException(nameof(comparisonType), $"Unsupported StringComparison value: {comparisonType}"),
    };

    /// <summary>
    /// Normalizes newlines to '\n' for hashing purposes. Required for consistency
    /// with StringComparer.GetHashCode behaviors, especially culture-aware ones.
    /// </summary>
    private static string NormalizeForHashing(string input)
    {
        // Optimization: Avoid allocation if no '\r' is present, as standalone '\n' doesn't need changing for normalization *to* '\n'.
        if (input.IndexOf('\r') == -1)
        {
            return input;
        }

        StringBuilder stringBuilder = new(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '\r')
            {
                stringBuilder.Append('\n');
                // handle \r\n
                if (i + 1 < input.Length && input[i + 1] == '\n')
                {
                    i++;
                }
            }
            else
            {
                stringBuilder.Append(c); // Append \n or other characters
            }
        }
        return stringBuilder.ToString();
    }
}
