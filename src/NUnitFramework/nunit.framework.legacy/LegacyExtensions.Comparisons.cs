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

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(int, int)"/>
            public static void Greater(int arg1, int arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(int, int, string, object[])"/>
                public static void Greater(int arg1, int arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(uint, uint)"/>
            [CLSCompliant(false)]
            public static void Greater(uint arg1, uint arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(uint, uint, string, object[])"/>
                [CLSCompliant(false)]
                public static void Greater(uint arg1, uint arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(long, long)"/>
            public static void Greater(long arg1, long arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(long, long, string, object[])"/>
                public static void Greater(long arg1, long arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(ulong, ulong)"/>
            [CLSCompliant(false)]
            public static void Greater(ulong arg1, ulong arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(ulong, ulong, string, object[])"/>
                [CLSCompliant(false)]
                public static void Greater(ulong arg1, ulong arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(decimal, decimal)"/>
            public static void Greater(decimal arg1, decimal arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(decimal, decimal, string, object[])"/>
                public static void Greater(decimal arg1, decimal arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(double, double)"/>
            public static void Greater(double arg1, double arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(double, double, string, object[])"/>
                public static void Greater(double arg1, double arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(float, float)"/>
            public static void Greater(float arg1, float arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(float, float, string, object[])"/>
                public static void Greater(float arg1, float arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(IComparable, IComparable)"/>
            public static void Greater(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.Greater(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Greater(IComparable, IComparable, string, object[])"/>
                public static void Greater(IComparable arg1, IComparable arg2, string message, params object[] args) => Legacy.ClassicAssert.Greater(arg1, arg2, message, args);

            #endregion

            #region Less

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(int, int)"/>
            public static void Less(int arg1, int arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(int, int, string, object[])"/>
                public static void Less(int arg1, int arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(uint, uint)"/>
            [CLSCompliant(false)]
            public static void Less(uint arg1, uint arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(uint, uint, string, object[])"/>
                [CLSCompliant(false)]
                public static void Less(uint arg1, uint arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(long, long)"/>
            public static void Less(long arg1, long arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(long, long, string, object[])"/>
                public static void Less(long arg1, long arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(ulong, ulong)"/>
            [CLSCompliant(false)]
            public static void Less(ulong arg1, ulong arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(ulong, ulong, string, object[])"/>
                [CLSCompliant(false)]
                public static void Less(ulong arg1, ulong arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(decimal, decimal)"/>
            public static void Less(decimal arg1, decimal arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(decimal, decimal, string, object[])"/>
                public static void Less(decimal arg1, decimal arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(double, double)"/>
            public static void Less(double arg1, double arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(double, double, string, object[])"/>
                public static void Less(double arg1, double arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(float, float)"/>
            public static void Less(float arg1, float arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(float, float, string, object[])"/>
                public static void Less(float arg1, float arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.Less(IComparable, IComparable)"/>
            public static void Less(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.Less(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.Less(IComparable, IComparable, string, object[])"/>
                public static void Less(IComparable arg1, IComparable arg2, string message, params object[] args) => Legacy.ClassicAssert.Less(arg1, arg2, message, args);

            #endregion

            #region GreaterOrEqual

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(int, int)"/>
            public static void GreaterOrEqual(int arg1, int arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(int, int, string, object[])"/>
                public static void GreaterOrEqual(int arg1, int arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(uint, uint)"/>
            [CLSCompliant(false)]
            public static void GreaterOrEqual(uint arg1, uint arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(uint, uint, string, object[])"/>
                [CLSCompliant(false)]
                public static void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(long, long)"/>
            public static void GreaterOrEqual(long arg1, long arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(long, long, string, object[])"/>
                public static void GreaterOrEqual(long arg1, long arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(ulong, ulong)"/>
            [CLSCompliant(false)]
            public static void GreaterOrEqual(ulong arg1, ulong arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(ulong, ulong, string, object[])"/>
                [CLSCompliant(false)]
                public static void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(decimal, decimal)"/>
            public static void GreaterOrEqual(decimal arg1, decimal arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(decimal, decimal, string, object[])"/>
                public static void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(double, double)"/>
            public static void GreaterOrEqual(double arg1, double arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(double, double, string, object[])"/>
                public static void GreaterOrEqual(double arg1, double arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(float, float)"/>
            public static void GreaterOrEqual(float arg1, float arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(float, float, string, object[])"/>
                public static void GreaterOrEqual(float arg1, float arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(IComparable, IComparable)"/>
            public static void GreaterOrEqual(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.GreaterOrEqual(IComparable, IComparable, string, object[])"/>
                public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args) => Legacy.ClassicAssert.GreaterOrEqual(arg1, arg2, message, args);

            #endregion

            #region LessOrEqual

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(int, int)"/>
            public static void LessOrEqual(int arg1, int arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(int, int, string, object[])"/>
                public static void LessOrEqual(int arg1, int arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(uint, uint)"/>
            [CLSCompliant(false)]
            public static void LessOrEqual(uint arg1, uint arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(uint, uint, string, object[])"/>
                [CLSCompliant(false)]
                public static void LessOrEqual(uint arg1, uint arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(long, long)"/>
            public static void LessOrEqual(long arg1, long arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(long, long, string, object[])"/>
                public static void LessOrEqual(long arg1, long arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(ulong, ulong)"/>
            [CLSCompliant(false)]
            public static void LessOrEqual(ulong arg1, ulong arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(ulong, ulong, string, object[])"/>
                [CLSCompliant(false)]
                public static void LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(decimal, decimal)"/>
            public static void LessOrEqual(decimal arg1, decimal arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(decimal, decimal, string, object[])"/>
                public static void LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(double, double)"/>
            public static void LessOrEqual(double arg1, double arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(double, double, string, object[])"/>
                public static void LessOrEqual(double arg1, double arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(float, float)"/>
            public static void LessOrEqual(float arg1, float arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(float, float, string, object[])"/>
                public static void LessOrEqual(float arg1, float arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(IComparable, IComparable)"/>
            public static void LessOrEqual(IComparable arg1, IComparable arg2) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2);
                /// <inheritdoc cref="Legacy.ClassicAssert.LessOrEqual(IComparable, IComparable, string, object[])"/>
                public static void LessOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args) => Legacy.ClassicAssert.LessOrEqual(arg1, arg2, message, args);

            #endregion
        }
    }
}
