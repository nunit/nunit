// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtainingn
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
using System.Text;

namespace NUnit.Framework.TestHarness
{
    public class TestFilterBuilder
    {
        public IList<string> Tests { get; set; }
        public IList<string> Include { get; set; }
        public IList<string> Exclude { get; set; }

        public TestFilterBuilder()
        {
            this.Tests = new List<string>();
            this.Include = new List<string>();
            this.Exclude = new List<string>();
        }

        public string GetFilterText()
        {
            StringBuilder sb = new StringBuilder("<filter>");

            if (Tests.Count > 0)
            {
                sb.Append("<tests>");
                foreach (string test in Tests)
                    sb.AppendFormat("<test>{0}</test>", test);
                sb.Append("</tests>");
            }

            if (Include.Count > 0)
            {
                sb.Append("<include>");
                foreach (string category in Include)
                    sb.AppendFormat("<category>{0}</category>", category);
                sb.Append("</include>");
            }

            if (Exclude.Count > 0)
            {
                sb.Append("<exclude>");
                foreach (string category in Exclude)
                    sb.AppendFormat("<category>{0}</category>", category);
                sb.Append("</exclude>");
            }

            sb.Append("</filter>");

            return sb.ToString();
        }

        public static string CreateTestFilter(CommandLineOptions options)
        {
            TestFilterBuilder builder = new TestFilterBuilder();
            foreach (string testName in options.Tests)
                builder.Tests.Add(testName);

            // TODO: Support multiple include / exclude options

            if (options.Include != null)
                builder.Include.Add(options.Include);

            if (options.Exclude != null)
                builder.Exclude.Add(options.Exclude);

            return builder.GetFilterText();
        }
    }
}
