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

//[assembly: SampleAction("AssemblySuite", ActionTargets.Suite)]
//[assembly: SampleAction("AssemblyTest", ActionTargets.Test)]
//[assembly: SampleAction("AssemblySite")]

namespace NUnit.TestData.ActionAttributeTests
{
    //[SetUpFixture]
    //[SampleAction("SetupFixtureSuite", ActionTargets.Suite)]
    //[SampleAction("SetupFixtureTest", ActionTargets.Test)]
    //[SampleAction("SetupFixtureSite")]
    public class SetupFixture : BaseSetupFixture
    {
    }

    //[SampleAction("BaseSetupFixtureSuite", ActionTargets.Suite)]
    //[SampleAction("BaseSetupFixtureTest", ActionTargets.Test)]
    //[SampleAction("BaseSetupFixtureSite")]
    public abstract class BaseSetupFixture
    {
    }

    [TestFixture]
    //[SampleAction("FixtureSuite", ActionTargets.Suite)]
    //[SampleAction("FixtureTest", ActionTargets.Test)]
    //[SampleAction("FixtureSite")]
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
        //[SampleAction("ParameterizedSuite", ActionTargets.Suite)] // Applies to parameterized suite
        [NamedAction("ParameterizedTest", ActionTargets.Test)] // Applies to each case
        [NamedAction("ParameterizedSite")]                     // Ditto
        public void ParameterizedTest(string message)
        {
            ((IWithAction)this).Events.Add("Case" + message);
        }

        [Test]
        [NamedAction("MethodSuite", ActionTargets.Suite)] // Ignored in this context
        [NamedAction("MethodTest", ActionTargets.Test)]
        [NamedAction("MethodSite")]
        public void SimpleTest()
        {
            ((IWithAction)this).Events.Add("SimpleTest");
        }
    }

    //[SampleAction("BaseFixtureSuite", ActionTargets.Suite)]
    //[SampleAction("BaseFixtureTest", ActionTargets.Test)]
    //[SampleAction("BaseFixtureSite")]
    public abstract class BaseActionAttributeFixture : IBaseWithAction
    {
    }

    //[SampleAction("InterfaceSuite", ActionTargets.Suite)]
    //[SampleAction("InterfaceTest", ActionTargets.Test)]
    //[SampleAction("InterfaceSite")]
    public interface IWithAction
    {
        List<string> Events { get; }
    }

    //[SampleAction("BaseInterfaceSuite", ActionTargets.Suite)]
    //[SampleAction("BaseInterfaceTest", ActionTargets.Test)]
    //[SampleAction("BaseInterfaceSite")]
    public interface IBaseWithAction
    {
    }

    public class NamedActionAttribute : TestActionAttribute
    {
        private readonly string _name = null;
        private readonly ActionTargets _targets = ActionTargets.Default;

        public NamedActionAttribute(string name)
        {
            _name = name;
        }

        public NamedActionAttribute(string name, ActionTargets targets)
        {
            _name = name;
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
            string message = string.Format("{0} Action={1} Phase={2} Target={3}", test.Name, _name, phase, _targets);

            if(ActionAttributeFixture.Events != null)
                ActionAttributeFixture.Events.Add(message);
        }
    }
}
