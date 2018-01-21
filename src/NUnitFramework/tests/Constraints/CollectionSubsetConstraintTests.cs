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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class CollectionSubsetConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new CollectionSubsetConstraint(new int[] { 1, 2, 3, 4, 5 });
            stringRepresentation = "<subsetof System.Int32[]>";
            expectedDescription = "subset of < 1, 2, 3, 4, 5 >";
        }

        static object[] SuccessData = new object[] { new int[] { 1, 3, 5 }, new int[] { 1, 2, 3, 4, 5 } };
        static object[] FailureData = new object[] { 
            new object[] { new int[] { 1, 3, 7 }, "< 1, 3, 7 >" },
            new object[] { new int[] { 1, 2, 2, 2, 5 }, "< 1, 2, 2, 2, 5 >" } };

        [Test]
        [TestCaseSource(typeof(IgnoreCaseDataProvider), nameof(IgnoreCaseDataProvider.TestCases))]
        public void HonorsIgnoreCase( IEnumerable expected, IEnumerable actual )
        {
            var constraint = new CollectionSubsetConstraint( expected ).IgnoreCase;
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
                    yield return new TestCaseData(new SimpleObjectCollection("w", "x", "y", "z"), new SimpleObjectCollection("z", "Y", "X"));
                    yield return new TestCaseData(new[] {'A', 'B', 'C', 'D', 'E'}, new object[] {'a', 'b', 'c'});
                    yield return new TestCaseData(new[] {"a", "b", "c", "d", "e"}, new object[] {"A", "C", "B"});
                    yield return new TestCaseData(new Dictionary<int, string> {{1, "a"}, {2, "b"}}, new Dictionary<int, string> {{1, "A"}});
                    yield return new TestCaseData(new Dictionary<int, char> {{1, 'A'}, {2, 'B'}}, new Dictionary<int, char> {{1, 'a'}});
                    yield return new TestCaseData(new Dictionary<string, int> {{ "b", 2 }, { "a", 1 } }, new Dictionary<string, int> {{"b", 2}});
                    yield return new TestCaseData(new Dictionary<char, int> {{'A', 1 }, {'B', 2}}, new Dictionary<char, int> {{'a', 1}});

#if !NETCOREAPP1_1
                    yield return new TestCaseData(new Hashtable {{1, "a"}, {2, "b"}}, new Hashtable {{1, "A"}});
                    yield return new TestCaseData(new Hashtable {{1, 'A'}, {2, 'B'}}, new Hashtable {{2, 'b'}});
                    yield return new TestCaseData(new Hashtable {{"b", 2}, {"a", 1}}, new Hashtable {{"A", 1}});
                    yield return new TestCaseData(new Hashtable {{'A', 1}, {'B', 2}}, new Hashtable {{'a', 1}});
#endif
                }
            }
        }

        [Test]
        public void IsSubsetHonorsUsingWhenCollectionsAreOfDifferentTypes()
        {
            ICollection set = new SimpleObjectCollection("1", "2", "3", "4", "5");
            ICollection subset = new SimpleObjectCollection(2, 3);

            Assert.That(subset, Is.SubsetOf(set).Using<int, string>((i, s) => i.ToString() == s));
        }
    }
}
