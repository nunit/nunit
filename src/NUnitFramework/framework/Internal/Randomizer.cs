// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Randomizer returns a set of random values in a repeatable
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
        private static Random _seedGenerator = null!; // Initialized by the InitialSeed setter called by the static constructor.

        /// <summary>
        /// Initial seed used to create randomizers for this run
        /// </summary>
        public static int InitialSeed
        {
            get => _initialSeed;
            set
            {
                _initialSeed = value;
                // Setting or resetting the initial seed creates seed generator
                _seedGenerator = new Random(_initialSeed);
            }
        }
        private static int _initialSeed;

        // Lookup Dictionary used to find randomizers for each member
        private static readonly Dictionary<MemberInfo, Randomizer> Randomizers;

        /// <summary>
        /// Get a Randomizer for a particular member, returning
        /// one that has already been created if it exists.
        /// This ensures that the same values are generated
        /// each time the tests are reloaded.
        /// </summary>
        public static Randomizer GetRandomizer(MemberInfo member)
        {
            if (Randomizers.TryGetValue(member, out var randomizer))
            {
                return randomizer;
            }
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
        public Randomizer()
        {
        }

        /// <summary>
        /// Construct based on seed value
        /// </summary>
        /// <param name="seed"></param>
        public Randomizer(int seed) : base(seed)
        {
        }

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
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", nameof(max));

            if (min == max)
                return min;

            uint range = max - min;

            // Avoid introduction of modulo bias
            uint limit = uint.MaxValue - uint.MaxValue % range;
            uint raw;
            do
            {
                raw = RawUInt32();
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
            return NextUShort(0, ushort.MaxValue);
        }

        /// <summary>
        /// Returns a random unsigned short less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public ushort NextUShort(ushort max)
        {
            return NextUShort(0, max);
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
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", nameof(max));

            if (min == max)
                return min;

            ulong range = (ulong)(max - min);

            // Avoid introduction of modulo bias
            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong raw;
            do
            {
                raw = RawUInt64();
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
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", nameof(max));

            ulong range = max - min;

            if (range == 0)
                return min;

            // Avoid introduction of modulo bias
            ulong limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong raw;
            do
            {
                raw = RawUInt64();
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
            return NextByte(0, Byte.MaxValue);
        }

        /// <summary>
        /// Returns a random Byte less than the specified maximum.
        /// </summary>
        public byte NextByte(byte max)
        {
            return NextByte(0, max);
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
            return NextSByte(0, SByte.MaxValue);
        }

        /// <summary>
        /// Returns a random sbyte less than the specified maximum.
        /// </summary>
        [CLSCompliant(false)]
        public sbyte NextSByte(sbyte max)
        {
            return NextSByte(0, max);
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
        /// Returns a random bool based on the probability a true result
        /// </summary>
        public bool NextBool(double probability)
        {
            Guard.ArgumentInRange(probability >= 0.0 && probability <= 1.0, "Probability must be from 0.0 to 1.0", nameof(probability));

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
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", nameof(max));

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
            Array enums = Enum.GetValues(type);
            return enums.GetValue(Next(0, enums.Length))!;
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
        /// <remarks>Default characters are the English alphabet (uppercase &amp; lowercase), Arabic numerals, and underscore</remarks>
        public const string DefaultStringChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_";

        private const int DefaultStringLength = 25;
        private const int MaxStackAllocSize = 256;

        /// <summary>
        /// Generate a random string based on the characters from the input string.
        /// </summary>
        /// <param name="outputLength">desired length of output string.</param>
        /// <param name="allowedChars">string representing the set of characters from which to construct the resulting string</param>
        /// <returns>A random string of arbitrary length</returns>
        public string GetString(int outputLength, string allowedChars)
        {
            if (outputLength < 0)
                throw new ArgumentOutOfRangeException(nameof(outputLength));
#if NET6_0_OR_GREATER
            return string.Create(outputLength, allowedChars, FillSpan);

            void FillSpan(Span<char> data, string allowedChars)
            {
                for (int i = 0; i < data.Length; i++)
                    data[i] = allowedChars[Next(0, allowedChars.Length)];
            }
#else
            var data = new char[outputLength];

            for (int i = 0; i < data.Length; i++)
                data[i] = allowedChars[Next(0, allowedChars.Length)];

            return new string(data);
#endif
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
            return new decimal(RawInt32(), RawInt32(), RawInt32(), false, 0);
        }

        /// <summary>
        /// Returns a random decimal between positive zero and the specified maximum.
        /// </summary>
        public decimal NextDecimal(decimal max)
        {
            if (max <= 1)
            {
                Guard.ArgumentInRange(max > 0, "Maximum must be greater than zero.", nameof(max));
                return 0;
            }

            unchecked
            {
                // Flooring sets the exponent (scale) to zero. This is necessary even if the number is already an
                // integer so that the low, mid and high parts of the mantissa get shifted by the appropriate power of
                // ten until they only contain the integral part of the number.
                // May as well set it back to the parameter so that the `result < max` lower down is doing less work.
                max = decimal.Floor(max);

                var parts = DecimalParts.FromValue(max);

                // If scale is not zero, that means that low, mid, and high are shifted to make room for digits from the
                // fractional part of the number. We relied on Decimal.Floor to prevent this.
                Guard.OperationValid(parts.Scale == 0, "Decimal.Floor returned a value whose scale was not 0.");

                while (true)
                {
                    // Fill all 96 bits with uniformly-distributed randomness except for the bits that must be zero in
                    // order to stay under the exclusive maximum.
                    int low, mid, high;

                    if (parts.High != 0)
                    {
                        var inclusiveMaximum = parts.High - 1;

                        low = RawInt32();
                        mid = RawInt32();
                        high = RawInt32() & (int)MaskToRemoveBitsGuaranteedToExceedMaximum(inclusiveMaximum);
                    }
                    else if (parts.Mid != 0)
                    {
                        var inclusiveMaximum = parts.Mid - 1;

                        low = RawInt32();
                        mid = RawInt32() & (int)MaskToRemoveBitsGuaranteedToExceedMaximum(inclusiveMaximum);
                        high = 0;
                    }
                    else
                    {
                        var inclusiveMaximum = parts.Low - 1;

                        low = RawInt32() & (int)MaskToRemoveBitsGuaranteedToExceedMaximum(inclusiveMaximum);
                        mid = 0;
                        high = 0;
                    }

                    var result = new decimal(low, mid, high, false, 0);

                    // By masking out the random bits which would put the result at or over the exclusive maximum, the
                    // chance of having to loop and try again is strictly less than 50% in the worst-case scenario. The
                    // best-case scenario is 0% chance, and an average is 25% chance per iteration.

                    // For the worst-case scenario of 50% chance per iteration, the total chance of having to iterate
                    // twice is < 25%. Three times, 12.5%. And so on.
                    // For the average scenario of 25% chance per iteration, the total chance of having to iterate twice
                    // is < 6.25%. Three times, 1.5625%. And so on.
                    if (result < max)
                        return result;
                }
            }
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
            Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", nameof(max));

            // Check that the range is not greater than MaxValue without
            // first calculating it, since this would cause overflow
            Guard.ArgumentValid(max < 0M == min < 0M || min + decimal.MaxValue >= max,
                "Range too great for decimal data, use double range", nameof(max));

            if (min == max)
                return min;

            return NextDecimal(max - min) + min;
        }

        private static uint MaskToRemoveBitsGuaranteedToExceedMaximum(uint maximum)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2 but
            // without the value-- and value++

            var value = maximum;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value;
        }

        #endregion

        #region Guid

        /// <summary>
        /// Generates a valid version 4 <see cref="Guid"/>.
        /// </summary>
        public Guid NextGuid()
        {
            //We use the algorithm described in https://tools.ietf.org/html/rfc4122#section-4.4
            var b = new byte[16];
            NextBytes(b);
            //set the version to 4
            b[7] = (byte)((b[7] & 0x0f) | 0x40);
            //set the 2-bits indicating the variant to 1 and 0
            b[8] = (byte)((b[8] & 0x3f) | 0x80);
            return new Guid(b);
        }

        #endregion

        #region Helper Methods

        private int RawInt32()
        {
            return unchecked((int)RawUInt32());
        }

        private uint RawUInt32()
        {
            var buffer = new byte[sizeof(uint)];
            NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private ulong RawUInt64()
        {
            var buffer = new byte[sizeof(ulong)];
            NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        #endregion
    }
}
