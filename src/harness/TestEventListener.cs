// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.IO;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.TestHarness
{
    class TestEventListener : MarshalByRefObject, ITestListener
    {
        CommandLineOptions options;
        TextWriter outWriter;

        int level = 0;
        string prefix = "";

        public TestEventListener(CommandLineOptions options, TextWriter outWriter)
        {
            this.options = options;
            this.outWriter = outWriter; 
        }

        #region ITestListener Members

        public void TestStarted(ITest test)
        {
            level++;
            prefix = new string('>', level);
            if(options.DisplayTestLabels == "On" || options.DisplayTestLabels == "All")
                outWriter.WriteLine("{0} {1}", prefix, test.Name);
        }

        public void TestFinished(ITestResult result)
        {
            level--;
            prefix = new string('>', level);
        }

        public void TestOutput(TestOutput testOutput)
        {
            outWriter.Write(testOutput.Text);
        }

        #endregion
    }
}
