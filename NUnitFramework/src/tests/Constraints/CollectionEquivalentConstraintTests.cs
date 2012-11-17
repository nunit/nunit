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
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints.Tests
{
    public class CollectionEquivalentConstraintTests
    {
        [Test]
        public void EqualCollectionsAreEquivalent()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("x", "y", "z");

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void WorksWithCollectionsOfArrays()
        {
            byte[] array1 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array2 = new byte[] { 0x42, 0x52, 0x72, 0xef };
            byte[] array3 = new byte[] { 0x20, 0x44, 0x56, 0x76, 0x1e, 0xff };
            byte[] array4 = new byte[] { 0x42, 0x52, 0x72, 0xef };

            ICollection set1 = new ICollectionAdapter(array1, array2);
            ICollection set2 = new ICollectionAdapter(array3, array4);

            Constraint constraint = new CollectionEquivalentConstraint(set1);
            Assert.That(constraint.ApplyTo(set2).IsSuccess);

            set2 = new ICollectionAdapter(array4, array3);
            Assert.That(constraint.ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentIgnoresOrder()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "y", "x");

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInActual()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("x", "y", "x");

            Assert.False(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentFailsWithDuplicateElementInExpected()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "x");
            ICollection set2 = new ICollectionAdapter("x", "y", "z");

            Assert.False(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHandlesNull()
        {
            ICollection set1 = new ICollectionAdapter(null, "x", null, "z");
            ICollection set2 = new ICollectionAdapter("z", null, "x", null);

            Assert.That(new CollectionEquivalentConstraint(set1).ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHonorsIgnoreCase()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1).IgnoreCase.ApplyTo(set2).IsSuccess);
        }

        [Test]
        public void EquivalentHonorsUsing()
        {
            ICollection set1 = new ICollectionAdapter("x", "y", "z");
            ICollection set2 = new ICollectionAdapter("z", "Y", "X");

            Assert.That(new CollectionEquivalentConstraint(set1)
                .Using<string>((x, y) => String.Compare(x, y, true))
                .ApplyTo(set2).IsSuccess);
        }

#if NET_3_5 || NET_4_0
        [Test, Platform("Net-3.5,Mono-3.5,Net-4.0,Mono-4.0")]
        public void WorksWithHashSets()
        {
            var hash1 = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var hash2 = new HashSet<string>(new string[] { "abracadabra", "presto", "hocuspocus" });

            Assert.That(new CollectionEquivalentConstraint(hash1).ApplyTo(hash2).IsSuccess);
        }

        [Test, Platform("Net-3.5,Mono-3.5,Net-4.0,Mono-4.0")]
        public void WorksWithHashSetAndArray()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocuspocus" };

            var constraint = new CollectionEquivalentConstraint(hash);
            Assert.That(constraint.ApplyTo(array).IsSuccess);
        }

        [Test, Platform("Net-3.5,Mono-3.5,Net-4.0,Mono-4.0")]
        public void WorksWithArrayAndHashSet()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocuspocus" };

            var constraint = new CollectionEquivalentConstraint(array);
            Assert.That(constraint.ApplyTo(hash).IsSuccess);
        }

        [Test, Platform("Net-3.5,Mono-3.5,Net-4.0,Mono-4.0")]
        public void FailureMessageWithHashSetAndArray()
        {
            var hash = new HashSet<string>(new string[] { "presto", "abracadabra", "hocuspocus" });
            var array = new string[] { "abracadabra", "presto", "hocusfocus" };

            var constraint = new CollectionEquivalentConstraint(hash);
            var constraintResult = constraint.ApplyTo(array);
            Assert.False(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Is.EqualTo(
                "  Expected: equivalent to < \"presto\", \"abracadabra\", \"hocuspocus\" >" + Environment.NewLine +
                "  But was:  < \"abracadabra\", \"presto\", \"hocusfocus\" >" + Environment.NewLine));
            Console.WriteLine(writer.ToString());
        }
#endif
    }
}