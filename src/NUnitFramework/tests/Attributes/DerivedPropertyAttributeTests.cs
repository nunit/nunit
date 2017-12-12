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
#if !NETCOREAPP1_1
        [TestCase(typeof(SetCultureAttribute), PropertyNames.SetCulture, "fr-FR")]
        [TestCase(typeof(SetUICultureAttribute), PropertyNames.SetUICulture, "fr-FR")]
#endif
#if APARTMENT_STATE
        [TestCase(typeof(ApartmentAttribute), PropertyNames.ApartmentState, ApartmentState.MTA)]
        [TestCase(typeof(ApartmentAttribute), PropertyNames.ApartmentState, ApartmentState.STA)]
#endif
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
#if !NETCOREAPP1_1
        [TestCase(typeof(RequiresThreadAttribute), PropertyNames.RequiresThread, true)]
#endif
        public void ConstructWithNoArgs<T>(Type attrType, string propName, T propValue)
        {
            var attr = Reflect.Construct(attrType) as PropertyAttribute;
            Assert.NotNull(attr, "{0} is not a PropertyAttribute", attrType.Name);
            Assert.That(attr.Properties.Get(propName), Is.EqualTo(propValue));
        }
    }
}
