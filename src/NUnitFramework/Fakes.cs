// ***********************************************************************
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
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using BF = System.Reflection.BindingFlags;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Fakes provides static methods for creating test dummies of various kinds
    /// </summary>
    public static class Fakes
    {
        #region GetTestMethod

        public static FakeTestMethod GetTestMethod(Type type, string name)
        {
            return new FakeTestMethod(type, name);
        }

        public static FakeTestMethod GetTestMethod(object obj, string name)
        {
            return new FakeTestMethod(obj, name);
        }

        #endregion

        #region GetWorkItem

        public static FakeWorkItem GetWorkItem(Test test, TestExecutionContext context)
        {
            return new FakeWorkItem(test, context);
        }

        public static FakeWorkItem GetWorkItem(Test test)
        {
            return GetWorkItem(test, new TestExecutionContext());
        }

        public static FakeWorkItem GetWorkItem(Type type, string name, TestExecutionContext context)
        {
            return GetWorkItem(GetTestMethod(type, name), context);
        }

        public static FakeWorkItem GetWorkItem(Type type, string name)
        {
            return GetWorkItem(GetTestMethod(type, name));
        }

        public static FakeWorkItem GetWorkItem(object obj, string name, TestExecutionContext context)
        {
            return GetWorkItem(obj.GetType(), name, context);
        }

        public static FakeWorkItem GetWorkItem(object obj, string name)
        {
            return GetWorkItem(obj.GetType(), name);
        }

        #endregion
    }

    #region FakeTestMethod Class

    /// <summary>
    /// FakeTestMethod is used in tests to simulate an actual TestMethod
    /// </summary>
    public class FakeTestMethod : TestMethod
    {
        public FakeTestMethod(object obj, string name)
            : this(obj.GetType(), name) { }

        public FakeTestMethod(Type type, string name)
            : base(new FixtureMethod(type, type.GetMethod(name, BF.Public | BF.NonPublic | BF.Static | BF.Instance))) { }
    }

    #endregion

    #region FakeWorkItem Class

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

    #endregion
}
