// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>Helper routines for working with floating point numbers</summary>
    /// <remarks>
    ///   <para>
    ///     The floating point comparison code is based on this excellent article:
    ///     https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
    ///   </para>
    ///   <para>
    ///     "ULP" means Unit in the Last Place and in the context of this library refers to
    ///     the distance between two adjacent floating point numbers. IEEE floating point
    ///     numbers can only represent a finite subset of natural numbers, with greater
    ///     accuracy for smaller numbers and lower accuracy for very large numbers.
    ///   </para>
    ///   <para>
    ///     If a comparison is allowed "2 ulps" of deviation, that means the values are
    ///     allowed to deviate by up to 2 adjacent floating point values, which might be
    ///     as low as 0.0000001 for small numbers or as high as 10.0 for large numbers.
    ///   </para>
    /// </remarks>
    internal static class FloatingPointNumerics
    {
        #region struct FloatIntUnion

        /// <summary>Union of a floating point variable and an integer</summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            /// <summary>The union's value as a floating point variable</summary>
            [FieldOffset(0)]
            public float Float;

            /// <summary>The union's value as an integer</summary>
            [FieldOffset(0)]
            public int Int;

            /// <summary>The union's value as an unsigned integer</summary>
            [FieldOffset(0)]
            public uint UInt;
        }

        #endregion // struct FloatIntUnion

        #region struct DoubleLongUnion

        /// <summary>Union of a double precision floating point variable and a long</summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleLongUnion
        {
            /// <summary>The union's value as a double precision floating point variable</summary>
            [FieldOffset(0)]
            public double Double;

            /// <summary>The union's value as a long</summary>
            [FieldOffset(0)]
            public long Long;

            /// <summary>The union's value as an unsigned long</summary>
            [FieldOffset(0)]
            public ulong ULong;
        }

        #endregion // struct DoubleLongUnion

        /// <summary>Compares two floating point values for equality</summary>
        /// <param name="left">First floating point value to be compared</param>
        /// <param name="right">Second floating point value t be compared</param>
        /// <param name="maxUlps">
        ///   Maximum number of representable floating point values that are allowed to
        ///   be between the left and the right floating point values
        /// </param>
        /// <returns>True if both numbers are equal or close to being equal</returns>
        /// <remarks>
        ///   <para>
        ///     Floating point values can only represent a finite subset of natural numbers.
        ///     For example, the values 2.00000000 and 2.00000024 can be stored in a float,
        ///     but nothing between them.
        ///   </para>
        ///   <para>
        ///     This comparison will count how many possible floating point values are between
        ///     the left and the right number. If the number of possible values between both
        ///     numbers is less than or equal to maxUlps, then the numbers are considered as
        ///     being equal.
        ///   </para>
        ///   <para>
        ///     Implementation partially follows the code outlined here:
        ///     http://www.anttirt.net/2007/08/19/proper-floating-point-comparisons/
        ///   </para>
        /// </remarks>
        public static bool AreAlmostEqualUlps(float left, float right, int maxUlps)
        {
            FloatIntUnion leftUnion = new FloatIntUnion();
            FloatIntUnion rightUnion = new FloatIntUnion();

            leftUnion.Float = left;
            rightUnion.Float = right;

            uint leftSignMask = (leftUnion.UInt >> 31);
            uint rightSignMask = (rightUnion.UInt >> 31);

            uint leftTemp = ((0x80000000 - leftUnion.UInt) & leftSignMask);
            leftUnion.UInt = leftTemp | (leftUnion.UInt & ~leftSignMask);

            uint rightTemp = ((0x80000000 - rightUnion.UInt) & rightSignMask);
            rightUnion.UInt = rightTemp | (rightUnion.UInt & ~rightSignMask);

            if (leftSignMask != rightSignMask) // Overflow possible, check each against zero
            {
                // This check is specifically used to trap the case of 0 == -0
                // In IEEE floating point maths, -0 is converted to Float.MinValue, which cannot be used with
                // Math.Abs(...) below due to overflow issues. This should only match the 0 == -0 condition.
                if (left == right)
                {
                    return true;
                }
                if (Math.Abs(leftUnion.Int) > maxUlps || Math.Abs(rightUnion.Int) > maxUlps)
                    return false;
            }

            // Either they have the same sign or both are very close to zero
            return Math.Abs(leftUnion.Int - rightUnion.Int) <= maxUlps;
        }

        /// <summary>Compares two double precision floating point values for equality</summary>
        /// <param name="left">First double precision floating point value to be compared</param>
        /// <param name="right">Second double precision floating point value t be compared</param>
        /// <param name="maxUlps">
        ///   Maximum number of representable double precision floating point values that are
        ///   allowed to be between the left and the right double precision floating point values
        /// </param>
        /// <returns>True if both numbers are equal or close to being equal</returns>
        /// <remarks>
        ///   <para>
        ///     Double precision floating point values can only represent a limited series of
        ///     natural numbers. For example, the values 2.0000000000000000 and 2.0000000000000004
        ///     can be stored in a double, but nothing between them.
        ///   </para>
        ///   <para>
        ///     This comparison will count how many possible double precision floating point
        ///     values are between the left and the right number. If the number of possible
        ///     values between both numbers is less than or equal to maxUlps, then the numbers
        ///     are considered as being equal.
        ///   </para>
        ///   <para>
        ///     Implementation partially follows the code outlined here:
        ///     http://www.anttirt.net/2007/08/19/proper-floating-point-comparisons/
        ///   </para>
        /// </remarks>
        public static bool AreAlmostEqualUlps(double left, double right, long maxUlps)
        {
            DoubleLongUnion leftUnion = new DoubleLongUnion();
            DoubleLongUnion rightUnion = new DoubleLongUnion();

            leftUnion.Double = left;
            rightUnion.Double = right;

            ulong leftSignMask = (leftUnion.ULong >> 63);
            ulong rightSignMask = (rightUnion.ULong >> 63);

            ulong leftTemp = ((0x8000000000000000 - leftUnion.ULong) & leftSignMask);
            leftUnion.ULong = leftTemp | (leftUnion.ULong & ~leftSignMask);

            ulong rightTemp = ((0x8000000000000000 - rightUnion.ULong) & rightSignMask);
            rightUnion.ULong = rightTemp | (rightUnion.ULong & ~rightSignMask);

            if (leftSignMask != rightSignMask) // Overflow possible, check each against zero
            {
                // This check is specifically used to trap the case of 0 == -0
                // In IEEE floating point maths, -0 is converted to Double.MinValue, which cannot be used with
                // Math.Abs(...) below due to overflow issues. This should only match the 0 == -0 condition.
                if (left == right)
                {
                    return true;
                }
                if (Math.Abs(leftUnion.Long) > maxUlps || Math.Abs(rightUnion.Long) > maxUlps)
                    return false;
            }

            // Either they have the same sign or both are very close to zero
            return Math.Abs(leftUnion.Long - rightUnion.Long) <= maxUlps;
        }
    }
}
