// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// TestCaseOutputWriter lists test cases
    /// </summary>
    public class TestCaseOutputWriter : OutputWriter
    {
        /// <summary>
        /// Write a list of test cases to a file
        /// </summary>
        /// <param name="test"></param>
        /// <param name="writer"></param>
        public override void WriteTestFile(ITest test, TextWriter writer)
        {
            if (test.IsSuite)
                foreach (var child in test.Tests)
                    WriteTestFile(child, writer);
            else
                writer.WriteLine(test.FullName);
        }

        /// <summary>
        /// Write a list of test cases to a file
        /// </summary>
        /// <param name="result"></param>
        /// <param name="writer"></param>
        /// <param name="runSettings"></param>
        /// <param name="filter"></param>
        public override void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter)
        {
            WriteTestFile(result.Test, writer);
        }
    }
}
