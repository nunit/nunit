using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal sealed class ValidationResult
    {
        public ValidationResult(ValidationState state, [NotNull] params string[] details)
        {
            Contract.Requires<ArgumentNullException>(details != null);

            State = state;
            Details = details.ToList();
        }

        public ValidationState State { get; private set; }

        public IEnumerable<string> Details { get; private set; }
    }
}
