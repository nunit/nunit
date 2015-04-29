using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public class Details
    {
        private readonly List<string> _details = new List<string>();

        public Details([NotNull] params string[] detailLines)
        {
            Contract.Requires<ArgumentNullException>(detailLines != null);

            _details.AddRange(detailLines);
        }

        public Details([NotNull] IEnumerable<string> detailLines)
        {
            Contract.Requires<ArgumentNullException>(detailLines != null);

            _details.AddRange(detailLines);
        }

        public Details Combine([NotNull] Details otherDetails, string prefix = "")
        {
            Contract.Requires<ArgumentNullException>(otherDetails != null);

            return new Details(_details.Concat(otherDetails._details.Select(i => prefix + i)));
        }

        public Details Combine([NotNull] IEnumerable<string> detailLines, string prefix = "")
        {
            Contract.Requires<ArgumentNullException>(detailLines != null);

            return new Details(_details.Concat(detailLines.Select(i => prefix + i)));
        }

        public void Add([NotNull] string detailLine)
        {
            Contract.Requires<ArgumentNullException>(detailLine != null);

            _details.Add(detailLine);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var detail in _details)
            {
                sb.AppendLine(detail);
            }

            return sb.ToString();
        }
    }
}
