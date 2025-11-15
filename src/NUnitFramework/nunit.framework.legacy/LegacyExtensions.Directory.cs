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

                /// <inheritdoc cref="Legacy.DirectoryAssert.AreEqual(DirectoryInfo, DirectoryInfo)"/>
            public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual)
                => Legacy.DirectoryAssert.AreEqual(expected, actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.AreEqual(DirectoryInfo, DirectoryInfo, string, object[])"/>
                public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.AreEqual(expected, actual, message, args);

            #endregion

            #region AreNotEqual - DirectoryInfo

                /// <inheritdoc cref="Legacy.DirectoryAssert.AreNotEqual(DirectoryInfo, DirectoryInfo)"/>
            public static void AreNotEqual(DirectoryInfo? expected, DirectoryInfo? actual)
                => Legacy.DirectoryAssert.AreNotEqual(expected, actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.AreNotEqual(DirectoryInfo, DirectoryInfo, string, object[])"/>
                public static void AreNotEqual(DirectoryInfo? expected, DirectoryInfo? actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.AreNotEqual(expected, actual, message, args);

            #endregion

            #region Exists - DirectoryInfo

                /// <inheritdoc cref="Legacy.DirectoryAssert.Exists(DirectoryInfo)"/>
            public static void Exists(DirectoryInfo actual)
                => Legacy.DirectoryAssert.Exists(actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.Exists(DirectoryInfo, string, object[])"/>
                public static void Exists(DirectoryInfo actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.Exists(actual, message, args);

            #endregion

            #region Exists - String path

                /// <inheritdoc cref="Legacy.DirectoryAssert.Exists(string)"/>
            public static void DirectoryExists(string actual)
                => Legacy.DirectoryAssert.Exists(actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.Exists(string, string, object[])"/>
                public static void DirectoryExists(string actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.Exists(actual, message, args);

            #endregion

            #region DoesNotExist - DirectoryInfo

                /// <inheritdoc cref="Legacy.DirectoryAssert.DoesNotExist(DirectoryInfo)"/>
            public static void DoesNotExist(DirectoryInfo actual)
                => Legacy.DirectoryAssert.DoesNotExist(actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.DoesNotExist(DirectoryInfo, string, object[])"/>
                public static void DoesNotExist(DirectoryInfo actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.DoesNotExist(actual, message, args);

            #endregion

            #region DoesNotExist - String path

                /// <inheritdoc cref="Legacy.DirectoryAssert.DoesNotExist(string)"/>
            public static void DirectoryDoesNotExist(string actual)
                => Legacy.DirectoryAssert.DoesNotExist(actual);
                /// <inheritdoc cref="Legacy.DirectoryAssert.DoesNotExist(string, string, object[])"/>
                public static void DirectoryDoesNotExist(string actual, string message, params object?[]? args)
                    => Legacy.DirectoryAssert.DoesNotExist(actual, message, args);

            #endregion
        }
    }
}
