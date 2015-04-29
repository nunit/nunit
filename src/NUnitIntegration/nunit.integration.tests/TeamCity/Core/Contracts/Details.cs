// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class Details
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

        [NotNull]
        public Details Combine([NotNull] Details otherDetails, string prefix = "")
        {
            Contract.Requires<ArgumentNullException>(otherDetails != null);
            Contract.Ensures(Contract.Result<Details>() != null);

            return new Details(_details.Concat(otherDetails._details.Select(i => prefix + i)));
        }

        [NotNull]
        public Details Combine([NotNull] IEnumerable<string> detailLines, string prefix = "")
        {
            Contract.Requires<ArgumentNullException>(detailLines != null);
            Contract.Ensures(Contract.Result<Details>() != null);

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
