// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class CollectionIntersectConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new CollectionIntersectConstraint(new int[] { 1, 2, 3, 4, 5 });
            stringRepresentation = "<intersectswith System.Int32[]>";
            expectedDescription = "intersects with < 1, 2, 3, 4, 5 >";
        }

        static object[] SuccessData = new object[] { new int[] { 1, 3, 5 }, new int[] { 1, 2, 3, 4, 5 }, new int[] { 1, 2, 3, 4, 5, 6, 7 } };
        static object[] FailureData = new object[] { 
            new object[] { new int[] { }, "<empty>" },
            new object[] { new int[] { 6, 7, 8 }, "< 6, 7, 8 >" } };

        [Test]
        [TestCaseSource(typeof(IgnoreCaseDataProvider), "TestCases")]
        public void HonorsIgnoreCase( IEnumerable expected, IEnumerable actual )
        {
            var constraint = new CollectionIntersectConstraint( expected ).IgnoreCase;
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
                    yield return new TestCaseData(new SimpleObjectCollection("w", "x", "y", "z"), new SimpleObjectCollection("z", "Y", "X", "N"));
                    yield return new TestCaseData(new[] {'Y', 'A', 'B', 'C', 'D', 'E', 'Z'}, new object[] {'x', 'a', 'b', 'c', "n" });
                    yield return new TestCaseData(new[] {"Y", "a", "b", "c", "d", "e", "z"}, new object[] {"X", "A", "C", "B", "N" });
                    yield return new TestCaseData(new Dictionary<int, string> {{1, "a"}, {2, "b"}}, new Dictionary<int, string> {{1, "A"}, {3, "C"}});
                    yield return new TestCaseData(new Dictionary<int, char> {{1, 'A'}, {2, 'B'}}, new Dictionary<int, char> {{1, 'a'}, {3, 'c'}});
                    yield return new TestCaseData(new Dictionary<string, int> {{"b", 2}, {"a", 1}}, new Dictionary<string, int> {{"B", 2}, {"c", 3}});
                    yield return new TestCaseData(new Dictionary<char, int> {{'A', 1 }, {'B', 2}}, new Dictionary<char, int> {{'a', 1}, {'c', 3}});

#if !PORTABLE && !NETSTANDARD1_6
                    yield return new TestCaseData(new Hashtable {{1, "a"}, {2, "b"}}, new Hashtable {{1, "A"}, {3, "C"}});
                    yield return new TestCaseData(new Hashtable {{1, 'A'}, {2, 'B'}}, new Hashtable {{2, 'b'}, {3, 'c'}});
                    yield return new TestCaseData(new Hashtable {{"b", 2}, {"a", 1}}, new Hashtable {{"A", 1}, {"C", 3}});
                    yield return new TestCaseData(new Hashtable {{'A', 1}, {'B', 2}}, new Hashtable {{'a', 1}, {'c', 3}});
#endif
                }
            }
        }

        [Test]
        public void IntersectsHonorsUsingWhenCollectionsAreOfDifferentTypes()
        {
            ICollection actual = new SimpleObjectCollection(2, 3, 4);
            ICollection expected = new SimpleObjectCollection("1", "2", "3");

            Assert.That(actual, Is.IntersectsWith(expected).Using<int, string>((i, s) => i.ToString() == s));
        }
    }
}
