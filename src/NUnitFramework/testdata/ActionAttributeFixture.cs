// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
        public static List<string> Events { get; private set; }

        List<string> IWithAction.Events { get { return Events; } }

        static ActionAttributeFixture()
        {
            Events = new List<string>();
        }

        public static void ClearResults()
        {
            Events.Clear();
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

        public override ActionTargets Targets
        {
            get { return _targets; }
        }

        private void AddResult(string phase, ITest test)
        {
            string message = string.Format("{0}.{1}.{2}.{3}", test.Name, _tag, phase, _targets);

            if(ActionAttributeFixture.Events != null)
                ActionAttributeFixture.Events.Add(message);
        }
    }
}
