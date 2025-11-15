// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for StringAssert methods on Assert
    /// </summary>
    public static class LegacyStringAssertExtensions
    {
        extension(Assert)
        {
            #region Contains (String version - different from collection Contains)

            /// <summary>
            /// Asserts that a string is found within another string.
            /// </summary>
            public static void StringContains(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.Contains(expected, actual, message ?? string.Empty, args);

            /// <summary>
            /// Asserts that a string is found within another string.
            /// </summary>
            public static void StringContains(string expected, string actual)
                => Legacy.StringAssert.Contains(expected, actual);

            #endregion

            #region DoesNotContain

            public static void DoesNotContain(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.DoesNotContain(expected, actual, message ?? string.Empty, args);

            public static void DoesNotContain(string expected, string actual)
                => Legacy.StringAssert.DoesNotContain(expected, actual);

            #endregion

            #region StartsWith

            public static void StartsWith(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.StartsWith(expected, actual, message ?? string.Empty, args);

            public static void StartsWith(string expected, string actual)
                => Legacy.StringAssert.StartsWith(expected, actual);

            #endregion

            #region DoesNotStartWith

            public static void DoesNotStartWith(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.DoesNotStartWith(expected, actual, message ?? string.Empty, args);

            public static void DoesNotStartWith(string expected, string actual)
                => Legacy.StringAssert.DoesNotStartWith(expected, actual);

            #endregion

            #region EndsWith

            public static void EndsWith(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.EndsWith(expected, actual, message ?? string.Empty, args);

            public static void EndsWith(string expected, string actual)
                => Legacy.StringAssert.EndsWith(expected, actual);

            #endregion

            #region DoesNotEndWith

            public static void DoesNotEndWith(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.DoesNotEndWith(expected, actual, message ?? string.Empty, args);

            public static void DoesNotEndWith(string expected, string actual)
                => Legacy.StringAssert.DoesNotEndWith(expected, actual);

            #endregion

            #region AreEqualIgnoringCase

            public static void AreEqualIgnoringCase(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.AreEqualIgnoringCase(expected, actual, message ?? string.Empty, args);

            public static void AreEqualIgnoringCase(string expected, string actual)
                => Legacy.StringAssert.AreEqualIgnoringCase(expected, actual);

            #endregion

            #region AreNotEqualIgnoringCase

            public static void AreNotEqualIgnoringCase(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.AreNotEqualIgnoringCase(expected, actual, message ?? string.Empty, args);

            public static void AreNotEqualIgnoringCase(string expected, string actual)
                => Legacy.StringAssert.AreNotEqualIgnoringCase(expected, actual);

            #endregion

            #region IsMatch

            public static void IsMatch(string pattern, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.IsMatch(pattern, actual, message ?? string.Empty, args);

            public static void IsMatch(string pattern, string actual)
                => Legacy.StringAssert.IsMatch(pattern, actual);

            #endregion

            #region DoesNotMatch

            public static void DoesNotMatch(string pattern, string actual, string? message = null, params object?[]? args)
                => Legacy.StringAssert.DoesNotMatch(pattern, actual, message ?? string.Empty, args);

            public static void DoesNotMatch(string pattern, string actual)
                => Legacy.StringAssert.DoesNotMatch(pattern, actual);

            #endregion
        }
    }
}
