using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    public class RandomizerTests
    {
        private Randomizer r;

        [SetUp]
        public void CreateRandomizer()
        {
            r = new Randomizer();
        }

        #region Ints

        [Test]
        public void RandomIntsAreUnique()
        {
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.Next();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(-300,300)]
        public void RandomIntsAreUnique(int min, int max)
        {
            int[] values = new int[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.Next(min,max);

            UniqueValues.Check(values, 8); // Heuristic
        }

        [Test]
        public void CanGetArrayOfRandomInts()
        {
            int[] ints = r.GetInts(1, 100, 10);
            Assert.That(ints.Length, Is.EqualTo(10));
            foreach (int i in ints)
                Assert.That(i, Is.InRange(1, 100));
        }


        #endregion

        #region Shorts

        [Test]
        public void RandomShortsAreUnique()
        {
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextShort();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(-300, 300)]
        public void RandomShortsAreUnique(short min, short max)
        {
            short[] values = new short[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextShort(min, max);

            UniqueValues.Check(values, 8); // Heuristic
        }

        #endregion

        #region Bytes

        [Test]
        public void RandomBytesAreUnique()
        {
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextByte();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [TestCase(byte.MinValue,byte.MaxValue)] // TODO: Fails occasionally
        public void RandomBytesAreUnique(byte min, byte max)
        {
            byte[] values = new byte[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextByte();

            UniqueValues.Check(values, 8); // Heuristic
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
                if (r.NextBool())
                    haveTrue = true;
                else
                    haveFalse = true;
            }
        }

        public void CanGetRandomBoolWithProbability()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.True(r.NextBool(.0));
                Assert.False(r.NextBool(1.0));
            }
        }

        #endregion

        #region Doubles

        [Test]
        public void RandomDoublesAreUnique()
        {
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextDouble();

            UniqueValues.Check(values, 8); // Heuristic
        }

        [Test]
        public void CanGetArrayOfRandomDoubles()
        {
            double[] doubles = r.GetDoubles(0.5, 1.5, 10);
            Assert.That(doubles.Length, Is.EqualTo(10));
            foreach (double d in doubles)
                Assert.That(d, Is.InRange(0.5, 1.5));

            // Heuristic: Could fail occasionally
            Assert.That(doubles, Is.Unique);
        }

        #endregion

        #region Floats

        [Test]
        public void RandomFloatsAreUnique()
        {
            double[] values = new double[10];
            for (int i = 0; i < 10; i++)
                values[i] = r.NextFloat();

            UniqueValues.Check(values, 8); // Heuristic
        }

        #endregion
    
        #region Strings
                
        [Test]
        [Description("Test that all generated strings are unique")]
        public void RandomStringsAreUnique()
        {
            string[] values = new string[10];
            for (int i = 0; i < 10; i++)
              values[i] = r.GetString();

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
              values[i] = r.GetString(outputLength, allowedChars);

            UniqueValues.Check(values, min); // Heuristic
        }
        
        #endregion

        #region Enums

        [Test]
        public void CanGetArrayOfRandomEnums()
        {
            object[] enums = r.GetEnums(10, typeof(AttributeTargets));
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
