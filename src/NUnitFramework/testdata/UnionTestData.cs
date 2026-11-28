// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class UnionTestData
    {
        private const int FortyTwo = 42;
        private static readonly UnionMember Member = new();
        private static readonly DerivedUnionMember DerivedMember = new();

        public static readonly TestCaseData[] DefaultValueTestCases =
        [
            new TestCaseData<GeneratedUnionType>(default),
            new TestCaseData<ExplicitIUnionType>(new ExplicitIUnionType(default(UnionMember))),
        ];

        public static readonly TestCaseData[] InitializedWithUnionMemberTestCases =
        [
            new TestCaseData<GeneratedUnionType>(new GeneratedUnionType(Member)),
            new TestCaseData<CustomUnionType>(new CustomUnionType(Member)),
            new TestCaseData<ExplicitIUnionType>(new ExplicitIUnionType(Member)),
        ];

        public static readonly TestCaseData[] InitializedWithDerivedUnionMemberTestCases =
        [
            new TestCaseData<GeneratedUnionType>(new GeneratedUnionType(DerivedMember)),
            new TestCaseData<CustomUnionType>(new CustomUnionType(DerivedMember)),
            new TestCaseData<ExplicitIUnionType>(new ExplicitIUnionType(DerivedMember)),
        ];

        public static readonly TestCaseData[] InitializedWithInt32TestCases =
        [
            new TestCaseData<GeneratedUnionType>(new GeneratedUnionType(FortyTwo)),
            new TestCaseData<CustomUnionType>(new CustomUnionType(FortyTwo)),
            new TestCaseData<ExplicitIUnionType>(new ExplicitIUnionType(FortyTwo)),
        ];

        public static IEnumerable<TestCaseData> InitializedValueTestCases =>
            InitializedWithUnionMemberTestCases
                .Concat(InitializedWithDerivedUnionMemberTestCases)
                .Concat(InitializedWithInt32TestCases);
    }

    public record DerivedUnionMember() : UnionMember();
    public record UnionMember();

    public readonly union GeneratedUnionType(UnionMember, int)
    {
        public override string ToString() => $"GeneratedUnionType({Value})";
    }

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

        public override string ToString() => $"CustomUnionType({Value})";
    }

    [Union]
    public class ExplicitIUnionType : IUnion
    {
        private readonly object? _value;

        public ExplicitIUnionType(UnionMember? value)
        {
            _value = value;
        }
        public ExplicitIUnionType(int value)
        {
            _value = value;
        }

        public object? Value => _value;

        public override string ToString() => $"ExplicitIUnionType({Value})";
    }
}
