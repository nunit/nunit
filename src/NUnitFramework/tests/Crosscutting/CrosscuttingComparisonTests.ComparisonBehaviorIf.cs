using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected struct ComparisonBehaviorIf<T>
        {
            private readonly T _first;
            private readonly T _second;
            private readonly ValueRelationship? relationship;

            public ComparisonBehaviorIf(object first, object second, ValueRelationship relationship)
            {
                if (first is T && second is T)
                {
                    _first = (T)first;
                    _second = (T)second;
                    this.relationship = relationship;
                }
                else
                {
                    this = default(ComparisonBehaviorIf<T>);
                }
            }

            public ComparisonBehaviorIf<T> Verify(Func<T, T, bool> comparer, ComparisonType comparisonType)
            {
                Guard.ArgumentNotNull(comparer, nameof(comparer));

                if (relationship != null)
                {
                    bool result, reverse;
                    using (new TestExecutionContext.IsolatedContext())
                    {
                        result = comparer.Invoke(_first, _second);
                        reverse = comparer.Invoke(_second, _first);
                    }

                    switch (comparisonType)
                    {
                        case ComparisonType.Equal:
                            // Comparing eq == eq, greater == lesser, unordered == unordered
                            Assert.That(result == (relationship.Value == ValueRelationship.Equality));
                            // Comparing eq == eq, lesser == greater, unordered == unordered
                            Assert.That(reverse == (relationship.Value == ValueRelationship.Equality));
                            break;
                        case ComparisonType.Unequal:
                            // Comparing eq != eq, greater != lesser, unordered != unordered
                            Assert.That(result != (relationship.Value == ValueRelationship.Equality));
                            // Comparing eq != eq, lesser != greater, unordered != unordered
                            Assert.That(reverse != (relationship.Value == ValueRelationship.Equality));
                            break;
                        case ComparisonType.Less:
                            // Comparing eq < eq, greater < lesser, unordered < unordered
                            Assert.That(!result);
                            // Comparing eq < eq, lesser < greater, unordered < unordered
                            Assert.That(reverse == (relationship.Value == ValueRelationship.FirstIsGreater));
                            break;
                        case ComparisonType.Greater:
                            // Comparing eq > eq, greater > lesser, unordered > unordered
                            Assert.That(result == (relationship.Value == ValueRelationship.FirstIsGreater));
                            // Comparing eq > eq, lesser > greater, unordered > unordered
                            Assert.That(!reverse);
                            break;
                        case ComparisonType.LessOrEqual:
                            // Comparing eq <= eq, greater <= lesser, unordered <= unordered
                            Assert.That(result == (relationship.Value == ValueRelationship.Equality));
                            // Comparing eq <= eq, lesser <= greater, unordered <= unordered
                            Assert.That(reverse == (relationship.Value == ValueRelationship.Equality | relationship.Value == ValueRelationship.FirstIsGreater));
                            break;
                        case ComparisonType.GreaterOrEqual:
                            // Comparing eq >= eq, greater >= lesser, unordered >= unordered
                            Assert.That(result == (relationship.Value == ValueRelationship.Equality | relationship.Value == ValueRelationship.FirstIsGreater));
                            // Comparing eq >= eq, lesser >= greater, unordered >= unordered
                            Assert.That(reverse == (relationship.Value == ValueRelationship.Equality));
                            break;
                    }
                }

                return this;
            }

            public ComparisonBehaviorIfWithAssertionException<T> WithAssertionException
            {
                get { return new ComparisonBehaviorIfWithAssertionException<T>(this); }
            }
        }
    }
}
