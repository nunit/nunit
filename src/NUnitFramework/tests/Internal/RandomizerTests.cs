using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    public class RandomizerTests
    {
        private Randomizer _randomizer;

        [SetUp]
        public void CreateRandomizer()
        {
            _randomizer = new Randomizer();
        }

        #region Ints

        [Test]
        public void CanGetArrayOfRandomInts()
        {
            int[] ints = _randomizer.GetInts(10);
            Assert.That(ints.Length, Is.EqualTo(10));
        }

        [Test]
        public void CanGetArrayOfRandomIntsInRange()
        {
            int[] ints = _randomizer.GetInts(1, 100, 10);
            Assert.That(ints.Length, Is.EqualTo(10));
            foreach (int i in ints)
                Assert.That(i, Is.InRange(1, 100));
        }

        [Test]
        public void RandomIntsAreUnique()
        {
            UniqueValues.Check(_randomizer.GetInts(10), 8); // Heuristic
        }

        [Test]
        public void RandomIntsInRangeAreUnique()
        {
            UniqueValues.Check(_randomizer.GetInts(-300, 300, 10), 8); // Heuristic
        }

        #endregion

        #region Shorts

        [Test]
        public void CanGetRandomShort()
        {
            short s = _randomizer.NextShort();
            Assert.That(s, Is.GreaterThan((short)0));
        }

        [Test]
        public void CanGetRandomShortWithMaximum()
        {
            short zero = 0;
            short max = 100;
            short s = _randomizer.NextShort(max);
            Assert.That(s >= zero && s < max, "Out of range");
        }

        [Test]
        public void CanGetRandomShortInRange()
        {
            short min = -10;
            short max = 100;
            short s = _randomizer.NextShort(min, max);
            Assert.That(s >= min && s < max, "Out of range");
        }

        [Test]
        public void CanGetArrayOfRandomShorts()
        {
            short[] shorts = _randomizer.GetShorts(10);
            Assert.That(shorts.Length, Is.EqualTo(10));
        }

        [Test]
        public void CanGetArrayOfRandomShortsInRange()
        {
            short[] shorts = _randomizer.GetShorts(1, 100, 10);
            Assert.That(shorts.Length, Is.EqualTo(10));
            foreach (int i in shorts)
                Assert.That(i, Is.InRange(1, 100));
        }

        [Test]
        public void RandomShortsAreUnique()
        {
            UniqueValues.Check(_randomizer.GetShorts(10), 8); // Heuristic
        }

        [Test]
        public void RandomShortsInRangeAreUnique()
        {
            UniqueValues.Check(_randomizer.GetShorts(-300, 300, 10), 8); // Heuristic
        }

        #endregion

        #region Bytes

        [Test]
        public void CanGetRandomByte()
        {
            byte b = _randomizer.NextByte();
            Assert.That(b >= byte.MinValue && b < byte.MaxValue);
        }

        [Test]
        public void CanGetRandomByteWithMaximum()
        {
            byte max = (byte)96;
            byte b = _randomizer.NextByte(max);
            Assert.That(b >= byte.MinValue && b < max);
        }

        public void CanGetRandomByteInRange()
        {
            byte min = (byte)16;
            byte max = (byte)96;
            byte b = _randomizer.NextByte();
            Assert.That(b >= min && b < max);
        }

        [Test]
        public void CanGetArrayOfRandomBytes()
        {
            byte[] bytes = _randomizer.GetBytes(10);
            Assert.That(bytes.Length, Is.EqualTo(10));
        }

        [Test]
        public void CanGetArrayOfRandomBytesInRange()
        {
            byte min = (byte)16;
            byte max = (byte)96;
            byte[] bytes = _randomizer.GetBytes(min, max, 10);
            Assert.That(bytes.Length, Is.EqualTo(10));
            foreach (byte b in bytes)
                Assert.That(b, Is.InRange(min, max));
        }

        [Test]
        public void RandomBytesAreUnique()
        {
            UniqueValues.Check(_randomizer.GetBytes(10), 8); // Heuristic
        }

        [Test]
        public void RandomBytesInRangeAreUnique()
        {
            UniqueValues.Check(_randomizer.GetBytes(
                byte.MinValue, byte.MaxValue, 10), 8); // Heuristic
        }

        #endregion

        #region Bool

        [Test]
        public void CanGetRandomBool()
        {
            bool haveTrue = false;
            bool haveFalse = false;
            int attempts = 0;
            while (!haveTrue && !haveFalse)
            {
                if (attempts++ > 10000)
                {
                    Assert.Fail("No randomness in 10000 attempts");
                }
                if (_randomizer.NextBool())
                    haveTrue = true;
                else
                    haveFalse = true;
            }
        }

        [Test]
        public void CanGetRandomBoolWithProbability()
        {
            bool haveTrue = false;
            bool haveFalse = false;
            int attempts = 0;
            while (!haveTrue && !haveFalse)
            {
                if (attempts++ > 10000)
                {
                    Assert.Fail("No randomness in 10000 attempts");
                }
                if (_randomizer.NextBool(.25))
                    haveTrue = true;
                else
                    haveFalse = true;
            }
        }

        [Test]
        public void RandomBoolWithProbabilityZeroIsAlwaysFalse()
        {
            for (int i = 0; i < 10; i++)
                Assert.False(_randomizer.NextBool(0.0));
        }

        public void RandomBoolWithProbabilityOneIsAlwaysTrue()
        {
            for (int i = 0; i < 10; i++)
                Assert.True(_randomizer.NextBool(1.0));
        }

        #endregion

        #region Doubles

        [Test]
        public void CanGetRandomDouble()
        {
            double d = _randomizer.NextDouble();
            Assert.That(d, Is.GreaterThan(0.0));
        }

        [Test]
        public void CanGetRandomDoubleWithMaximum()
        {
            double d = _randomizer.NextDouble(100.0);
            Assert.That(d >= 0.0 && d < 100.0, "Out of range");
        }

        [Test]
        public void CanGetRandomDoubleInRange()
        {
            double d = _randomizer.NextDouble(0.2, 0.7);
            Assert.That(d >= 0.2 && d < 0.7, "Out of range");
        }

        [Test]
        public void CanGetArrayOfRandomDoubles()
        {
            double[] doubles = _randomizer.GetDoubles(10);
            Assert.That(doubles.Length, Is.EqualTo(10));
        }

        [Test]
        public void CanGetArrayOfRandomDoublesInRange()
        {
            double[] doubles = _randomizer.GetDoubles(0.5, 2.5, 10);
            Assert.That(doubles.Length, Is.EqualTo(10));
            foreach (double d in doubles)
                Assert.That(d, Is.InRange(0.5, 2.5));
        }

        [Test]
        public void RandomDoublesAreUnique()
        {
            UniqueValues.Check(_randomizer.GetDoubles(10), 8); // Heuristic
        }

        [Test]
        public void RandomDoublesInRangeAreUnique()
        {
            UniqueValues.Check(_randomizer.GetDoubles(0.1, 0.7, 10), 8); // Heuristic
        }

        #endregion

        #region Floats

        [Test]
        public void CanGetRandomFloat()
        {
            float f = _randomizer.NextFloat();
            Assert.That(f, Is.GreaterThan(0.0f));
        }

        [Test]
        public void CanGetRandomFloatWithMaximum()
        {
            float f = _randomizer.NextFloat(100.0f);
            Assert.That(f >= 0.0f && f < 100.0f, "Out of range");
        }

        [Test]
        public void CanGetRandomFloatInRange()
        {
            float f = _randomizer.NextFloat(0.2f, 0.7f);
            Assert.That(f >= 0.2f && f < 0.7f, "Out of range");
        }

        [Test]
        public void CanGetArrayOfRandomFloats()
        {
            float[] floats = _randomizer.GetFloats(10);
            Assert.That(floats.Length, Is.EqualTo(10));
        }

        [Test]
        public void CanGetArrayOfRandomFloatsInRange()
        {
            float[] floats = _randomizer.GetFloats(0.5f, 2.5f, 10);
            Assert.That(floats.Length, Is.EqualTo(10));
            foreach (float f in floats)
                Assert.That(f, Is.InRange(0.5f, 2.5f));
        }

        [Test]
        public void RandomFloatsAreUnique()
        {
            UniqueValues.Check(_randomizer.GetFloats(10), 8); // Heuristic
        }

        [Test]
        public void RandomFloatsInRamgeAreUnique()
        {
            UniqueValues.Check(_randomizer.GetFloats(0.5f, 1.5f, 10), 8); // Heuristic
        }

        #endregion
    
        #region Strings
                
        [Test]
        [Description("Test that all generated strings are unique")]
        public void RandomStringsAreUnique()
        {
            string[] values = new string[10];
            for (int i = 0; i < 10; i++)
              values[i] = _randomizer.GetString();

            UniqueValues.Check(values, 8); // Heuristic
        }
        
        [TestCase(30, "Tｈｅɋúｉｃｋƃｒòｗｎｆ߀хｊｕｍｐëԁoѵerｔհëｌａȥｙｄｏɢ", 8)]
        [TestCase(200, "ａèí߀ù123456", 8)]
        [TestCase(1000, Randomizer.DefaultStringChars, 8)]
        [Description("Test that all generated strings are unique for varying output length")]
        public void RandomStringsAreUnique(int outputLength, string allowedChars, int min)
        {
            string[] values = new string[10];
            for (int i = 0; i < 10; i++)
              values[i] = _randomizer.GetString(outputLength, allowedChars);

            UniqueValues.Check(values, min); // Heuristic
        }
        
        #endregion

        #region Enums

        [Test]
        public void CanGetRandomEnum()
        {
            object e = _randomizer.NextEnum(typeof(AttributeTargets));
            Assert.That(e, Is.TypeOf<AttributeTargets>());
        }

        [Test]
        public void CanGetRandomEnum_Generic()
        {
            AttributeTargets at = _randomizer.NextEnum<AttributeTargets>();
        }

        [Test]
        public void CanGetArrayOfRandomEnums()
        {
            object[] enums = _randomizer.GetEnums(10, typeof(AttributeTargets));
            Assert.That(enums.Length, Is.EqualTo(10));
            foreach (object e in enums)
                Assert.That(e, Is.TypeOf(typeof(AttributeTargets)));
        }

        #endregion

        #region Repeatability

        public static class Repeatability
        {
            [Test]
            public static void RandomizersWithSameSeedsReturnSameValues()
            {
                var r1 = new Randomizer(1234);
                var r2 = new Randomizer(1234);

                for (int i = 0; i < 10; i++)
                    Assert.That(r1.NextDouble(), Is.EqualTo(r2.NextDouble()));
            }

            [Test]
            public static void RandomizersWithDifferentSeedsReturnDifferentValues()
            {
                var r1 = new Randomizer(1234);
                var r2 = new Randomizer(4321);

                for (int i = 0; i < 10; i++)
                    Assert.That(r1.NextDouble(), Is.Not.EqualTo(r2.NextDouble()));
            }

            [Test]
            public static void ReturnsSameRandomizerForSameParameter()
            {
                ParameterInfo p = testMethod1.GetParameters()[0];
                var r1 = Randomizer.GetRandomizer(p);
                var r2 = Randomizer.GetRandomizer(p);
                Assert.That(r1, Is.SameAs(r2));
            }

            [Test]
            public static void ReturnsSameRandomizerForDifferentParametersOfSameMethod()
            {
                ParameterInfo p1 = testMethod1.GetParameters()[0];
                ParameterInfo p2 = testMethod1.GetParameters()[1];
                var r1 = Randomizer.GetRandomizer(p1);
                var r2 = Randomizer.GetRandomizer(p2);
                Assert.That(r1, Is.SameAs(r2));
            }

            [Test]
            public static void ReturnsSameRandomizerForSameMethod()
            {
                var r1 = Randomizer.GetRandomizer(testMethod1);
                var r2 = Randomizer.GetRandomizer(testMethod1);
                Assert.That(r1, Is.SameAs(r2));
            }

            [Test]
            public static void ReturnsDifferentRandomizersForDifferentMethods()
            {
                var r1 = Randomizer.GetRandomizer(testMethod1);
                var r2 = Randomizer.GetRandomizer(testMethod2);
                Assert.That(r1, Is.Not.SameAs(r2));
            }

            static readonly MethodInfo testMethod1 =
                typeof(Repeatability).GetMethod("TestMethod1", BindingFlags.NonPublic | BindingFlags.Static);
            private static void TestMethod1(int x, int y)
            {
            }

            static readonly MethodInfo testMethod2 =
                typeof(Repeatability).GetMethod("TestMethod2", BindingFlags.NonPublic | BindingFlags.Static);
            private static void TestMethod2(int x, int y)
            {
            }
        }

        #endregion
    }
}
