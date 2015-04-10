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

using System.IO;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Drivers
{
    public class NotRunnableFrameworkDriver : IFrameworkDriver
    {
        // TODO: The id should not be hard-coded
        private const string LOAD_RESULT_FORMAT =
            "<test-suite type='Assembly' id='2' name='{0}' fullname='{1}' testcasecount='0' runstate='NotRunnable'>" +
                "<properties>" +
                    "<property name='_SKIPREASON' value='{2}'/>" +
                "</properties>" +
            "</test-suite>";

        private const string RUN_RESULT_FORMAT =
            "<test-suite type='Assembly' id='2' name='{0}' fullname='{1}' testcasecount='0' runstate='NotRunnable' result='Failed' label='Invalid'>" +
                "<properties>" +
                    "<property name='_SKIPREASON' value='{2}'/>" +
                "</properties>" +
                "<reason>" +
                    "<message>{2}</message>" +
                "</reason>" +
            "</test-suite>";

        private string _name;
        private string _fullname;
        private string _message;

        public NotRunnableFrameworkDriver(string assemblyPath, string message)
        {
            _name = Escape(Path.GetFileName(assemblyPath));
            _fullname = Escape(assemblyPath);
            _message = Escape(message);
        }

        public string Load()
        {
            return string.Format(LOAD_RESULT_FORMAT, _name, _fullname, _message);
        }

        public int CountTestCases(TestFilter filter)
        {
            return 0;
        }

        public string Run(ITestEventListener listener, TestFilter filter)
        {
            return string.Format(RUN_RESULT_FORMAT, _name, _fullname, _message);
        }

        public string Explore(TestFilter filter)
        {
            return string.Format(LOAD_RESULT_FORMAT, _name, _fullname, _message);
        }

        public void StopRun(bool force)
        {
        }

        private static string Escape(string original)
        {
            return original
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }
}
