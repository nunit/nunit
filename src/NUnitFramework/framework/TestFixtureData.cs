// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// The TestFixtureData class represents a set of arguments
    /// and other parameter info to be used for a parameterized
    /// fixture. It is derived from TestFixtureParameters and adds a
    /// fluent syntax for use in initializing the fixture.
    /// </summary>
    public class TestFixtureData : TestFixtureParameters
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TestFixtureData(params object[] args)
            : base(args == null ? new object[] { null } : args)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestFixtureData(object arg)
            : base(new object[] { arg })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestFixtureData(object arg1, object arg2)
            : base(new object[] { arg1, arg2 })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestFixtureData(object arg1, object arg2, object arg3)
            : base( new object[] { arg1, arg2, arg3 })
        {
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Marks the test fixture as explicit.
        /// </summary>
        public TestFixtureData Explicit()	{
            this.RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test fixture as explicit, specifying the reason.
        /// </summary>
        public TestFixtureData Explicit(string reason)
        {
            this.RunState = RunState.Explicit;
            this.Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        /// <summary>
        /// Ignores this TestFixture, specifying the reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public TestFixtureData Ignore(string reason)
        {
            this.RunState = RunState.Ignored;
            this.Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }
}
