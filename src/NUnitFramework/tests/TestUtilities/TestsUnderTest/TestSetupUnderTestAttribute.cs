// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class TestSetupUnderTestAttribute : Attribute, IApplyToTest
    {
        public static readonly string Category = "TestsUnderTest";

        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable && test.RunState != RunState.Ignored)
            {
                test.RunState = RunState.Explicit;
                test.Properties.Add(PropertyNames.Category, Category);
            }
        }
    }
}
