// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
#if CLR_2_0
using System.Collections.Generic;
#endif

namespace NUnit.Framework.Syntax
{
    public class UniqueTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<uniqueitems>";
            staticSyntax = Is.Unique;
            inheritedSyntax = Helper().Unique;
            builderSyntax = Builder().Unique;
        }
    }

    public class CollectionOrderedTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<ordered>";
            staticSyntax = Is.Ordered;
            inheritedSyntax = Helper().Ordered;
            builderSyntax = Builder().Ordered;
        }
    }

    public class CollectionOrderedTest_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<ordered descending>";
            staticSyntax = Is.Ordered.Descending;
            inheritedSyntax = Helper().Ordered.Descending;
            builderSyntax = Builder().Ordered.Descending;
        }
    }

    public class CollectionOrderedTest_Comparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = Comparer.Default;
            parseTree = "<ordered System.Collections.Comparer>";
            staticSyntax = Is.Ordered.Using(comparer);
            inheritedSyntax = Helper().Ordered.Using(comparer);
            builderSyntax = Builder().Ordered.Using(comparer);
        }
    }

    public class CollectionOrderedTest_Comparer_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = Comparer.Default;
            parseTree = "<ordered descending System.Collections.Comparer>";
            staticSyntax = Is.Ordered.Using(comparer).Descending;
            inheritedSyntax = Helper().Ordered.Using(comparer).Descending;
            builderSyntax = Builder().Ordered.Using(comparer).Descending;
        }
    }

    public class CollectionOrderedByTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<orderedby SomePropertyName>";
            staticSyntax = Is.Ordered.By("SomePropertyName");
            inheritedSyntax = Helper().Ordered.By("SomePropertyName");
            builderSyntax = Builder().Ordered.By("SomePropertyName");
        }
    }

    public class CollectionOrderedByTest_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<orderedby SomePropertyName descending>";
            staticSyntax = Is.Ordered.By("SomePropertyName").Descending;
            inheritedSyntax = Helper().Ordered.By("SomePropertyName").Descending;
            builderSyntax = Builder().Ordered.By("SomePropertyName").Descending;
        }
    }

    public class CollectionOrderedByTest_Comparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<orderedby SomePropertyName System.Collections.Comparer>";
            staticSyntax = Is.Ordered.By("SomePropertyName").Using(Comparer.Default);
            inheritedSyntax = Helper().Ordered.By("SomePropertyName").Using(Comparer.Default);
            builderSyntax = Builder().Ordered.By("SomePropertyName").Using(Comparer.Default);
        }
    }

    public class CollectionOrderedByTest_Comparer_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<orderedby SomePropertyName descending System.Collections.Comparer>";
            staticSyntax = Is.Ordered.By("SomePropertyName").Using(Comparer.Default).Descending;
            inheritedSyntax = Helper().Ordered.By("SomePropertyName").Using(Comparer.Default).Descending;
            builderSyntax = Builder().Ordered.By("SomePropertyName").Using(Comparer.Default).Descending;
        }
    }

    public class CollectionContainsTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<contains 42>";
            staticSyntax = Has.Member(42);
            inheritedSyntax = Helper().Contains(42);
            builderSyntax = Builder().Contains(42);
        }
    }

    public class CollectionSubsetTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            int[] ints = new int[] { 1, 2, 3 };
            parseTree = "<subsetof System.Int32[]>";
            staticSyntax = Is.SubsetOf(ints);
            inheritedSyntax = Helper().SubsetOf(ints);
            builderSyntax = Builder().SubsetOf(ints);
        }
    }

    public class CollectionEquivalentTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            int[] ints = new int[] { 1, 2, 3 };
            parseTree = "<equivalent System.Int32[]>";
            staticSyntax = Is.EquivalentTo(ints);
            inheritedSyntax = Helper().EquivalentTo(ints);
            builderSyntax = Builder().EquivalentTo(ints);
        }
    }
}
