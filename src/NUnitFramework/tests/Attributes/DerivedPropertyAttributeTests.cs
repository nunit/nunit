// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class DerivedPropertyAttributeTests
    {
        [TestCase(typeof(DescriptionAttribute), PropertyNames.Description, "description")]
        [TestCase(typeof(LevelOfParallelismAttribute), PropertyNames.LevelOfParallelism, 7)]
        [TestCase(typeof(MaxTimeAttribute), PropertyNames.MaxTime, 50)]
        [TestCase(typeof(ParallelizableAttribute), PropertyNames.ParallelScope, ParallelScope.Fixtures)]
        [TestCase(typeof(SetCultureAttribute), PropertyNames.SetCulture, "fr-FR")]
        [TestCase(typeof(SetUICultureAttribute), PropertyNames.SetUICulture, "fr-FR")]
        [TestCase(typeof(ApartmentAttribute), PropertyNames.ApartmentState, ApartmentState.MTA)]
        [TestCase(typeof(ApartmentAttribute), PropertyNames.ApartmentState, ApartmentState.STA)]
#if THREAD_ABORT
        [TestCase(typeof(TimeoutAttribute), PropertyNames.Timeout, 50)]
#endif
        public void ConstructWithOneArg<T>(Type attrType, string propName, T propValue)
        {
            var attr = Reflect.Construct(attrType, new object[] { propValue }) as PropertyAttribute;
            Assert.NotNull(attr, "{0} is not a PropertyAttribute", attrType.Name);
            Assert.That(attr.Properties.Get(propName), Is.EqualTo(propValue));
        }

        [TestCase(typeof(ParallelizableAttribute), PropertyNames.ParallelScope, ParallelScope.Self)]
        [TestCase(typeof(RequiresThreadAttribute), PropertyNames.RequiresThread, true)]
        public void ConstructWithNoArgs<T>(Type attrType, string propName, T propValue)
        {
            var attr = Reflect.Construct(attrType) as PropertyAttribute;
            Assert.NotNull(attr, "{0} is not a PropertyAttribute", attrType.Name);
            Assert.That(attr.Properties.Get(propName), Is.EqualTo(propValue));
        }
    }
}
