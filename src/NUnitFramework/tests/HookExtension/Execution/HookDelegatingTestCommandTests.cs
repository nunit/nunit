// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.HookExtension.Execution
{
    [TestFixture]
    internal class HookDelegatingTestCommandTests
    {
        [TestFixture]
        private class SomeEmptyTest
        {
            [Test]
            public void EmptyTest() { }
        }

        [Test]
        public void InnerCommandIsAlwaysExecuted()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SomeEmptyTest), nameof(SomeEmptyTest.EmptyTest));
            var work = TestBuilder.CreateWorkItem(test) as SimpleWorkItem;

            Assert.That(work, Is.Not.Null);
            TestCommand command = work.MakeTestCommand();

            Assert.That(command, Is.TypeOf(typeof(HookDelegatingTestCommand)));
            HookDelegatingTestCommand hookDelegatingTestCommandCommand = (HookDelegatingTestCommand)command;

            command = GetInnerCommand(hookDelegatingTestCommandCommand);

            Assert.That(command, Is.TypeOf(typeof(TestMethodCommand)));
        }

        private TestCommand GetInnerCommand(DelegatingTestCommand command)
        {
            FieldInfo? innerCommand =
                command.GetType().GetField("innerCommand", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(innerCommand, Is.Not.Null);
            return (TestCommand)innerCommand.GetValue(command)!;
        }
    }
}
