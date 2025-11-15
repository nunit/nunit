// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for StringAssert methods on Assert
    /// </summary>
    public static partial class ClassicAssertExtensions
    {
        extension(Assert)
        {
            #region Contains (String version - different from collection Contains)

                /// <inheritdoc cref="Legacy.StringAssert.Contains(string, string)"/>
            public static void StringContains(string expected, string actual)
                => Legacy.StringAssert.Contains(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.Contains(string, string, string, object[])"/>
                public static void StringContains(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.Contains(expected, actual, message, args);

            #endregion

            #region DoesNotContain

                /// <inheritdoc cref="Legacy.StringAssert.DoesNotContain(string, string)"/>
            public static void DoesNotContain(string expected, string actual)
                => Legacy.StringAssert.DoesNotContain(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.DoesNotContain(string, string, string, object[])"/>
                public static void DoesNotContain(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.DoesNotContain(expected, actual, message, args);

            #endregion

            #region StartsWith

                /// <inheritdoc cref="Legacy.StringAssert.StartsWith(string, string)"/>
            public static void StartsWith(string expected, string actual)
                => Legacy.StringAssert.StartsWith(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.StartsWith(string, string, string, object[])"/>
                public static void StartsWith(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.StartsWith(expected, actual, message, args);

            #endregion

            #region DoesNotStartWith

                /// <inheritdoc cref="Legacy.StringAssert.DoesNotStartWith(string, string)"/>
            public static void DoesNotStartWith(string expected, string actual)
                => Legacy.StringAssert.DoesNotStartWith(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.DoesNotStartWith(string, string, string, object[])"/>
                public static void DoesNotStartWith(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.DoesNotStartWith(expected, actual, message, args);

            #endregion

            #region EndsWith

                /// <inheritdoc cref="Legacy.StringAssert.EndsWith(string, string)"/>
            public static void EndsWith(string expected, string actual)
                => Legacy.StringAssert.EndsWith(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.EndsWith(string, string, string, object[])"/>
                public static void EndsWith(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.EndsWith(expected, actual, message, args);

            #endregion

            #region DoesNotEndWith

                /// <inheritdoc cref="Legacy.StringAssert.DoesNotEndWith(string, string)"/>
            public static void DoesNotEndWith(string expected, string actual)
                => Legacy.StringAssert.DoesNotEndWith(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.DoesNotEndWith(string, string, string, object[])"/>
                public static void DoesNotEndWith(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.DoesNotEndWith(expected, actual, message, args);

            #endregion

            #region AreEqualIgnoringCase

                /// <inheritdoc cref="Legacy.StringAssert.AreEqualIgnoringCase(string, string)"/>
            public static void AreEqualIgnoringCase(string expected, string actual)
                => Legacy.StringAssert.AreEqualIgnoringCase(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.AreEqualIgnoringCase(string, string, string, object[])"/>
                public static void AreEqualIgnoringCase(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.AreEqualIgnoringCase(expected, actual, message, args);

            #endregion

            #region AreNotEqualIgnoringCase

                /// <inheritdoc cref="Legacy.StringAssert.AreNotEqualIgnoringCase(string, string)"/>
            public static void AreNotEqualIgnoringCase(string expected, string actual)
                => Legacy.StringAssert.AreNotEqualIgnoringCase(expected, actual);
                /// <inheritdoc cref="Legacy.StringAssert.AreNotEqualIgnoringCase(string, string, string, object[])"/>
                public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.AreNotEqualIgnoringCase(expected, actual, message, args);

            #endregion

            #region IsMatch

                /// <inheritdoc cref="Legacy.StringAssert.IsMatch(string, string)"/>
            public static void IsMatch(string pattern, string actual)
                => Legacy.StringAssert.IsMatch(pattern, actual);
                /// <inheritdoc cref="Legacy.StringAssert.IsMatch(string, string, string, object[])"/>
                public static void IsMatch(string pattern, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.IsMatch(pattern, actual, message, args);

            #endregion

            #region DoesNotMatch

                /// <inheritdoc cref="Legacy.StringAssert.DoesNotMatch(string, string)"/>
            public static void DoesNotMatch(string pattern, string actual)
                => Legacy.StringAssert.DoesNotMatch(pattern, actual);
                /// <inheritdoc cref="Legacy.StringAssert.DoesNotMatch(string, string, string, object[])"/>
                public static void DoesNotMatch(string pattern, string actual, string message, params object?[]? args)
                    => Legacy.StringAssert.DoesNotMatch(pattern, actual, message, args);

            #endregion
        }
    }
}
