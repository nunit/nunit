// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

        [TestCaseSourceAttribute(nameof(Scopes))]
        public void ApplyScopeToTestFixture(ParallelScope scope)
        {
            var fixture = new TestFixture(new TypeWrapper(typeof(FixtureClass)));
            var attr = new ParallelizableAttribute(scope);
            attr.ApplyToTest(fixture);
            if (scope == ParallelScope.Fixtures)
                scope |= ParallelScope.Self;
            Assert.That(fixture.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(scope));
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

        [Test]
        public void MayNotCombineParallelScopeSelfAndParallelScopeNone()
        {
            var fixture = new TestFixture(new TypeWrapper(typeof(FixtureClass)));
            var attr = new ParallelizableAttribute(ParallelScope.Self | ParallelScope.None);
            attr.ApplyToTest(fixture);
            Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void MayNotUseParallelScopeFixturesOnTestMethod()
        {
            var test = TestBuilder.MakeTestCase(GetType(), nameof(DummyMethod));
            var attr = new ParallelizableAttribute(ParallelScope.Fixtures);
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void MayNotUseParallelScopeFixturesOnParameterizedTestMethod()
        {
            var test = TestBuilder.MakeParameterizedMethodSuite(GetType(), nameof(DummyTestCase));
            var attr = new ParallelizableAttribute(ParallelScope.Fixtures);
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void MayNotUseParallelScopeChildrenOnTestMethod()
        {
            var test = TestBuilder.MakeTestCase(GetType(), nameof(DummyMethod));
            var attr = new ParallelizableAttribute(ParallelScope.Children);
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void OkToUseParallelScopeChildrenOnParameterizedTestMethod()
        {
            var test = TestBuilder.MakeParameterizedMethodSuite(GetType(), nameof(DummyTestCase));
            var attr = new ParallelizableAttribute(ParallelScope.Children);
            attr.ApplyToTest(test);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        public class FixtureClass { }

        public void DummyMethod() { }

        [TestCase(1)]
        public void DummyTestCase(int i) { }
    }
}
