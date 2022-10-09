// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Globalization;
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
