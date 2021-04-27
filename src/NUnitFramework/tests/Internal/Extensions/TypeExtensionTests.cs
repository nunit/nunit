// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;

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
