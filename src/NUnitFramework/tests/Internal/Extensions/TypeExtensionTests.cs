// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Tests.Internal.Extensions
{
    public class TypeExtensionTests
    {
        [TestCaseSource(nameof(WrapTestCaseForGenericMethod), new object[] { nameof(TypesThatImplementIComparable) })]
        public void TypesThatImplementIComparable_ReturnTrue<T>()
        {
            Assert.That(TypeExtensions.ImplementsIComparable<T>(), Is.True);
            Assert.That(typeof(T).ImplementsIComparable(), Is.True);
        }

        [TestCaseSource(nameof(WrapTestCaseForGenericMethod), new object[] { nameof(TypesThatDontImplementIComparable) })]
        public void TypesThatDontImplementIComparable_ReturnFalse<T>()
        {
            Assert.That(TypeExtensions.ImplementsIComparable<T>(), Is.False);
            Assert.That(typeof(T).ImplementsIComparable(), Is.False);
        }

        [TestCaseSource(nameof(WrapTestCaseForGenericMethod), new object[] { nameof(TypesThatAreSortable) })]
        public void TypesThatAreSortable_ReturnTrue<T>()
        {
            Assert.That(TypeExtensions.IsSortable<T>(), Is.True);
            Assert.That(typeof(T).IsSortable(), Is.True);
        }

        [TestCaseSource(nameof(WrapTestCaseForGenericMethod), new object[] { nameof(TypesThatAreNotSortable) })]
        public void TypesThatAreNotSortable_ReturnFalse<T>()
        {
            Assert.That(TypeExtensions.IsSortable<T>(), Is.False);
            Assert.That(typeof(T).IsSortable(), Is.False);
        }

        private static TestCaseData[] WrapTestCaseForGenericMethod(string sourceName)
        {
            var types = typeof(TypeExtensionTests).GetProperty(sourceName)?.GetValue(null) as IEnumerable<Type>;

            if (types is null)
            {
                throw new ArgumentException($"The property '{sourceName}' was not found or did not return an IEnumerable<Type>.", nameof(sourceName));
            }

            return types.Select(type => new TestCaseData() { TypeArgs = [type] }).ToArray();
        }

        public static IEnumerable<Type> TypesThatAreNotSortable => TypesThatDontImplementIComparable.Union(new[]
        {
            typeof(Tuple<int, Stream>),
            typeof(Tuple<int, long, Stream>),
            typeof(Tuple<int, Tuple<int, Stream>>),
            typeof(ValueTuple<int, Stream>),
            typeof(ValueTuple<int, long, Stream>),
            typeof(ValueTuple<int, ValueTuple<int, Stream>>)
        });

        public static IEnumerable<Type> TypesThatAreSortable => TypesThatImplementIComparable.Union(new[]
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
            typeof(object),
            typeof(Exception),
            typeof(ValueType),
            typeof(Stream)
        };
    }
}
