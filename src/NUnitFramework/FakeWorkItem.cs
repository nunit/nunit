﻿// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Compatibility;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

#if NETSTANDARD1_3
using BF = NUnit.Compatibility.BindingFlags;
#else
using BF = System.Reflection.BindingFlags;
#endif

namespace NUnit.TestUtilities
{
    /// <summary>
    /// FakeWorkItem is used in tests to simulate an actual WorkItem
    /// </summary>
    public class FakeWorkItem : WorkItem
    {
        public event System.EventHandler Executed;

        public FakeWorkItem(Test test)
            : this(test, new TestExecutionContext()) { }

        public FakeWorkItem(Test test, TestExecutionContext context)
            : base(test, TestFilter.Empty)
        {
            InitializeContext(context);
        }

        public override void Execute()
        {
            if (Executed != null)
                Executed(this, System.EventArgs.Empty);
        }

        protected override void PerformWork() { }
    }
}
