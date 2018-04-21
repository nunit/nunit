using System;

namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected struct ComparisonBehavior
        {
            private readonly object _first;
            private readonly object _second;
            private readonly ValueRelationship _relationship;

            public ComparisonBehavior(object first, object second, ValueRelationship relationship)
            {
                _first = first;
                _second = second;
                _relationship = relationship;
            }

            public ComparisonBehavior Verify(Func<object, object, bool> comparer, ComparisonType comparisonType)
            {
                If<object>().Verify(comparer, comparisonType);
                return this;
            }

            public ComparisonBehaviorIf<T> If<T>()
            {
                return new ComparisonBehaviorIf<T>(_first, _second, _relationship);
            }

            public ComparisonBehaviorWithAssertionException WithAssertionException
            {
                get { return new ComparisonBehaviorWithAssertionException(this); }
            }
        }
    }
}
