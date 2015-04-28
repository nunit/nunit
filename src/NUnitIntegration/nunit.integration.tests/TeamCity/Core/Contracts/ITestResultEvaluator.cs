using JetBrains.Annotations;

using NUnit.Framework.Internal;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface ITestResultEvaluator
    {
        [NotNull]
        TestResultDto Evaluate();
    }
}