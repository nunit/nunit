using System;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class TestResultEvaluator : ITestResultEvaluator
    {
        private readonly string _description;

        private readonly Lazy<TestResultDto> lazyValue;

        public TestResultEvaluator([NotNull] Func<TestResultDto> evaluator, [NotNull] string description)
        {
            Contract.Requires<ArgumentNullException>(evaluator != null);
            Contract.Requires<ArgumentNullException>(description != null);

            _description = description;
            lazyValue = new Lazy<TestResultDto>(evaluator);
        }

        public TestResultEvaluator([NotNull] TestResultDto result, [NotNull] string description)
        {
            Contract.Requires<ArgumentNullException>(result != null);
            Contract.Requires<ArgumentNullException>(description != null);

            _description = description;
            lazyValue = new Lazy<TestResultDto>(() => result);
        }

        public TestResultDto Evaluate()
        {
            Contract.Ensures(Contract.Result<TestResultDto>() != null);
            return lazyValue.Value;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return _description;
        }
    }
}
