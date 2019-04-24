// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Syntax
{
    public class UniqueTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<uniqueitems>";
            StaticSyntax = Is.Unique;
            BuilderSyntax = Builder().Unique;
        }
    }

    public class CollectionOrderedTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<ordered>";
            StaticSyntax = Is.Ordered;
            BuilderSyntax = Builder().Ordered;
        }
    }

    public class CollectionOrderedTest_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<ordered descending>";
            StaticSyntax = Is.Ordered.Descending;
            BuilderSyntax = Builder().Ordered.Descending;
        }
    }

    public class CollectionOrderedTest_Comparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = ObjectComparer.Default;
            ParseTree = "<ordered NUnit.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.Using(comparer);
            BuilderSyntax = Builder().Ordered.Using(comparer);
        }
    }

    public class CollectionOrderedTest_Comparer_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = ObjectComparer.Default;
            ParseTree = "<ordered descending NUnit.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.Using(comparer).Descending;
            BuilderSyntax = Builder().Ordered.Using(comparer).Descending;
        }
    }

    public class CollectionOrderedByTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName>";
            StaticSyntax = Is.Ordered.By("SomePropertyName");
            BuilderSyntax = Builder().Ordered.By("SomePropertyName");
        }
    }

    public class CollectionOrderedByTest_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName descending>";
            StaticSyntax = Is.Ordered.By("SomePropertyName").Descending;
            BuilderSyntax = Builder().Ordered.By("SomePropertyName").Descending;
        }
    }

    public class CollectionOrderedByTest_Comparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName NUnit.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.By("SomePropertyName").Using(ObjectComparer.Default);
            BuilderSyntax = Builder().Ordered.By("SomePropertyName").Using(ObjectComparer.Default);
        }
    }

    public class CollectionOrderedByTest_Comparer_Descending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName descending NUnit.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.By("SomePropertyName").Using(ObjectComparer.Default).Descending;
            BuilderSyntax = Builder().Ordered.By("SomePropertyName").Using(ObjectComparer.Default).Descending;
        }
    }

    public class CollectionContainsTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<some <equal 42>>";
            StaticSyntax = Has.Member(42);
            BuilderSyntax = Builder().Contains(42);
        }
    }

    public class CollectionSubsetTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            int[] ints = new int[] { 1, 2, 3 };
            ParseTree = "<subsetof System.Int32[]>";
            StaticSyntax = Is.SubsetOf(ints);
            BuilderSyntax = Builder().SubsetOf(ints);
        }
    }

    public class CollectionEquivalentTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            int[] ints = new int[] { 1, 2, 3 };
            ParseTree = "<equivalent System.Int32[]>";
            StaticSyntax = Is.EquivalentTo(ints);
            BuilderSyntax = Builder().EquivalentTo(ints);
        }
    }
}
