using System;

namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected struct ComparisonBehaviorWithAssertionException
        {
            private readonly ComparisonBehavior _comparisonBehavior;

            public ComparisonBehaviorWithAssertionException(ComparisonBehavior comparisonBehavior)
            {
                _comparisonBehavior = comparisonBehavior;
            }

            public ComparisonBehaviorWithAssertionException Verify(Action<object, object> throwsAssertionExceptionForFalse, ComparisonType comparisonType)
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
