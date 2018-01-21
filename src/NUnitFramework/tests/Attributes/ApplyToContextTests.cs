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

using System.Globalization;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class ApplyToContextTests
    {
        private TestExecutionContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = new TestExecutionContext();
        }

        [Test]
        public void SingleThreadedAttribute()
        {
            var attr = new SingleThreadedAttribute();
            attr.ApplyToContext(_context);
            Assert.True(_context.IsSingleThreaded);
        }

#if !NETCOREAPP1_1
        [Test]
        public void SetCultureAttribute()
        {
            var attr = new SetCultureAttribute("fr-FR") as IApplyToContext;
            attr.ApplyToContext(_context);
            Assert.That(_context.CurrentCulture, Is.EqualTo(new CultureInfo("fr-FR")));
        }

        [Test]
        public void SetUICultureAttribute()
        {
            var attr = new SetUICultureAttribute("fr-FR") as IApplyToContext;
            attr.ApplyToContext(_context);
            Assert.That(_context.CurrentUICulture, Is.EqualTo(new CultureInfo("fr-FR")));
        }
#endif

#if THREAD_ABORT
        [Test]
        public void TimeoutAttribute()
        {
            var attr = new TimeoutAttribute(50) as IApplyToContext;
            attr.ApplyToContext(_context);
            Assert.That(_context.TestCaseTimeout, Is.EqualTo(50));
        }
#endif
    }
}
