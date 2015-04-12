﻿// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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

using System.Collections.Generic;
using System.Text;
using NUnit.Engine;

namespace NUnit.ConsoleRunner.Utilities
{
    public class TestFilterBuilder
    {
        public IList<string> Tests { get; set; }
        public IList<string> Include { get; set; }
        public IList<string> Exclude { get; set; }

        public TestFilterBuilder()
        {
            Tests = new List<string>();
            Include = new List<string>();
            Exclude = new List<string>();
        }

        public TestFilter GetFilter()
        {
            var tests = new StringBuilder();
            var include = new StringBuilder();
            var exclude = new StringBuilder();

            if (Tests.Count > 0)
            {
                tests.Append("<tests>");
                foreach (var test in Tests)
                    tests.AppendFormat("<test>{0}</test>", test);
                tests.Append("</tests>");
            }

            if (Include.Count > 0)
            {
                include.Append("<cat>");
                var needComma = false;
                foreach (var category in Include)
                {
                    if (needComma) include.Append(',');
                    include.Append(category);
                    needComma = true;
                }
                include.Append("</cat>");
            }

            if (Exclude.Count > 0)
            {
                exclude.Append("<not><cat>");
                var needComma = false;
                foreach (var category in Exclude)
                {
                    if (needComma) exclude.Append(',');
                    exclude.Append(category);
                    needComma = true;
                }
                exclude.Append("</cat></not>");
            }

            var testFilter = new StringBuilder("<filter>");
            if (tests.Length > 0)
                testFilter.Append(tests);
            if (include.Length > 0)
                testFilter.Append(include);
            if (exclude.Length > 0)
                testFilter.Append(exclude);
            testFilter.Append("</filter>");

            return new TestFilter(testFilter.ToString());
        }
    }
}
