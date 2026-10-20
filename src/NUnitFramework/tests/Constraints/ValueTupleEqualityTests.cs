// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ValueTupleEqualityTests
    {
        private const string Hello = "Hello";

        [Test]
        public void SucceedsWhenTuplesAreEmpty()
        {
            ValueTuple tuple1 = ValueTuple.Create();
            ValueTuple tuple2 = ValueTuple.Create();
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenSingleTuplesAreTheSame()
        {
            ValueTuple<int> tuple1 = ValueTuple.Create(3);
            ValueTuple<int> tuple2 = ValueTuple.Create(3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenTuplesAreTheSame()
        {
            ValueTuple<string, int> tuple1 = (Hello, 3);
            ValueTuple<string, int> tuple2 = (Hello, 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenTuplesAreTheSameWithinTolerance()
        {
            ValueTuple<string, int> tuple1 = (Hello, 3);
            ValueTuple<string, int> tuple2 = (Hello, 4);
            Assert.That(tuple1, Is.EqualTo(tuple2).Within(1));
        }

        [Test]
        public void FailsWhenNoneOfTheTupleMembersSupportTolerance()
        {
            // We need two identical strings, but not the same reference.
            // Equals bails out when to instances have the same reference, before calling the appropriate comparer.
            // So we need a different reference to trigger calling the comparer, which will then reject tolerance for strings.
            string alsoHello = string.Format("{0}", Hello);
            Assert.That(alsoHello, Is.Not.SameAs(Hello));
            Assert.That(alsoHello, Is.EqualTo(Hello));

            ValueTuple<string, string> tuple1 = (Hello, alsoHello);
            ValueTuple<string, string> tuple2 = (alsoHello, Hello);
            Assert.That(tuple1, Is.EqualTo(tuple2));

#pragma warning disable NUnit2047 // Incompatible types for Within constraint
            Assert.That(() => Assert.That(tuple1, Is.EqualTo(tuple2).Within(1)),
                        Throws.InstanceOf<NotSupportedException>());
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalent()
        {
            ValueTuple<string, int> tuple1 = (Hello, 3);
            ValueTuple<string, int?> tuple2 = (Hello, 3);
            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith8Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = (1, 2, 3, 4, 5, 6, 7, 8.0f);

            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsWhenContentOfTuplesAreEquivalentWith15Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            var tuple2 = (1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f, 11.0f, 12.0f, 13.0f, 14.0f, 15.0f);

            Assert.That(tuple1, Is.EqualTo(tuple2));
        }

        [Test]
        public void SucceedsForTuplesWith32Elements()
        {
            const string thirteen = "13";
            string alsoThirteen = string.Format("{0}", thirteen);
            Assert.That(thirteen, Is.Not.SameAs(alsoThirteen));
            Assert.That(thirteen, Is.EqualTo(alsoThirteen));

            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, thirteen, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
            var tuple2 = (2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 12, 12, alsoThirteen, 14, 16, 16, 18, 18, 20, 20, 22, 22, 24, 24, 26, 26, 28, 28, 30, 30, 32, 32);

            Assert.That(tuple2, Is.EqualTo(tuple1).Within(1));
        }

        [TestCase(64)]
        [TestCase(128)]
        [TestCase(512)]
        ////[TestCase(999)] // Causes StackOverflowException in System.ValueTuple
        public void SucceedsOnRidiculousLargeTuples(int numberOfElements)
        {
            const string fixtureName = "LargeTuplesTest";
            string numbers = string.Join(", ", Enumerable.Range(1, numberOfElements - 1));
            string code = $$"""
                using NUnit.Framework;

                [TestFixture]
                public sealed class {{fixtureName}}
                {
                    [Test]
                    public void Test1()
                    {
                        const string Text = "Text";
                        string alsoText = string.Format("{0}", Text);

                        var tuple1 = (Text, {{numbers}});
                        var tuple2 = (alsoText, {{numbers}});

                        Assert.That(tuple2, Is.EqualTo(tuple1).Within(1));
                    }
                }
                """;

            var compiler = new TestCompiler(typeof(Assert));
            var assembly = compiler.GenerateInMemoryAssembly(code);
            var testType = assembly.GetType(fixtureName);
            Assert.That(testType, Is.Not.Null);
            var fixture = TestBuilder.MakeFixture(testType);

            ITestResult result = TestBuilder.RunTest(fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void FailsWhenTuplesAreOfDifferentLengths()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int, int> tuple2 = ("Hello", 3, 1);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferent()
        {
            ValueTuple<string, int> tuple1 = ("Hello", 3);
            ValueTuple<string, int> tuple2 = ("Hello", 4);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void FailsWhenContentOfTuplesAreDifferentWith8Elements()
        {
            var tuple1 = (1, 2, 3, 4, 5, 6, 7, 8);
            var tuple2 = (1, 2, 3, 4, 5, 6, 7, 9);
            Assert.That(tuple1, Is.Not.EqualTo(tuple2));
        }

        [Test]
        public void ValueTupleElementsAreComparedUsingNUnitEqualityComparer()
        {
            var a = ValueTuple.Create(new Dictionary<string, string>());
            var b = ValueTuple.Create(new Dictionary<string, string>());
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void SucceedsWithStringModifiersIgnoreCaseAndIgnoreWhiteSpace()
        {
            var a = (2, 5, "HELLO ");
            var b = (2, 5, "hello");

            Assert.That(a, Is.EqualTo(b).IgnoreCase.IgnoreWhiteSpace);
        }

        [Test]
        public void SucceedsWithStringModifiersIgnoreCaseAndLineEnding()
        {
            var a = (2, 5, "HELLO\r");
            var b = (2, 5, "hello\n");

            Assert.That(a, Is.EqualTo(b).IgnoreCase.IgnoreLineEndingFormat);
        }

        [Test]
        public void CanCompareTuplesWithDifferentTypes()
        {
            ValueTuple<int, int, int> intTuple = (1, 2, 3);
            ValueTuple<double, double, double> doubleTuple = (1.0, 2.0, 3.0);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(doubleTuple, Is.EqualTo(intTuple));
                Assert.That(intTuple, Is.EqualTo(doubleTuple));
            }
        }

        [Test]
        public void CanCompareTuplesWithValueTuples()
        {
            Tuple<int, int, int> tuple = Tuple.Create(1, 2, 3);
            ValueTuple<double, double, double> valueTuple = (1.0, 2.0, 3.0);

            using (Assert.EnterMultipleScope())
            {
                // This is a side effect of using ITuple to compare members.
                // NUnit allows comparing Tuple<int> with Tuple<double> so why not allow ValueTuple.
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
                Assert.That(valueTuple, Is.EqualTo(tuple));
                Assert.That(tuple, Is.EqualTo(valueTuple));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            }
        }
    }
}
