using System;

namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected struct ComparisonBehaviorIfWithAssertionException<T>
        {
            private readonly ComparisonBehaviorIf<T> _comparisonBehavior;

            public ComparisonBehaviorIfWithAssertionException(ComparisonBehaviorIf<T> comparisonBehavior)
            {
                _comparisonBehavior = comparisonBehavior;
            }

            public ComparisonBehaviorIfWithAssertionException<T> Verify(Action<T, T> throwsAssertionExceptionForFalse, ComparisonType comparisonType)
            {
                Guard.ArgumentNotNull(throwsAssertionExceptionForFalse, nameof(throwsAssertionExceptionForFalse));

                _comparisonBehavior.Verify((x, y) =>
                {
                    try
                    {
                        throwsAssertionExceptionForFalse.Invoke(x, y);
                        return true;
                    }
                    catch (AssertionException)
                    {
                        return false;
                    }
                }, comparisonType);

                return this;
            }
        }
    }
}
