// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Syntax
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

    public class CollectionOrderedTestDescending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<ordered descending>";
            StaticSyntax = Is.Ordered.Descending;
            BuilderSyntax = Builder().Ordered.Descending;
        }
    }

    public class CollectionOrderedTestComparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = ObjectComparer.Default;
            ParseTree = "<ordered NUnit.Framework.Tests.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.Using(comparer);
            BuilderSyntax = Builder().Ordered.Using(comparer);
        }
    }

    public class CollectionOrderedTestComparerDescending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            IComparer comparer = ObjectComparer.Default;
            ParseTree = "<ordered descending NUnit.Framework.Tests.TestUtilities.Comparers.ObjectComparer>";
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

    public class CollectionOrderedByTestDescending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName descending>";
            StaticSyntax = Is.Ordered.By("SomePropertyName").Descending;
            BuilderSyntax = Builder().Ordered.By("SomePropertyName").Descending;
        }
    }

    public class CollectionOrderedByTestComparer : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName NUnit.Framework.Tests.TestUtilities.Comparers.ObjectComparer>";
            StaticSyntax = Is.Ordered.By("SomePropertyName").Using(ObjectComparer.Default);
            BuilderSyntax = Builder().Ordered.By("SomePropertyName").Using(ObjectComparer.Default);
        }
    }

    public class CollectionOrderedByTestComparerDescending : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<orderedby SomePropertyName descending NUnit.Framework.Tests.TestUtilities.Comparers.ObjectComparer>";
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
