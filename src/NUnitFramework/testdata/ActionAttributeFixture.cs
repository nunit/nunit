// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.TestData.ActionAttributeTests;

[assembly: TaggedAction("OnAssembly", ActionTargets.Suite | ActionTargets.Test)]
[assembly: TaggedAction("OnAssembly", ActionTargets.Suite)]
[assembly: TaggedAction("OnAssembly", ActionTargets.Test)]
[assembly: TaggedAction("OnAssembly")]

namespace NUnit.TestData.ActionAttributeTests
{
    [SetUpFixture]
    [TaggedAction("OnSetupFixture", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnSetupFixture", ActionTargets.Suite)]
    [TaggedAction("OnSetupFixture", ActionTargets.Test)]
    [TaggedAction("OnSetupFixture")]
    public class SetupFixture : BaseSetupFixture
    {
    }

    [TaggedAction("OnBaseSetupFixture", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnBaseSetupFixture", ActionTargets.Suite)]
    [TaggedAction("OnBaseSetupFixture", ActionTargets.Test)]
    [TaggedAction("OnBaseSetupFixture")]
    public abstract class BaseSetupFixture
    {
    }

    [TestFixture]
    [TaggedAction("OnFixture", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnFixture", ActionTargets.Suite)]
    [TaggedAction("OnFixture", ActionTargets.Test)]
    [TaggedAction("OnFixture")]
    public class ActionAttributeFixture : BaseActionAttributeFixture, IWithAction
    {
        public static List<string> Events { get; }

        List<string> IWithAction.Events => Events;

        static ActionAttributeFixture()
        {
            Events = new List<string>();
        }

        public static void ClearResults()
        {
            Events.Clear();
        }

        [SetUp]
        public void SetUp()
        {
            Events.Add($"{TestContext.CurrentContext.Test.Name}.SetUpTearDown.Before.Test");
        }

        [TearDown]
        public void TearDown()
        {
            Events.Add($"{TestContext.CurrentContext.Test.Name}.SetUpTearDown.After.Test");
        }

        [TestCase("One", TestName="CaseOne")]
        [TestCase("Two", TestName="CaseTwo")]
        [TaggedAction("OnMethod", ActionTargets.Suite | ActionTargets.Test)] // Applies to both suite and test
        [TaggedAction("OnMethod", ActionTargets.Suite)] // Applies to parameterized suite
        [TaggedAction("OnMethod", ActionTargets.Test)] // Applies to each case
        [TaggedAction("OnMethod")]                     // Ditto
        public void ParameterizedTest(string message)
        {
            ((IWithAction)this).Events.Add("Case" + message);
        }

        [Test]
        [TaggedAction("OnMethod", ActionTargets.Suite | ActionTargets.Test)] // Suite is Ignored on a simple method
        [TaggedAction("OnMethod", ActionTargets.Suite)]
        [TaggedAction("OnMethod", ActionTargets.Test)]
        [TaggedAction("OnMethod")]
        public void SimpleTest()
        {
            ((IWithAction)this).Events.Add("SimpleTest");
        }
    }

    [TaggedAction("OnBaseFixture", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnBaseFixture", ActionTargets.Suite)]
    [TaggedAction("OnBaseFixture", ActionTargets.Test)]
    [TaggedAction("OnBaseFixture")]
    public abstract class BaseActionAttributeFixture : IBaseWithAction
    {
    }

    [TaggedAction("OnInterface", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnInterface", ActionTargets.Suite)]
    [TaggedAction("OnInterface", ActionTargets.Test)]
    [TaggedAction("OnInterface")]
    public interface IWithAction
    {
        List<string> Events { get; }
    }

    [TaggedAction("OnBaseInterface", ActionTargets.Suite | ActionTargets.Test)]
    [TaggedAction("OnBaseInterface", ActionTargets.Suite)]
    [TaggedAction("OnBaseInterface", ActionTargets.Test)]
    [TaggedAction("OnBaseInterface")]
    public interface IBaseWithAction
    {
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class TaggedActionAttribute : TestActionAttribute
    {
        private readonly string _tag = null;
        private readonly ActionTargets _targets = ActionTargets.Default;

        public TaggedActionAttribute(string tag)
        {
            _tag = tag;
        }

        public TaggedActionAttribute(string tag, ActionTargets targets)
        {
            _tag = tag;
            _targets = targets;
        }

        public override void BeforeTest(ITest test)
        {
            AddResult("Before", test);
        }

        public override void AfterTest(ITest test)
        {
            AddResult("After", test);
        }

        public override ActionTargets Targets => _targets;

        private void AddResult(string phase, ITest test)
        {
            string message = $"{test.Name}.{_tag}.{phase}.{_targets}";

            if(ActionAttributeFixture.Events != null)
                ActionAttributeFixture.Events.Add(message);
        }
    }
}
