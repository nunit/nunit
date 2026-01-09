// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class NoTestsAttribute : PropertyAttribute, IApplyToTestSuite
    {
        private readonly TestStatus _defaultStatus;

        public NoTestsAttribute(TestStatus defaultStatus)
        {
            _defaultStatus = defaultStatus;
            Properties.Add(PropertyNames.NoTestsStatus, defaultStatus);
        }

        public void ApplyToTestSuite(TestSuite testSuite)
        {
        }

        //public void ApplyToContext(TestExecutionContext context)
        //{
        //    context.DefaultNoTestsStatus = _defaultStatus;
        //}
    }
}
