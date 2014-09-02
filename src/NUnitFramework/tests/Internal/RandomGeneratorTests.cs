using System;
using System.Collections;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    public static class RandomGeneratorTests
    {
        #region Ints
        [Test]
        public static void RandomIntsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetInt();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(-300,300)]
        public static void RandomIntsAreUnique(int min, int max)
        {
           RandomGenerator r = new RandomGenerator(new Random().Next());
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetInt(min,max);

            UniqueValues.Check(values, 8); // Heuristic
        }
        #endregion

        #region Shorts
        [Test]
        public static void RandomShortsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetShort();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(-300, 300)]
        public static void RandomShortsAreUnique(short min, short max)
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetShort(min, max);

            UniqueValues.Check(values, 8); // Heuristic
        }
        #endregion

        #region Bytes
        [Test]
        public static void RandomBytesAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetByte();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(byte.MinValue,byte.MaxValue)] // TODO: Fails occasionally
        public static void RandomBytesAreUnique(byte min, byte max)
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetByte();

            UniqueValues.Check(values, 8); // Heuristic
        }
        #endregion

        #region Bool
        [Test]
        public static void CanGetRandomBool()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            bool haveTrue = false;
            bool haveFalse = false;
            int attempts = 0;
            while (!haveTrue && !haveFalse)
            {
                if (attempts++ > 10000)
                {
                    Assert.Fail("No randomness in 10000 attempts");
                }
                if (r.GetBool())
                    haveTrue = true;
                else
                    haveFalse = true;
            }
        }

        public static void CanGetRandomBoolWithProbability()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            for (int i = 0; i < 10; i++)
            {
                Assert.True(r.GetBool(.0));
                Assert.False(r.GetBool(1.0));
            }
        }
        #endregion

        #region Doubles & Floats
        [Test]
        public static void RandomDoublesAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetDouble();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [Test]
        public static void RandomFloatsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetFloat();

            UniqueValues.Check(values, 8); // Heuristic
        }
        #endregion
    }
}
