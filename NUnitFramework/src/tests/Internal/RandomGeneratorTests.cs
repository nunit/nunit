using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitLite.Tests.Internal
{
    public class RandomGeneratorTests
    {
        #region Properties & Constructor
        public RandomGeneratorTests()
        {
        }
        #endregion

        #region Ints
        [Test]
        public void RandomIntsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetInt();

            Assert.That(values, Is.Unique);
        }
        [TestCase(-300,300)]
        public void RandomIntsAreUnique(int min, int max)
        {
           RandomGenerator r = new RandomGenerator(new Random().Next());
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetInt(min,max);

            Assert.That(values, Is.Unique);
        }
        #endregion

        #region Shorts
        [Test]
        public void RandomShortsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetShort();

            Assert.That(values, Is.Unique);
        }
        [TestCase(short.MinValue,short.MaxValue)]
        public void RandomShortsAreUnique(short min, short max)
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetShort(min, max);

            Assert.That(values, Is.Unique);
        }
        #endregion

        #region Btyes
        [Test]
        public void RandomBytesAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetByte();

            Assert.That(values, Is.Unique);
        }

        [TestCase(byte.MinValue,byte.MaxValue)]
        public void RandomBytesAreUnique(byte min, byte max)
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetByte();

            Assert.That(values, Is.Unique);
        }
        #endregion

        #region Bool
        [Test]
        public void CanGetRandomBool()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            bool[] values = new bool[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetBool();
            Assert.That(values, Contains.Item(true));
            Assert.That(values, Contains.Item(false));
        }

        public void CanGetRandomBoolWithProbability()
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
        public void RandomDoublesAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetDouble();

            Assert.That(values, Is.Unique);
        }

        [Test]
        public void RandomFloatsAreUnique()
        {
            RandomGenerator r = new RandomGenerator(new Random().Next());
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.GetFloat();

            Assert.That(values, Is.Unique);
        }
        #endregion
    }
}
