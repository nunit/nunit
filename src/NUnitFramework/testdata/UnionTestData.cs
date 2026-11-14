// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NET11_0_OR_GREATER

namespace NUnit.TestData
{
    public record DerivedUnionMember() : UnionMember();
    public record UnionMember();

    public readonly union GeneratedUnionType(UnionMember, int);

    [System.Runtime.CompilerServices.Union]
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
}
#endif
