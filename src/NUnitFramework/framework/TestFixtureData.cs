// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        public TestFixtureData(params object?[]? args)
            : base(args ?? new object?[] { null })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestFixtureData(object? arg)
            : base(new object?[] { arg })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestFixtureData(object? arg1, object? arg2)
            : base(new object?[] { arg1, arg2 })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestFixtureData(object? arg1, object? arg2, object? arg3)
            : base(new object?[] { arg1, arg2, arg3 })
        {
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the name of the test fixture
        /// </summary>
        /// <returns>The modified TestFixtureData instance</returns>
        internal TestFixtureData SetName(string? name)
        {
            TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        public TestFixtureData SetArgDisplayNames(params string[]? displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// Objects are formatted using the same logic as default test names.
        /// </summary>
        public TestFixtureData SetArgDisplayNames(params object?[]? displayNames)
        {
            if (displayNames is null)
            {
                ArgDisplayNames = null;
            }
            else
            {
                var formattedNames = new string[displayNames.Length];
                for (int i = 0; i < displayNames.Length; i++)
                {
                    formattedNames[i] = Constraints.MsgUtils.FormatValue(displayNames[i]);
                }
                ArgDisplayNames = formattedNames;
            }
            return this;
        }

        /// <summary>
        /// Marks the test fixture as explicit.
        /// </summary>
        public TestFixtureData Explicit()
        {
            RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test fixture as explicit, specifying the reason.
        /// </summary>
        public TestFixtureData Explicit(string reason)
        {
            RunState = RunState.Explicit;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        /// <summary>
        /// Ignores this TestFixture, specifying the reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public TestFixtureData Ignore(string reason)
        {
            RunState = RunState.Ignored;
            Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
    }
}
