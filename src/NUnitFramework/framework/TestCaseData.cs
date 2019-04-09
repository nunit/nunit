// ***********************************************************************
// Copyright (c) 2008–2018 Charlie Poole, Rob Prouse
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
    /// <summary>
    /// The TestCaseData class represents a set of arguments
    /// and other parameter info to be used for a parameterized
    /// test case. It is derived from TestCaseParameters and adds a
    /// fluent syntax for use in initializing the test case.
    /// </summary>
    public class TestCaseData : TestCaseParameters
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TestCaseData(params object[] args)
            : base(args == null ? new object[] { null } : args)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg">The argument.</param>
        public TestCaseData(object arg)
            : base(new object[] { arg })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        public TestCaseData(object arg1, object arg2)
            : base(new object[] { arg1, arg2 })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseData"/> class.
        /// </summary>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        public TestCaseData(object arg1, object arg2, object arg3)
            : base( new object[] { arg1, arg2, arg3 })
        {
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Sets the expected result for the test
        /// </summary>
        /// <param name="result">The expected result</param>
        /// <returns>A modified TestCaseData</returns>
        public TestCaseData Returns(object result)
        {
            this.ExpectedResult = result;
            return this;
        }

        /// <summary>
        /// Sets the name of the test case
        /// </summary>
        /// <returns>The modified TestCaseData instance</returns>
        public TestCaseData SetName(string name)
        {
            this.TestName = name;
            return this;
        }

        /// <summary>
        /// Sets the list of display names to use as the parameters in the test name.
        /// </summary>
        public TestCaseData SetArgDisplayNames(params string[] displayNames)
        {
            ArgDisplayNames = displayNames;
            return this;
        }

        /// <summary>
        /// Sets the description for the test case
        /// being constructed.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The modified TestCaseData instance.</returns>
        public TestCaseData SetDescription(string description)
        {
            this.Properties.Set(PropertyNames.Description, description);
            return this;
        }

        /// <summary>
        /// Applies a category to the test
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public TestCaseData SetCategory(string category)
        {
            this.Properties.Add(PropertyNames.Category, category);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, string propValue)
        {
            this.Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, int propValue)
        {
            this.Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Applies a named property to the test
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public TestCaseData SetProperty(string propName, double propValue)
        {
            this.Properties.Add(propName, propValue);
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit.
        /// </summary>
        public TestCaseData Explicit()	{
            this.RunState = RunState.Explicit;
            return this;
        }

        /// <summary>
        /// Marks the test case as explicit, specifying the reason.
        /// </summary>
        public TestCaseData Explicit(string reason)
        {
            this.RunState = RunState.Explicit;
            this.Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        /// <summary>
        /// Ignores this TestCase, specifying the reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        public TestCaseData Ignore(string reason)
        {
            this.RunState = RunState.Ignored;
            this.Properties.Set(PropertyNames.SkipReason, reason);
            return this;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Returns a list of <see cref="TestCaseData"/> objects representing all possible combinations of the given parameters.
        /// </summary>
        /// <param name="values">Each array in this object array is to represent a parameter for the method being tested</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TestCaseData"/> objects that represents all possible combinations for the given parameters</returns>
        /// <remarks>
        /// This method will generate a collection of <see cref="TestCaseData"/> instances where the collection represents every possible combination that could be made
        /// with the given parameters.  That is to say, with the following code:
        /// <code>
        /// return TestCaseData.Combinatorial(
        ///             new[] { true, false }.Cast&lt;object&gt;(),
        ///             new[] { 0, 1, 10 }.Cast&lt;object&gt;());
        /// </code>
        /// 
        /// The following test data will be returned:
        /// <list type="bullet">
        /// <item><description>TestCaseData(true, 0)</description></item>
        /// <item><description>TestCaseData(true, 1)</description></item>
        /// <item><description>TestCaseData(true, 10)</description></item>
        /// <item><description>TestCaseData(false, 0)</description></item>
        /// <item><description>TestCaseData(false, 1)</description></item>
        /// <item><description>TestCaseData(false, 10)</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// public class TestClass
        /// {
        ///     #region TestCaseSource
        /// 
        ///     private static IEnumerable ExampleTest_TestCases
        ///     {
        ///         return TestCaseData.Combinatorial(
        ///             new[] { true, false }(),
        ///             new[] { 0, 1, 10 }());
        ///     }
        /// 
        ///     #endregion
        ///     [Test, TestCaseSource(typeof(TestClass), nameof(ExampleTest_TestCases))]
        ///     public void ExampleTest(bool condition, int input)
        ///     {
        ///          // Execute test
        ///     }
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> Combinatorial(params IEnumerable[] values)
        {
            return CombinatorialStrategy.GetCombinations<TestCaseData>(values, (o) => new TestCaseData(o));
        }

        #endregion
    }
}
