// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

// ReSharper disable once CheckNamespace
namespace NUnit.Framework
{
    /// <summary>
    /// C#14 static extension methods for comparison assertions (Greater, Less, etc.)
    /// </summary>
    public static class LegacyAssertComparisonExtensions
    {
        extension(Assert)
        {
            #region Greater

            public static void Greater(int arg1, int arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(int arg1, int arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            [CLSCompliant(false)]
            public static void Greater(uint arg1, uint arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void Greater(uint arg1, uint arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            public static void Greater(long arg1, long arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(long arg1, long arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            [CLSCompliant(false)]
            public static void Greater(ulong arg1, ulong arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void Greater(ulong arg1, ulong arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            public static void Greater(decimal arg1, decimal arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(decimal arg1, decimal arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            public static void Greater(double arg1, double arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(double arg1, double arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            public static void Greater(float arg1, float arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(float arg1, float arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            public static void Greater(IComparable arg1, IComparable arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message ?? string.Empty, args);
            public static void Greater(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);

            #endregion

            #region Less

            public static void Less(int arg1, int arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(int arg1, int arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            [CLSCompliant(false)]
            public static void Less(uint arg1, uint arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void Less(uint arg1, uint arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            public static void Less(long arg1, long arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(long arg1, long arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            [CLSCompliant(false)]
            public static void Less(ulong arg1, ulong arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void Less(ulong arg1, ulong arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            public static void Less(decimal arg1, decimal arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(decimal arg1, decimal arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            public static void Less(double arg1, double arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(double arg1, double arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            public static void Less(float arg1, float arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(float arg1, float arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            public static void Less(IComparable arg1, IComparable arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message ?? string.Empty, args);
            public static void Less(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.Less(arg1, arg2);

            #endregion

            #region GreaterOrEqual

            public static void GreaterOrEqual(int arg1, int arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(int arg1, int arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            [CLSCompliant(false)]
            public static void GreaterOrEqual(uint arg1, uint arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void GreaterOrEqual(uint arg1, uint arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            public static void GreaterOrEqual(long arg1, long arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(long arg1, long arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            [CLSCompliant(false)]
            public static void GreaterOrEqual(ulong arg1, ulong arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void GreaterOrEqual(ulong arg1, ulong arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            public static void GreaterOrEqual(decimal arg1, decimal arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(decimal arg1, decimal arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            public static void GreaterOrEqual(double arg1, double arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(double arg1, double arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            public static void GreaterOrEqual(float arg1, float arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(float arg1, float arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void GreaterOrEqual(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);

            #endregion

            #region LessOrEqual

            public static void LessOrEqual(int arg1, int arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(int arg1, int arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            [CLSCompliant(false)]
            public static void LessOrEqual(uint arg1, uint arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void LessOrEqual(uint arg1, uint arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            public static void LessOrEqual(long arg1, long arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(long arg1, long arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            [CLSCompliant(false)]
            public static void LessOrEqual(ulong arg1, ulong arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            [CLSCompliant(false)]
            public static void LessOrEqual(ulong arg1, ulong arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            public static void LessOrEqual(decimal arg1, decimal arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(decimal arg1, decimal arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            public static void LessOrEqual(double arg1, double arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(double arg1, double arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            public static void LessOrEqual(float arg1, float arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(float arg1, float arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            public static void LessOrEqual(IComparable arg1, IComparable arg2, string? message = null, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message ?? string.Empty, args);
            public static void LessOrEqual(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);

            #endregion
        }
    }
}
