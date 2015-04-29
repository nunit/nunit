using System;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ValidationResult
    {
        public ValidationResult(ValidationState state, [NotNull] Details details)
        {
            Contract.Requires<ArgumentNullException>(details != null);

            State = state;
            Details = details;
        }

        public ValidationState State { get; private set; }

        public Details Details { get; private set; }
    }
}
