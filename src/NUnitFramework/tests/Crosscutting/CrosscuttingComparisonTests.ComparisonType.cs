namespace NUnit.Framework.Crosscutting
{
    public abstract partial class CrosscuttingComparisonTests
    {
        protected enum ComparisonType
        {
            Equal,
            Unequal,
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual
        }
    }
}
