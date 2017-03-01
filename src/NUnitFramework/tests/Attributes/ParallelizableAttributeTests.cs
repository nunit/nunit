// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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

using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class ParallelizableAttributeTests
    {
        [Test]
        public void NonParallelizableAttributeUsesParallelScopeNone()
        {
            var attr = new NonParallelizableAttribute();
            Assert.That(attr, Is.AssignableTo<ParallelizableAttribute>());
            Assert.That(attr.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.None));
        }

        [Test]
        public void DefaultConstructorUsesParallelScopeSelf()
        {
            var attr = new ParallelizableAttribute();
            Assert.That(attr.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.Self));
        }

        [TestCaseSource(nameof(Scopes))]
        public void ConstructWithScopeArgument(ParallelScope scope)
        {
            var attr = new ParallelizableAttribute(scope);
            Assert.That(attr.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(scope));
        }

        [TestCaseSource(nameof(Scopes))]
        public void ApplyScopeToTest(ParallelScope scope)
        {
            var test = new TestDummy();
            var attr = new ParallelizableAttribute(scope);
            attr.ApplyToTest(test);
            Assert.That(test.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(scope));
        }

        [TestCaseSource(nameof(Scopes))]
        public void ApplyScopeToContext(ParallelScope scope)
        {
            var context = new TestExecutionContext();
            var attr = new ParallelizableAttribute(scope);
            attr.ApplyToContext(context);
            Assert.That(context.ParallelScope, Is.EqualTo(scope & ParallelScope.ContextMask));
        }

        static ParallelScope[] Scopes = new ParallelScope[]
        {
            ParallelScope.None,
            ParallelScope.Self,
            ParallelScope.Fixtures,
            ParallelScope.Children,
            ParallelScope.All,
            ParallelScope.Self | ParallelScope.Children,
            ParallelScope.Self | ParallelScope.Fixtures
        };
    }
}
