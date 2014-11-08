// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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
using NUnit.TestUtilities;
using NUnit.TestUtilities.Collections;
using NUnit.TestUtilities.Comparers;

#if !SILVERLIGHT
using System.Data;
#endif

#if NET_3_5 || NET_4_0 || SILVERLIGHT || NETCF_3_5 || PORTABLE
using System.Linq;
#endif

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Test Library for the NUnit CollectionAssert class.
    /// </summary>
    [TestFixture()]
    public class CollectionAssertTest
    {
        #region AllItemsAreInstancesOfType
        [Test()]
        public void ItemsOfType()
        {
            var collection = new SimpleObjectCollection("x", "y", "z");
            CollectionAssert.AllItemsAreInstancesOfType(collection,typeof(string));
        }

        [Test]
        public void ItemsOfTypeFailure()
        {
            var collection = new SimpleObjectCollection("x", "y", new object());

            var expectedMessage =
                "  Expected: all items instance of <System.String>" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", <System.Object> >" + Environment.NewLine;
            
            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AllItemsAreInstancesOfType(collection,typeof(string)));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        #endregion

        #region AllItemsAreNotNull
        [Test()]
        public void ItemsNotNull()
        {
            var collection = new SimpleObjectCollection("x", "y", "z");
            CollectionAssert.AllItemsAreNotNull(collection);
        }

        [Test]
        public void ItemsNotNullFailure()
        {
            var collection = new SimpleObjectCollection("x", null, "z");

            var expectedMessage =
                "  Expected: all items not null" + Environment.NewLine +
                "  But was:  < \"x\", null, \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AllItemsAreNotNull(collection));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        #endregion

        #region AllItemsAreUnique

        [Test]
        public void Unique_WithObjects()
        {
            CollectionAssert.AllItemsAreUnique(
                new SimpleObjectCollection(new object(), new object(), new object()));
        }

        [Test]
        public void Unique_WithStrings()
        {
            CollectionAssert.AllItemsAreUnique(new SimpleObjectCollection("x", "y", "z"));
        }

        [Test]
        public void Unique_WithNull()
        {
            CollectionAssert.AllItemsAreUnique(new SimpleObjectCollection("x", "y", null, "z"));
        }

        [Test]
        public void UniqueFailure()
        {
            var expectedMessage =
                "  Expected: all items unique" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"x\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AllItemsAreUnique(new SimpleObjectCollection("x", "y", "x")));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void UniqueFailure_WithTwoNulls()
        {
            Assert.Throws<AssertionException>(
                () => CollectionAssert.AllItemsAreUnique(new SimpleObjectCollection("x", null, "y", null, "z")));
        }

        #endregion

        #region AreEqual

        [Test]
        public void AreEqual()
        {
            var set1 = new SimpleObjectCollection("x", "y", "z");
            var set2 = new SimpleObjectCollection("x", "y", "z");

            CollectionAssert.AreEqual(set1,set2);
            CollectionAssert.AreEqual(set1,set2,new TestComparer());

            Assert.AreEqual(set1,set2);
        }

        [Test]
        public void AreEqualFailCount()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("x", "y", "z", "a");

            var expectedMessage =
                "  Expected is <NUnit.TestUtilities.Collections.SimpleObjectList> with 3 elements, actual is <NUnit.TestUtilities.Collections.SimpleObjectList> with 4 elements" + Environment.NewLine +
                "  Values differ at index [3]" + Environment.NewLine +
                "  Extra:    < \"a\" >";

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreEqual(set1, set2, new TestComparer()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreEqualFail()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("x", "y", "a");

            var expectedMessage =
                "  Expected and actual are both <NUnit.TestUtilities.Collections.SimpleObjectList> with 3 elements" + Environment.NewLine +
                "  Values differ at index [2]" + Environment.NewLine +
                "  String lengths are both 1. Strings differ at index 0." + Environment.NewLine +
                "  Expected: \"z\"" + Environment.NewLine +
                "  But was:  \"a\"" + Environment.NewLine +
                "  -----------^" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreEqual(set1,set2,new TestComparer()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreEqual_HandlesNull()
        {
            object[] set1 = new object[3];
            object[] set2 = new object[3];

            CollectionAssert.AreEqual(set1,set2);
            CollectionAssert.AreEqual(set1,set2,new TestComparer());
        }

        [Test]
        public void EnsureComparerIsUsed()
        {
            // Create two collections
            int[] array1 = new int[2];
            int[] array2 = new int[2];

            array1[0] = 4;
            array1[1] = 5;

            array2[0] = 99;
            array2[1] = -99;

            CollectionAssert.AreEqual(array1, array2, new AlwaysEqualComparer());
        }

        [Test]
        public void AreEqual_UsingIterator()
        {
            int[] array = new int[] { 1, 2, 3 };

            CollectionAssert.AreEqual(array, CountToThree());
        }

        IEnumerable CountToThree()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        [Test]
        public void AreEqual_UsingIterator_Fails()
        {
            int[] array = new int[] { 1, 3, 5 };
 
            AssertionException ex = Assert.Throws<AssertionException>( 
                delegate { CollectionAssert.AreEqual(array, CountToThree()); } );
            
            Assert.That(ex.Message, Does.Contain("Values differ at index [1]").And.
                                    Contains("Expected: 3").And.
                                    Contains("But was:  2"));
        }
 
#if NET_3_5 || NET_4_0 || SILVERLIGHT || NETCF_3_5 || PORTABLE
        [Test]
        public void AreEqual_UsingLinqQuery()
        {
            int[] array = new int[] { 1, 2, 3 };
 
            CollectionAssert.AreEqual(array, array.Select((item) => item));
        }
 
        [Test]
        public void AreEqual_UsingLinqQuery_Fails()
        {
            int[] array = new int[] { 1, 2, 3 };
 
            AssertionException ex = Assert.Throws<AssertionException>(
                delegate { CollectionAssert.AreEqual(array, array.Select((item) => item * 2)); } );
            
            Assert.That(ex.Message, Does.Contain("Values differ at index [0]").And.
                                    Contains("Expected: 1").And.
                                    Contains("But was:  2"));
        }
#endif
        
        #endregion

        #region AreEquivalent

        [Test]
        public void Equivalent()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("z", "y", "x");

            CollectionAssert.AreEquivalent(set1,set2);
        }

        [Test]
        public void EquivalentFailOne()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "z");
            ICollection set2 = new SimpleObjectCollection("x", "y", "x");

            var expectedMessage =
                "  Expected: equivalent to < \"x\", \"y\", \"z\" >" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"x\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreEquivalent(set1,set2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EquivalentFailTwo()
        {
            ICollection set1 = new SimpleObjectCollection("x", "y", "x");
            ICollection set2 = new SimpleObjectCollection("x", "y", "z");
            
            var expectedMessage =
                "  Expected: equivalent to < \"x\", \"y\", \"x\" >" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreEquivalent(set1,set2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreEquivalentHandlesNull()
        {
            ICollection set1 = new SimpleObjectCollection(null, "x", null, "z");
            ICollection set2 = new SimpleObjectCollection("z", null, "x", null);
            
            CollectionAssert.AreEquivalent(set1,set2);
        }
        #endregion

        #region AreNotEqual

        [Test]
        public void AreNotEqual()
        {
            var set1 = new SimpleObjectCollection("x", "y", "z");
            var set2 = new SimpleObjectCollection("x", "y", "x");

            CollectionAssert.AreNotEqual(set1,set2);
            CollectionAssert.AreNotEqual(set1,set2,new TestComparer());
            CollectionAssert.AreNotEqual(set1,set2,"test");
            CollectionAssert.AreNotEqual(set1,set2,new TestComparer(),"test");
            CollectionAssert.AreNotEqual(set1,set2,"test {0}","1");
            CollectionAssert.AreNotEqual(set1,set2,new TestComparer(),"test {0}","1");
        }

        [Test]
        public void AreNotEqual_Fails()
        {
            var set1 = new SimpleObjectCollection("x", "y", "z");
            var set2 = new SimpleObjectCollection("x", "y", "z");

            var expectedMessage = 
                "  Expected: not equal to < \"x\", \"y\", \"z\" >" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreNotEqual(set1, set2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void AreNotEqual_HandlesNull()
        {
            object[] set1 = new object[3];
            var set2 = new SimpleObjectCollection("x", "y", "z");

            CollectionAssert.AreNotEqual(set1,set2);
            CollectionAssert.AreNotEqual(set1,set2,new TestComparer());
        }

        #endregion

        #region AreNotEquivalent

        [Test]
        public void NotEquivalent()
        {
            var set1 = new SimpleObjectCollection("x", "y", "z");
            var set2 = new SimpleObjectCollection("x", "y", "x");

            CollectionAssert.AreNotEquivalent(set1,set2);
        }

        [Test]
        public void NotEquivalent_Fails()
        {
            var set1 = new SimpleObjectCollection("x", "y", "z");
            var set2 = new SimpleObjectCollection("x", "z", "y");

            var expectedMessage =
                "  Expected: not equivalent to < \"x\", \"y\", \"z\" >" + Environment.NewLine +
                "  But was:  < \"x\", \"z\", \"y\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.AreNotEquivalent(set1,set2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotEquivalentHandlesNull()
        {
            var set1 = new SimpleObjectCollection("x", null, "z");
            var set2 = new SimpleObjectCollection("x", null, "x");

            CollectionAssert.AreNotEquivalent(set1,set2);
        }
        #endregion

        #region Contains
        [Test]
        public void Contains_IList()
        {
            var list = new SimpleObjectList("x", "y", "z");
            CollectionAssert.Contains(list, "x");
        }

        [Test]
        public void Contains_ICollection()
        {
            var collection = new SimpleObjectCollection("x", "y", "z");
            CollectionAssert.Contains(collection,"x");
        }

        [Test]
        public void ContainsFails_ILIst()
        {
            var list = new SimpleObjectList("x", "y", "z");

            var expectedMessage =
                "  Expected: collection containing \"a\"" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.Contains(list,"a"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ContainsFails_ICollection()
        {
            var collection = new SimpleObjectCollection("x", "y", "z");

            var expectedMessage =
                "  Expected: collection containing \"a\"" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.Contains(collection,"a"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ContainsFails_EmptyIList()
        {
            var list = new SimpleObjectList();

            var expectedMessage =
                "  Expected: collection containing \"x\"" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.Contains(list,"x"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ContainsFails_EmptyICollection()
        {
            var ca = new SimpleObjectCollection(new object[0]);

            var expectedMessage =
                "  Expected: collection containing \"x\"" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.Contains(ca,"x"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ContainsNull_IList()
        {
            Object[] oa = new object[] { 1, 2, 3, null, 4, 5 };
            CollectionAssert.Contains( oa, null );
        }

        [Test]
        public void ContainsNull_ICollection()
        {
            var ca = new SimpleObjectCollection(new object[] { 1, 2, 3, null, 4, 5 });
            CollectionAssert.Contains( ca, null );
        }
        #endregion

        #region DoesNotContain
        [Test]
        public void DoesNotContain()
        {
            var list = new SimpleObjectList();
            CollectionAssert.DoesNotContain(list,"a");
        }

        [Test]
        public void DoesNotContain_Empty()
        {
            var list = new SimpleObjectList();
            CollectionAssert.DoesNotContain(list,"x");
        }

        [Test]
        public void DoesNotContain_Fails()
        {
            var list = new SimpleObjectList("x", "y", "z");

            var expectedMessage = 
                "  Expected: not collection containing \"y\"" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.DoesNotContain(list,"y"));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        #endregion

        #region IsSubsetOf
        [Test]
        public void IsSubsetOf()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("y", "z");

            CollectionAssert.IsSubsetOf(set2,set1);
            Assert.That(set2, Is.SubsetOf(set1));
        }

        [Test]
        public void IsSubsetOf_Fails()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("y", "z", "a");

            var expectedMessage =
                "  Expected: subset of < \"y\", \"z\", \"a\" >" + Environment.NewLine +
                "  But was:  < \"x\", \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.IsSubsetOf(set1,set2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsSubsetOfHandlesNull()
        {
            var set1 = new SimpleObjectList("x", null, "z");
            var set2 = new SimpleObjectList(null, "z");

            CollectionAssert.IsSubsetOf(set2,set1);
            Assert.That(set2, Is.SubsetOf(set1));
        }
        #endregion

        #region IsNotSubsetOf
        [Test]
        public void IsNotSubsetOf()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("y", "z", "a");

            CollectionAssert.IsNotSubsetOf(set1,set2);
            Assert.That(set1, Is.Not.SubsetOf(set2));
        }

        [Test]
        public void IsNotSubsetOf_Fails()
        {
            var set1 = new SimpleObjectList("x", "y", "z");
            var set2 = new SimpleObjectList("y", "z");

            var expectedMessage =
                "  Expected: not subset of < \"x\", \"y\", \"z\" >" + Environment.NewLine +
                "  But was:  < \"y\", \"z\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.IsNotSubsetOf(set2,set1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        
        [Test]
        public void IsNotSubsetOfHandlesNull()
        {
            var set1 = new SimpleObjectList("x", null, "z");
            var set2 = new SimpleObjectList(null, "z", "a");

            CollectionAssert.IsNotSubsetOf(set1,set2);
        }
        #endregion

        #region IsOrdered

        [Test]
        public void IsOrdered()
        {
            var list = new SimpleObjectList("x", "y", "z");
            CollectionAssert.IsOrdered(list);
        }

        [Test]
        public void IsOrdered_Fails()
        {
            var list = new SimpleObjectList("x", "z", "y");

            var expectedMessage =
                "  Expected: collection ordered" + Environment.NewLine +
                "  But was:  < \"x\", \"z\", \"y\" >" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => CollectionAssert.IsOrdered(list));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsOrdered_Allows_adjacent_equal_values()
        {
            var list = new SimpleObjectList("x", "x", "z");
            CollectionAssert.IsOrdered(list);
        }

        [Test]
        public void IsOrdered_Handles_null()
        {
            var list = new SimpleObjectList("x", null, "z");

            var ex = Assert.Throws<ArgumentNullException>(() => CollectionAssert.IsOrdered(list));
            Assert.That(ex.Message, Does.Contain("index 1"));
        }

        [Test]
        public void IsOrdered_ContainedTypesMustBeCompatible()
        {
            var list = new SimpleObjectList(1, "x");
            Assert.Throws<ArgumentException>(() => CollectionAssert.IsOrdered(list));
        }

        [Test]
        public void IsOrdered_TypesMustImplementIComparable()
        {
            var list = new SimpleObjectList(new object(), new object());
            Assert.Throws<ArgumentException>(() => CollectionAssert.IsOrdered(list));
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison()
        {
            var list = new SimpleObjectList(new object(), new object());
            CollectionAssert.IsOrdered(list, new AlwaysEqualComparer());
        }

        [Test]
        public void IsOrdered_Handles_custom_comparison2()
        {
            var list = new SimpleObjectList(2, 1);
            CollectionAssert.IsOrdered(list, new TestComparer());
        }

        #endregion
    }
}
