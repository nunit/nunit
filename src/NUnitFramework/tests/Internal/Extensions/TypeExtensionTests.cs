// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NUnit.Framework.Internal.Extensions
{
    public class TypeExtensionTests
    {
        [TestCaseSource(nameof(TypesThatImplementIComparable))]
        public void TypesThatImplementIComparable_ReturnTrue(Type type)
        {
            Assert.That(type.ImplementsIComparable(), Is.True);
        }

        [TestCaseSource(nameof(TypesThatDontImplementIComparable))]
        public void TypesThatDontImplementIComparable_ReturnFalse(Type type)
        {
            Assert.That(type.ImplementsIComparable(), Is.False);
        }

        [TestCaseSource(nameof(TypesThatAreSortable))]
        public void TypesThatAreSortable_ReturnTrue(Type type)
        {
            Assert.That(type.IsSortable(), Is.True);
        }

        [TestCaseSource(nameof(TypesThatAreNotSortable))]
        public void TypesThatAreNotSortable_ReturnFalse(Type type)
        {
            Assert.That(type.IsSortable(), Is.False);
        }

        public static IEnumerable<Type> TypesThatAreNotSortable => TypesThatDontImplementIComparable.Union(new Type[]
        {
            typeof(Tuple<int, Stream>),
            typeof(Tuple<int, long, Stream>),
            typeof(Tuple<int, Tuple<int, Stream>>),
            typeof(ValueTuple<int, Stream>),
            typeof(ValueTuple<int, long, Stream>),
            typeof(ValueTuple<int, ValueTuple<int, Stream>>)
        });

        public static IEnumerable<Type> TypesThatAreSortable => TypesThatImplementIComparable.Union(new Type[]
        {
            typeof(Tuple<int, long>),
            typeof(Tuple<int, long, double>),
            typeof(Tuple<int, Tuple<int, long>>),
            typeof(ValueTuple<int, long>),
            typeof(ValueTuple<int, long, double>),
            typeof(ValueTuple<int, ValueTuple<int, long>>)
        });

        public static IEnumerable<Type> TypesThatImplementIComparable => new[]
        {
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(char),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(string),
            typeof(bool),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(BigInteger),
            typeof(ValueTuple),
            typeof(Version),
        };

        public static IEnumerable<Type> TypesThatDontImplementIComparable => new[]
        {
            null,
            typeof(object),
            typeof(Exception),
            typeof(ValueType),
            typeof(Stream)
        };
    }
}
