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

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class CollectionContainsConstraintTests
    {
        [Test]
        public void CanTestContentsOfArray()
        {
            object item = "xyz";
            object[] c = new object[] { 123, item, "abc" };
            Assert.That(c, new CollectionContainsConstraint(item));
        }

        [Test]
        public void CanTestContentsOfObjectList()
        {
            object item = "xyz";
            var list = new List<object>();
            list.Add(123);
            list.Add(item);
            list.Add("abc");
            Assert.That(list, new CollectionContainsConstraint(item));
        }

#if !SILVERLIGHT
        [Test]
        public void CanTestContentsOfSortedList()
        {
            object item = "xyz";
            SortedList list = new SortedList();
            list.Add("a", 123);
            list.Add("b", item);
            list.Add("c", "abc");
            Assert.That(list.Values, new CollectionContainsConstraint(item));
            Assert.That(list.Keys, new CollectionContainsConstraint("b"));
        }
#endif

        [Test]
        public void CanTestContentsOfCollectionNotImplementingIList()
        {
            SimpleObjectCollection ints = new SimpleObjectCollection(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.That(ints, new CollectionContainsConstraint(9));
        }

        [Test]
        [TestCaseSource( "IgnoreCaseData" )]
        public void IgnoreCaseIsHonored( object expected, IEnumerable actual )
        {
            var constraint = new CollectionContainsConstraint( expected ).IgnoreCase;
            var constraintResult = constraint.ApplyTo( actual );
            if ( !constraintResult.IsSuccess )
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo( writer );
                Assert.Fail( writer.ToString() );
            }
        }

        private static readonly object[] IgnoreCaseData =
        {
            new object[] {"WORLD", new string[] { "Hello", "World" }},
            new object[] {"z",new SimpleObjectCollection("z", "Y", "X")},
            new object[] {'A', new object[] {'a', 'b', 'c'}},
            new object[] {"a", new object[] {"A", "B", "C"}}
        };

        [Test]
        public void UsingIsHonored()
        {
            Assert.That(new string[] { "Hello", "World" },
                new CollectionContainsConstraint("WORLD").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
        }
    }
}