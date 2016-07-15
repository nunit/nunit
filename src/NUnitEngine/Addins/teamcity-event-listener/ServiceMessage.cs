// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Engine.Listeners
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal struct ServiceMessage
    {
        public ServiceMessage(string name, params ServiceMessageAttribute[] attributes)
        {
            // ReSharper disable once UseNameofExpression
            if (name == null) throw new ArgumentNullException("name");
            // ReSharper disable once UseNameofExpression
            if (attributes == null) throw new ArgumentNullException("attributes");

            Name = name;
            Attributes = new ReadOnlyCollection<ServiceMessageAttribute>(attributes);
        }

        public string Name { get; private set; }

        public IEnumerable<ServiceMessageAttribute> Attributes { get; private set; }

        public static class Names
        {
            public const string TestStdOut = "testStdOut";
            public const string TestSuiteStarted = "testSuiteStarted";
            public const string TestSuiteFinished = "testSuiteFinished";
            public const string FlowStarted = "flowStarted";
            public const string FlowFinished = "flowFinished";
            public const string TestStarted = "testStarted";
            public const string TestFinished = "testFinished";
            public const string TestFailed = "testFailed";
            public const string TestIgnored = "testIgnored";
        }
    }    
}
