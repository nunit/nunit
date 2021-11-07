// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;
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
            : base(new MethodWrapper(type, type.GetMethod(name, BF.Public | BF.NonPublic | BF.Static | BF.Instance))) { }
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
