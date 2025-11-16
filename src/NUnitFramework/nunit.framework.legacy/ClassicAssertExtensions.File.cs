// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for FileAssert methods on Assert
    /// </summary>
    public static partial class ClassicAssertExtensions
    {
        extension(Assert)
        {
            #region AreEqual - Streams

            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(Stream, Stream)"/>
            public static void AreEqual(Stream? expected, Stream? actual)
                => Legacy.FileAssert.AreEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(Stream, Stream, string, object[])"/>
            public static void AreEqual(Stream? expected, Stream? actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message, args);

            #endregion

            #region AreEqual - FileInfo

            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(FileInfo, FileInfo)"/>
            public static void AreEqual(FileInfo expected, FileInfo actual)
                => Legacy.FileAssert.AreEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(FileInfo, FileInfo, string, object[])"/>
            public static void AreEqual(FileInfo expected, FileInfo actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message, args);

            #endregion

            #region AreEqual - String paths

            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(string, string)"/>
            public static void FileAreEqual(string expected, string actual)
                => Legacy.FileAssert.AreEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreEqual(string, string, string, object[])"/>
            public static void FileAreEqual(string expected, string actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreEqual(expected, actual, message, args);

            #endregion

            #region AreNotEqual - Streams

            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(Stream, Stream)"/>
            public static void AreNotEqual(Stream? expected, Stream? actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(Stream, Stream, string, object[])"/>
            public static void AreNotEqual(Stream? expected, Stream? actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message, args);

            #endregion

            #region AreNotEqual - FileInfo

            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(FileInfo, FileInfo)"/>
            public static void AreNotEqual(FileInfo expected, FileInfo actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(FileInfo, FileInfo, string, object[])"/>
            public static void AreNotEqual(FileInfo expected, FileInfo actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message, args);

            #endregion

            #region AreNotEqual - String paths

            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(string, string)"/>
            public static void FileAreNotEqual(string expected, string actual)
                => Legacy.FileAssert.AreNotEqual(expected, actual);
            /// <inheritdoc cref="Legacy.FileAssert.AreNotEqual(string, string, string, object[])"/>
            public static void FileAreNotEqual(string expected, string actual, string message, params object?[]? args)
                => Legacy.FileAssert.AreNotEqual(expected, actual, message, args);

            #endregion

            #region Exists - FileInfo

            /// <inheritdoc cref="Legacy.FileAssert.Exists(FileInfo)"/>
            public static void Exists(FileInfo actual)
                => Legacy.FileAssert.Exists(actual);
            /// <inheritdoc cref="Legacy.FileAssert.Exists(FileInfo, string, object[])"/>
            public static void Exists(FileInfo actual, string message, params object?[]? args)
                => Legacy.FileAssert.Exists(actual, message, args);

            #endregion

            #region Exists - String path

            /// <inheritdoc cref="Legacy.FileAssert.Exists(string)"/>
            public static void FileExists(string actual)
                => Legacy.FileAssert.Exists(actual);
            /// <inheritdoc cref="Legacy.FileAssert.Exists(string, string, object[])"/>
            public static void FileExists(string actual, string message, params object?[]? args)
                => Legacy.FileAssert.Exists(actual, message, args);

            #endregion

            #region DoesNotExist - FileInfo

            /// <inheritdoc cref="Legacy.FileAssert.DoesNotExist(FileInfo)"/>
            public static void DoesNotExist(FileInfo actual)
                => Legacy.FileAssert.DoesNotExist(actual);
            /// <inheritdoc cref="Legacy.FileAssert.DoesNotExist(FileInfo, string, object[])"/>
            public static void DoesNotExist(FileInfo actual, string message, params object?[]? args)
                => Legacy.FileAssert.DoesNotExist(actual, message, args);

            #endregion

            #region DoesNotExist - String path

            /// <inheritdoc cref="Legacy.FileAssert.DoesNotExist(string)"/>
            public static void FileDoesNotExist(string actual)
                => Legacy.FileAssert.DoesNotExist(actual);
            /// <inheritdoc cref="Legacy.FileAssert.DoesNotExist(string, string, object[])"/>
            public static void FileDoesNotExist(string actual, string message, params object?[]? args)
                => Legacy.FileAssert.DoesNotExist(actual, message, args);

            #endregion
        }
    }
}
