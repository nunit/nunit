// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Runtime.CompilerServices;

namespace NUnit.TestData
{
    public record DerivedUnionMember() : UnionMember();
    public record UnionMember();

    public readonly union GeneratedUnionType(UnionMember, int);

    [Union]
    public class CustomUnionType
    {
        public CustomUnionType(UnionMember value)
        {
            Value = value;
        }
        public CustomUnionType(int value)
        {
            Value = value;
        }
        public object Value { get; }
    }

    [Union]
    public class ExplicitIUnionType : IUnion
    {
        private readonly object _value;

        public ExplicitIUnionType(UnionMember value)
        {
            _value = value;
        }
        public ExplicitIUnionType(int value)
        {
            _value = value;
        }

        public object? Value => _value;
    }
}
