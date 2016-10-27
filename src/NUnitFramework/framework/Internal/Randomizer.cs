// ***********************************************************************
// Copyright (c) 2013-2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Randomizer returns a set of random _values in a repeatable
    /// way, to allow re-running of tests if necessary. It extends
    /// the .NET Random class, providing random values for a much
    /// wider range of types.
    /// 
    /// The class is used internally by the framework to generate 
    /// test case data and is also exposed for use by users through 
    /// the TestContext.Random property.
    /// </summary>
    /// <remarks>
    /// For consistency with the underlying Random Type, methods 
    /// returning a single value use the prefix "Next..." Those
    /// without an argument return a non-negative value up to
    /// the full positive range of the Type. Overloads are provided
    /// for specifying a maximum or a range. Methods that return
    /// arrays or strings use the prefix "Get..." to avoid 
    /// confusion with the single-value methods.
    /// </remarks>
    public class Randomizer : Random
    {
        #region Static Members

        // Static constructor initializes values
        static Randomizer()
        {
            InitialSeed = new Random().Next();
            Randomizers = new Dictionary<MemberInfo, Randomizer>();
        }

        // Static Random instance used exclusively for the generation
        // of seed values for new Randomizers.
        private static Random _seedGenerator;

        /// <summary>
        /// Initial seed used to create randomizers for this run
        /// </summary>
        public static int InitialSeed
        {
            get { return _initialSeed; }
            set
            {
                _initialSeed = value;
                // Setting or resetting the initial seed creates seed generator
                _seedGenerator = new Random(_initialSeed);
            }
        }
        private static int _initialSeed;

        // Lookup Dictionary used to find randomizers for each member
        private static Dictionary<MemberInfo, Randomizer> Randomizers;

        /// <summary>
        /// Get a Randomizer for a particular member, returning
        /// one that has already been created if it exists.
        /// This ensures that the same _values are generated
        /// each time the tests are reloaded.
        /// </summary>
        public static Randomizer GetRandomizer(MemberInfo member)
        {
            if (Randomizers.ContainsKey(member))
                return Randomizers[member];
            else
            {
                var r = CreateRandomizer();
                Randomizers[member] = r;
                return r;
            }
        }


        /// <summary>
        /// Get a randomizer for a particular parameter, returning
        /// one that has already been created if it exists.
        /// This ensures that the same values are generated
        /// each time the tests are reloaded.
        /// </summary>
        public static Randomizer GetRandomizer(ParameterInfo parameter)
        {
            return GetRandomizer(parameter.Member);
        }

        /// <summary>
        /// Create a new Randomizer using the next seed
        /// available to ensure that each randomizer gives
        /// a unique sequence of values.
        /// </summary>
        /// <returns></returns>
        public static Randomizer CreateRandomizer()
        {
            return new Randomizer(_seedGenerator.Next());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Randomizer() { }

        /// <summary>
        /// Construct based on seed value
        /// </summary>
        /// <param name="seed"></param>
        public Randomizer(int seed) : base(seed) { }

        #endregion

        #region Ints

        // NOTE: Next(), Next(int max) and Next(int min, int max) are
        // inherited from Random.

        #endregion

        #region Unsigned Ints

        /// <summary>
        /// Returns a random unsigned int.
        /// </summary>
        [CLSCompliant(false)]
        public uint NextUInt()
        {
            return NextUInt(0u, uint.MaxValue);
        }

        /// <summary>
        /// Returns a random unsigned int less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public uint NextUInt(uint max)
        {
            return NextUInt(0u, max);
        }

        /// <summary>
        /// Returns a random unsigned int within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public uint NextUInt(uint min, uint max)
        {
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");

            if (min == max)
                return min;

            uint range = max - min;

            // Avoid introduction of modulo bias
            uint limit = uint.MaxValue - uint.MaxValue % range;
            uint raw;
            do
            {
                raw = RawUInt();
            }
            while (raw > limit);
            
            return unchecked(raw % range + min);
        }

        #endregion

        #region Shorts

        /// <summary>
        /// Returns a non-negative random short.
        /// </summary>
        public short NextShort()
        {
            return NextShort(0, short.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random short less than the specified maximum.
        /// </summary>
        public short NextShort(short max)
        {
            return NextShort((short)0, max);
        }

        /// <summary>
        /// Returns a non-negative random short within a specified range.
        /// </summary>
        public short NextShort(short min, short max)
        {
            return (short)Next(min, max);
        }

        #endregion

        #region Unsigned Shorts

        /// <summary>
        /// Returns a random unsigned short.
        /// </summary>
        [CLSCompliant(false)]
        public ushort NextUShort()
        {
            return NextUShort((ushort)0, ushort.MaxValue);
        }

        /// <summary>
        /// Returns a random unsigned short less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public ushort NextUShort(ushort max)
        {
            return NextUShort((ushort)0, max);
        }

        /// <summary>
        /// Returns a random unsigned short within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public ushort NextUShort(ushort min, ushort max)
        {
            return (ushort)Next(min, max);
        }

        #endregion

        #region Longs

        /// <summary>
        /// Returns a random long.
        /// </summary>
        public long NextLong()
        {
            return NextLong(0L, long.MaxValue);
        }

        /// <summary>
        /// Returns a random long less than the specified maximum.
        /// </summary>
        public long NextLong(long max)
        {
            return NextLong(0L, max);
        }

        /// <summary>
        /// Returns a non-negative random long within a specified range.
        /// </summary>
        public long NextLong(long min, long max)
        {
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");

            if (min == max)
                return min;

            ulong range = (ulong)(max - min);

            // Avoid introduction of modulo bias
            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong raw;
            do
            {
                raw = RawULong();
            }
            while (raw > limit);

            return (long)(raw % range + (ulong)min);
        }

        #endregion

        #region Unsigned Longs

        /// <summary>
        /// Returns a random ulong.
        /// </summary>
        [CLSCompliant(false)]
        public ulong NextULong()
        {
            return NextULong(0ul, ulong.MaxValue);
        }

        /// <summary>
        /// Returns a random ulong less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public ulong NextULong(ulong max)
        {
            return NextULong(0ul, max);
        }

        /// <summary>
        /// Returns a non-negative random long within a specified range.
        /// </summary>
        [CLSCompliant(false)]
        public ulong NextULong(ulong min, ulong max)
        {
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");

            ulong range = max - min;

            if (range == 0)
                return min;

            // Avoid introduction of modulo bias
            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong raw;
            do
            {
                raw = RawULong();
            }
            while (raw > limit);
            
            return unchecked(raw % range + min);
        }

        #endregion

        #region Bytes

        /// <summary>
        /// Returns a random Byte
        /// </summary>
        public byte NextByte()
        {
            return NextByte((byte)0, Byte.MaxValue);
        }

        /// <summary>
        /// Returns a random Byte less than the specified maximum.
        /// </summary>
        public byte NextByte(byte max)
        {
            return NextByte((byte)0, max);
        }

        /// <summary>
        /// Returns a random Byte within a specified range
        /// </summary>
        public byte NextByte(byte min, byte max)
        {
            return (byte)Next(min, max);
        }

        #endregion

        #region SBytes

        /// <summary>
        /// Returns a random SByte
        /// </summary>
        [CLSCompliant(false)]
        public sbyte NextSByte()
        {
            return NextSByte((sbyte)0, SByte.MaxValue);
        }

        /// <summary>
        /// Returns a random sbyte less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public sbyte NextSByte(sbyte max)
        {
            return NextSByte((sbyte)0, max);
        }

        /// <summary>
        /// Returns a random sbyte within a specified range
        /// </summary>
        [CLSCompliant(false)]
        public sbyte NextSByte(sbyte min, sbyte max)
        {
            return (sbyte)Next(min, max);
        }

        #endregion

        #region Bools

        /// <summary>
        /// Returns a random bool
        /// </summary>
        public bool NextBool()
        {
            return NextDouble() < 0.5;
        }

        /// <summary>
        /// Returns a random bool based on the probablility a true result
        /// </summary>
        public bool NextBool(double probability)
        {
            Guard.ArgumentInRange(probability >= 0.0 && probability <= 1.0, "Probability must be from 0.0 to 1.0", "probability");

            return NextDouble() < probability;
        }

        #endregion

        #region Doubles

        // NOTE: NextDouble() is inherited from Random.

        /// <summary>
        /// Returns a random double between 0.0 and the specified maximum.
        /// </summary>
        public double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        /// <summary>
        /// Returns a random double within a specified range.
        /// </summary>
        public double NextDouble(double min, double max)
        {
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");

            if (max == min)
                return min;

            double range = max - min;
            return NextDouble() * range + min;
        }

        #endregion

        #region Floats

        /// <summary>
        /// Returns a random float.
        /// </summary>
        public float NextFloat()
        {
            return (float)NextDouble();
        }

        /// <summary>
        /// Returns a random float between 0.0 and the specified maximum.
        /// </summary>
        public float NextFloat(float max)
        {
            return (float)NextDouble(max);
        }

        /// <summary>
        /// Returns a random float within a specified range.
        /// </summary>
        public float NextFloat(float min, float max)
        {
            return (float)NextDouble(min, max);
        }

        #endregion

        #region Enums

        /// <summary>
        /// Returns a random enum value of the specified Type as an object.
        /// </summary>
        public object NextEnum(Type type)
        {
            Array enums = TypeHelper.GetEnumValues(type);
            return enums.GetValue(Next(0, enums.Length));
        }

        /// <summary>
        /// Returns a random enum value of the specified Type.
        /// </summary>
        public T NextEnum<T>()
        {
            return (T)NextEnum(typeof(T));
        }

        #endregion
        
        #region String
        
        /// <summary>
        /// Default characters for random functions.
        /// </summary>
        /// <remarks>Default characters are the English alphabet (uppercase &amp; lowercase), arabic numerals, and underscore</remarks>
        public const string DefaultStringChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_";

        private const int DefaultStringLength = 25;
                
        /// <summary>
        /// Generate a random string based on the characters from the input string.
        /// </summary>
        /// <param name="outputLength">desired length of output string.</param>
        /// <param name="allowedChars">string representing the set of characters from which to construct the resulting string</param>
        /// <returns>A random string of arbitrary length</returns>
        public string GetString(int outputLength, string allowedChars)
        {

            var sb = new StringBuilder(outputLength);

            for (int i = 0; i < outputLength ; i++)
            {
                sb.Append(allowedChars[Next(0,allowedChars.Length)]);
            }
        
            return sb.ToString();
        }

        /// <summary>
        /// Generate a random string based on the characters from the input string.
        /// </summary>
        /// <param name="outputLength">desired length of output string.</param>
        /// <returns>A random string of arbitrary length</returns>
        /// <remarks>Uses <see cref="DefaultStringChars">DefaultStringChars</see> as the input character set </remarks>
        public string GetString(int outputLength)
        {
            return GetString(outputLength, DefaultStringChars);
        }

        /// <summary>
        /// Generate a random string based on the characters from the input string.
        /// </summary>
        /// <returns>A random string of the default length</returns>
        /// <remarks>Uses <see cref="DefaultStringChars">DefaultStringChars</see> as the input character set </remarks>
        public string GetString()
        {
            return GetString(DefaultStringLength, DefaultStringChars);
        }

        #endregion

        #region Decimal

        // We treat decimal as an integral type for now.
        // The scaling factor is always zero.

        /// <summary>
        /// Returns a random decimal.
        /// </summary>
        public decimal NextDecimal()
        {
            int low = Next(0, int.MaxValue);
            int mid = Next(0, int.MaxValue);
            int high = Next(0, int.MaxValue);
            return new Decimal(low, mid, high, false, 0);
        }

        /// <summary>
        /// Returns a random decimal between positive zero and the specified maximum.
        /// </summary>
        public decimal NextDecimal(decimal max)
        {
            return NextDecimal() % max;
        }

        /// <summary>
        /// Returns a random decimal within a specified range, which is not
        /// permitted to exceed decimal.MaxVal in the current implementation.
        /// </summary>
        /// <remarks>
        /// A limitation of this implementation is that the range from min
        /// to max must not exceed decimal.MaxVal.
        /// </remarks>
        public decimal NextDecimal(decimal min, decimal max)
        {
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");

            // Check that the range is not greater than MaxValue without 
            // first calculating it, since this would cause overflow
            Guard.ArgumentValid(max < 0M == min < 0M || min + decimal.MaxValue >= max,
                "Range too great for decimal data, use double range", "max");

            if (min == max)
                return min;

            decimal range = max - min;

            // Avoid introduction of modulo bias
            decimal limit = decimal.MaxValue - decimal.MaxValue % range;
            decimal raw;
            do
            {
                raw = NextDecimal();
            }
            while (raw > limit);

            return unchecked(raw % range + min);
        }

        #endregion

        #region Helper Methods

        private uint RawUInt()
        {
            var buffer = new byte[sizeof(uint)];
            NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private uint RawUShort()
        {
            var buffer = new byte[sizeof(uint)];
            NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private ulong RawULong()
        {
            var buffer = new byte[sizeof(ulong)];
            NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        private long RawLong()
        {
            var buffer = new byte[sizeof(long)];
            NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        private decimal RawDecimal()
        {
            int low = Next(0, int.MaxValue);
            int mid = Next(0, int.MaxValue);
            int hi = Next(0, int.MaxValue);
            bool isNegative = NextBool();
            byte scale = NextByte(29);
            return new Decimal(low, mid, hi, isNegative, scale);
        }

        #endregion
    }
}
