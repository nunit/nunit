using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
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
