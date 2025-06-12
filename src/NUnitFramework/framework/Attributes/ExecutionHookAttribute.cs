// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// Attribute to activate execution hooks.
    /// </summary>
    public abstract class ExecutionHookAttribute : NUnitAttribute, IWrapTestMethod
    {
        /// <inheritdoc />
        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }
}
