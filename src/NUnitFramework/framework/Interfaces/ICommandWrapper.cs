// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
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
using NUnit.Framework.Internal.Commands;

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
}
