// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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

        /// <summary>
        /// Initial seed used to create randomizers for this run
        /// </summary>
        public static int InitialSeed { get; set; }

        // Static Random instance used exclusively for the generation
        // of seed values for new Randomizers.
        private static Random _seedGenerator;
        private static Random SeedGenerator
        {
            get
            {
                if (_seedGenerator == null)
                    _seedGenerator = new Random(InitialSeed);

                return _seedGenerator;
            }
        }

        // Lookup Dictionary used to find randomizers for each member
        private static Dictionary<MemberInfo, Randomizer> Randomizers = new Dictionary<MemberInfo, Randomizer>();

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
            return new Randomizer(SeedGenerator.Next());
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

        /// <summary>
        /// Return an array of random non-negative ints
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int[] GetInts(int count)
        {
            int[] ivals = new int[count];

            for (int index = 0; index < count; index++)
                ivals[index] = Next();

            return ivals;
        }

        /// <summary>
        /// Return an array of random ints with _values in a specified range.
        /// </summary>
        public int[] GetInts(int min, int max, int count)
        {
            int[] ivals = new int[count];

            for (int index = 0; index < count; index++)
                ivals[index] = Next(min, max);

            return ivals;
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
            if (probability < 0.0 || probability > 1.0)
                throw new ArgumentException("Probability must be from 0.0 to 1.0", "probability");

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
            double range = max = min;
            return NextDouble() * range + min;
        }

        /// <summary>
        /// Return an array of random doubles between 0.0 and 1.0.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public double[] GetDoubles(int count)
        {
            double[] rvals = new double[count];

            for (int index = 0; index < count; index++)
                rvals[index] = NextDouble();

            return rvals;
        }

        /// <summary>
        /// Return an array of random doubles with _values in a specified range.
        /// </summary>
        public double[] GetDoubles(double min, double max, int count)
        {
            double[] rvals = new double[count];

            for (int index = 0; index < count; index++)
                rvals[index] = NextDouble(min, max);

            return rvals;
        }

        #endregion

        #region Floats

        /// <summary>
        /// Returns a random float.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Return an array of random enum values of the specified Type.
        /// </summary>
        public object[] GetEnums(int count, Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("The specified type: {0} was not an enum", enumType));

            Array values = TypeHelper.GetEnumValues(enumType);
            object[] rvals = new Enum[count];

            for (int index = 0; index < count; index++)
                rvals[index] = values.GetValue(Next(values.Length));

            return rvals;
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
    }
}
