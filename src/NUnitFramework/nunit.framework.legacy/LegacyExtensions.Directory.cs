// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for DirectoryAssert methods on Assert
    /// </summary>
    public static class LegacyDirectoryAssertExtensions
    {
        extension(Assert)
        {
            #region AreEqual - DirectoryInfo

            public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.AreEqual(expected, actual, message ?? string.Empty, args);

            public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual)
                => Legacy.DirectoryAssert.AreEqual(expected, actual);

            #endregion

            #region AreNotEqual - DirectoryInfo

            public static void AreNotEqual(DirectoryInfo? expected, DirectoryInfo? actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.AreNotEqual(expected, actual, message ?? string.Empty, args);

            public static void AreNotEqual(DirectoryInfo? expected, DirectoryInfo? actual)
                => Legacy.DirectoryAssert.AreNotEqual(expected, actual);

            #endregion

            #region Exists - DirectoryInfo

            public static void Exists(DirectoryInfo actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.Exists(actual, message ?? string.Empty, args);

            public static void Exists(DirectoryInfo actual)
                => Legacy.DirectoryAssert.Exists(actual);

            #endregion

            #region Exists - String path

            public static void DirectoryExists(string actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.Exists(actual, message ?? string.Empty, args);

            public static void DirectoryExists(string actual)
                => Legacy.DirectoryAssert.Exists(actual);

            #endregion

            #region DoesNotExist - DirectoryInfo

            public static void DoesNotExist(DirectoryInfo actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.DoesNotExist(actual, message ?? string.Empty, args);

            public static void DoesNotExist(DirectoryInfo actual)
                => Legacy.DirectoryAssert.DoesNotExist(actual);

            #endregion

            #region DoesNotExist - String path

            public static void DirectoryDoesNotExist(string actual, string? message = null, params object?[]? args)
                => Legacy.DirectoryAssert.DoesNotExist(actual, message ?? string.Empty, args);

            public static void DirectoryDoesNotExist(string actual)
                => Legacy.DirectoryAssert.DoesNotExist(actual);

            #endregion
        }
    }
}
