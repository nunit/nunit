// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test that must run on a separate thread.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited=true)]
    public class RequiresThreadAttribute : PropertyAttribute, IApplyToTest
    {
        /// <summary>
        /// Construct a RequiresThreadAttribute
        /// </summary>
        public RequiresThreadAttribute()
            : base(true) { }

        /// <summary>
        /// Construct a RequiresThreadAttribute, specifying the apartment
        /// </summary>
        public RequiresThreadAttribute(ApartmentState apartment)
            : base(true)
        {
            Guard.ArgumentValid(apartment != ApartmentState.Unknown, "must be STA or MTA", nameof(apartment));

            Properties.Add(PropertyNames.ApartmentState, apartment);
        }

        void IApplyToTest.ApplyToTest(Test test)
        {
            test.RequiresThread = true;
            base.ApplyToTest(test);
        }
    }
}
