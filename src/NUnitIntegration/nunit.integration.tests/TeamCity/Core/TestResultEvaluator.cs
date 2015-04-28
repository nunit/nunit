using System;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class TestResultEvaluator : ITestResultEvaluator
    {
        private readonly string _description;
        private Func<TestResultDto> _evaluator;

        public TestResultEvaluator([NotNull] Func<TestResultDto> evaluator, [NotNull] string description)
        {
            Contract.Requires<ArgumentNullException>(evaluator != null);
            Contract.Requires<ArgumentNullException>(description != null);

            _description = description;
            _evaluator = evaluator;
        }

        public TestResultEvaluator([NotNull] TestResultDto result, [NotNull] string description)
        {
            Contract.Requires<ArgumentNullException>(result != null);
            Contract.Requires<ArgumentNullException>(description != null);

            _description = description;
            _evaluator = () => result;
        }

        public TestResultDto Evaluate()
        {
            Contract.Ensures(Contract.Result<TestResultDto>() != null);
            return _evaluator();
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
