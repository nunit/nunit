// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    public class CollectionEquivalentConstraintTests
    {
        [Test]
        public void EqualCollectionsAreEquivalent()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("x", "y", "z");

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void WorksWithCollectionsOfArrays()
        {
            byte[] array1 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array2 = new byte[] { 0x42, 0x52, 0x72, 0xef };
            byte[] array3 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array4 = new byte[] { 0x42, 0x52, 0x72, 0xef };

            ICollection set1 = new SimpleObjectCollection(array1, array2);
            ICollection set2 = new SimpleObjectCollection(array3, array4);

            Constraint constraint = new CollectionEquivalentConstraint(set1);
            Assert.That(constraint.ApplyTo(set2).IsSuccess);

            set2 = new SimpleObjectCollection(array4, array3);
            Assert.That(constraint.ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentIgnoresOrder()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("z", "y", "x");

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInActual()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("x", "y", "x");

            Assert.False(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInExpected()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "x");
            ICollection set2 = new SimpleObjectCollection("x", "y", "z");

            Assert.False(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentFailsWithExtraItemsInActual()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y");
            ICollection set2 = new SimpleObjectCollection("x", "x", "y");

            Assert.False(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHandlesNull()
        {
            ICollection set1 = new SimpleObjectCollection(null, "x", null, "z");
            ICollection set2 = new SimpleObjectCollection("z", null, "x", null);

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHonorsIgnoreCase()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1).IgnoreCase.ApplyTo(set2).IsSuccess);
        }

        [Test]
        [TestCaseSource(typeof(IgnoreCaseDataProvider), nameof(IgnoreCaseDataProvider.TestCases))]
        public void HonorsIgnoreCase( IEnumerable expected, IEnumerable actual )
        {
            var constraint = new CollectionEquivalentConstraint( expected ).IgnoreCase;
            var constraintResult = constraint.ApplyTo( actual );
            if ( !constraintResult.IsSuccess )
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo( writer );
                Assert.Fail( writer.ToString() );
            }
        }

        public class IgnoreCaseDataProvider
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(new SimpleObjectCollection("x", "y", "z"), new SimpleObjectCollection("z", "Y", "X"));
                    yield return new TestCaseData(new[] {'A', 'B', 'C'}, new object[] {'a', 'c', 'b'});
                    yield return new TestCaseData(new[] {"a", "b", "c"}, new object[] {"A", "C", "B"});
                    yield return new TestCaseData(new Dictionary<int, string> {{2, "b"}, {1, "a"}}, new Dictionary<int, string> {{1, "A"}, {2, "b"}});
                    yield return new TestCaseData(new Dictionary<int, char> {{1, 'A'}}, new Dictionary<int, char> {{1, 'a'}});
                    yield return new TestCaseData(new Dictionary<string, int> {{ "b", 2 }, { "a", 1 } }, new Dictionary<string, int> {{"A", 1}, {"b", 2}});
                    yield return new TestCaseData(new Dictionary<char, int> {{'A', 1 }}, new Dictionary<char, int> {{'a', 1}});

#if !NETCOREAPP1_1
                    yield return new TestCaseData(new Hashtable {{1, "a"}, {2, "b"}}, new Hashtable {{1, "A"},{2, "B"}});
                    yield return new TestCaseData(new Hashtable {{1, 'A'}, {2, 'B'}}, new Hashtable {{1, 'a'},{2, 'b'}});
                    yield return new TestCaseData(new Hashtable {{"b", 2}, {"a", 1}}, new Hashtable {{"A", 1}, {"b", 2}});
                    yield return new TestCaseData(new Hashtable {{'A', 1}}, new Hashtable {{'a', 1}});
#endif
                }
            }
        }


        [Test]
        public void EquivalentHonorsUsing()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1)
                .Using<string>((x, y) => StringUtil.Compare(x, y, true))
                .ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHonorsUsingWhenCollectionsAreOfDifferentTypes()
        {
            ICollection strings = new SimpleObjectCollection("1", "2", "3");
            ICollection ints = new SimpleObjectCollection(1, 2, 3);

            Assert.That(ints, Is.EquivalentTo(strings).Using<int, string>((i, s) => i.ToString() == s));
        }

#if !(NET20 || NET35)
        [Test]
        public static void UsesProvidedGenericEqualityComparison()
        {
            var comparer = new GenericEqualityComparison<int>();
            Assert.That(new[] { 1 }, Is.EquivalentTo(new[] { 1 }).Using<int>(comparer.Delegate));
            Assert.That(comparer.WasCalled, "Comparer was not called");
        }

        [Test]
        public static void UsesBooleanReturningDelegateWithImplicitParameterTypes()
        {
            Assert.That(new[] { 1 }, Is.EquivalentTo(new[] { 1 }).Using<int>((x, y) => x.Equals(y)));
        }

        [Test]
        public void CheckCollectionEquivalentConstraintResultIsReturned()
        {
            IEnumerable<string> set1 = new List<string>() { "one" };
            IEnumerable<string> set2 = new List<string>() { "two" };

            Assert.IsInstanceOf(typeof(CollectionEquivalentConstraintResult),
                new CollectionEquivalentConstraint(set1).ApplyTo(set2));
        }

        /// <summary>
        /// A singular point test to ensure that the <see cref="ConstraintResult"/> returned by
        /// <see cref="CollectionEquivalentConstraint"/> includes the feature of describing both
        /// extra and missing elements when the collections are not equivalent.
        /// </summary>
        /// <remarks>
        /// This is not intended to fully test the display of missing/extra elements, but to ensure
        /// that the functionality is actually there.
        /// </remarks>
        [Test]
        public void TestConstraintResultMessageDisplaysMissingAndExtraElements()
        {
            List<string> expectedCollection = new List<string>() { "one", "two" };
            List<string> actualCollection = new List<string>() { "three", "one" };

            ConstraintResult cr = new CollectionEquivalentConstraint(expectedCollection).ApplyTo(actualCollection);

            TextMessageWriter writer = new TextMessageWriter();
            cr.WriteMessageTo(writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"three\", \"one\" >" + Environment.NewLine +
                "  Missing (1): < \"two\" >" + Environment.NewLine +
                "  Extra (1): < \"three\" >" + Environment.NewLine;
            Assert.AreEqual(expectedMsg, writer.ToString());
        }

        [Test]
        public void WorksWithHashSets()
        {
            var hash1 = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var hash2 = new HashSet<string>(new string[] { "abracadabra", "presto", "hocuspocus" });

            Assert.That(new CollectionEquivalentConstraint(hash1).ApplyTo(hash2).IsSuccess);
        }

        [Test]
        public void WorksWithHashSetAndArray()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocuspocus" };

            var constraint = new CollectionEquivalentConstraint(hash);
            Assert.That(constraint.ApplyTo(array).IsSuccess);
        }

        [Test]
        public void WorksWithArrayAndHashSet()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocuspocus" };

            var constraint = new CollectionEquivalentConstraint(array);
            Assert.That(constraint.ApplyTo(hash).IsSuccess);
        }

        [Test]
        public void FailureMessageWithHashSetAndArray()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocusfocus" };

            var constraint = new CollectionEquivalentConstraint(hash);
            var constraintResult = constraint.ApplyTo(array);
            Assert.False(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);

            var expectedMessage =
                "  Expected: equivalent to < \"presto\", \"abracadabra\", \"hocuspocus\" >" + Environment.NewLine +
                "  But was:  < \"abracadabra\", \"presto\", \"hocusfocus\" >" + Environment.NewLine +
                "  Missing (1): < \"hocuspocus\" >" + Environment.NewLine +
                "  Extra (1): < \"hocusfocus\" >" + Environment.NewLine;

            Assert.That(writer.ToString(), Is.EqualTo(expectedMessage));
        }
#endif
    }
}
