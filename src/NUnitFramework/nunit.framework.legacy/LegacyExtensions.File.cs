// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for FileAssert methods on Assert
    /// </summary>
    public static class LegacyFileAssertExtensions
    {
        extension(Assert)
        {
            #region AreEqual - Streams

            public static void AreEqual(Stream? expected, Stream? actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message ?? string.Empty, args);

            public static void AreEqual(Stream? expected, Stream? actual)
                => Legacy.FileAssert.AreEqual(expected, actual);

            #endregion

            #region AreEqual - FileInfo

            public static void AreEqual(FileInfo expected, FileInfo actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message ?? string.Empty, args);

            public static void AreEqual(FileInfo expected, FileInfo actual)
                => Legacy.FileAssert.AreEqual(expected, actual);

            #endregion

            #region AreEqual - String paths

            public static void FileAreEqual(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message ?? string.Empty, args);

            public static void FileAreEqual(string expected, string actual)
                => Legacy.FileAssert.AreEqual(expected, actual);

            #endregion

            #region AreNotEqual - Streams

            public static void AreNotEqual(Stream? expected, Stream? actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message ?? string.Empty, args);

            public static void AreNotEqual(Stream? expected, Stream? actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);

            #endregion

            #region AreNotEqual - FileInfo

            public static void AreNotEqual(FileInfo expected, FileInfo actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message ?? string.Empty, args);

            public static void AreNotEqual(FileInfo expected, FileInfo actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);

            #endregion

            #region AreNotEqual - String paths

            public static void FileAreNotEqual(string expected, string actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message ?? string.Empty, args);

            public static void FileAreNotEqual(string expected, string actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);

            #endregion

            #region Exists - FileInfo

            public static void Exists(FileInfo actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.Exists(actual, message ?? string.Empty, args);

            public static void Exists(FileInfo actual)
                => Legacy.FileAssert.Exists(actual);

            #endregion

            #region Exists - String path

            public static void FileExists(string actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.Exists(actual, message ?? string.Empty, args);

            public static void FileExists(string actual)
                => Legacy.FileAssert.Exists(actual);

            #endregion

            #region DoesNotExist - FileInfo

            public static void DoesNotExist(FileInfo actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.DoesNotExist(actual, message ?? string.Empty, args);

            public static void DoesNotExist(FileInfo actual)
                => Legacy.FileAssert.DoesNotExist(actual);

            #endregion

            #region DoesNotExist - String path

            public static void FileDoesNotExist(string actual, string? message = null, params object?[]? args)
                => Legacy.FileAssert.DoesNotExist(actual, message ?? string.Empty, args);

            public static void FileDoesNotExist(string actual)
                => Legacy.FileAssert.DoesNotExist(actual);

            #endregion
        }
    }
}
