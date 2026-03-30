// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.TestUtilities
{
    public class ObservablePlatformAttribute : PlatformAttribute, IApplyToTest
    {
        public const string PlatformIncludesPropertyName = "PlatformIncludes";
        public const string PlatformExcludesPropertyName = "PlatformExcludes";

        public ObservablePlatformAttribute() : base()
        {
        }

        public new void ApplyToTest(Test test)
        {
            if (Includes.Length > 0)
            {
                test.Properties.Set(PlatformIncludesPropertyName, Includes);
            }

            if (Excludes.Length > 0)
            {
                test.Properties.Set(PlatformExcludesPropertyName, Excludes);
            }

            base.ApplyToTest(test);
        }
    }
}
