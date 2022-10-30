// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// ICommandWrapper is implemented by attributes and other
    /// objects able to wrap a TestCommand with another command.
    /// </summary>
    /// <remarks>
    /// Attributes or other objects should implement one of the
    /// derived interfaces, rather than this one, since they
    /// indicate in which part of the command chain the wrapper
    /// should be applied.
    /// </remarks>
    public interface ICommandWrapper
    {
        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        TestCommand Wrap(TestCommand command);
    }

    /// <summary>
    /// Objects implementing this interface are used to wrap
    /// the TestMethodCommand itself. They apply after SetUp
    /// has been run and before TearDown.
    /// </summary>
    public interface IWrapTestMethod : ICommandWrapper
    {
    }

    /// <summary>
    /// Objects implementing this interface are used to wrap
    /// the entire test, including SetUp and TearDown.
    /// </summary>
    public interface IWrapSetUpTearDown : ICommandWrapper
    {
    }

    /// <summary>
    /// Objects implementing this interface are used to wrap
    /// tests that can repeat. The implementing command is run once,
    /// invoking the chained commands any number of times.
    /// </summary>
    public interface IRepeatTest : ICommandWrapper
    {
    }
}
