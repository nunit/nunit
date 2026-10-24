// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TestCommandExtensions
    {
        extension(TestCommand command)
        {
            /// <summary>
            /// Wraps the command once for each item, in order.
            /// Each wrap encloses the result of the previous one creating a chain of commands.
            /// </summary>
            public TestCommand WrapWith<T>(IEnumerable<T> items, Func<TestCommand, T, TestCommand> wrap)
            {
                TestCommand wrapped = command;
                foreach (T item in items)
                    wrapped = wrap(wrapped, item);

                return wrapped;
            }

            /// <summary>
            /// Wraps the command with each wrapper, in order.
            /// Each wrap encloses the result of the previous one.
            /// </summary>
            public TestCommand WrapWith(IEnumerable<ICommandWrapper> wrappers)
            {
                TestCommand wrapped = command;
                foreach (ICommandWrapper wrapper in wrappers)
                    wrapped = wrapper.Wrap(wrapped);

                return wrapped;
            }
        }
    }
}
