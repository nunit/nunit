// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal.Builders;

    /// <summary>
    /// Indicates that a test method is a theory and can be run multiple times with different input data.
    /// </summary>
    /// <remarks>
    /// The theory attribute allows a test to be executed with a variety of data sets. For more details, please refer to the NUnit documentation.
    /// </remarks>
    /// <seealso href="https://docs.nunit.org/articles/nunit/writing-tests/attributes/theory.html">NUnit Theory Documentation</seealso>
    ///
    /// <example>
    /// public class SqrtTests
    ///{
    ///    [DatapointSource]
    ///    public double[] values = new double[] { 0.0, 1.0, -1.0, 42.0 };
    ///
    ///    [Theory]
    ///    public void SquareRootDefinition(double num)
    ///    {
    ///        Assume.That(num >= 0.0);
    ///
    ///        double sqrt = Math.Sqrt(num);
    ///
    ///        Assert.That(sqrt >= 0.0);
    ///        Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
    ///    }
    ///}
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TheoryAttribute : CombiningStrategyAttribute, ITestBuilder, IImplyFixture
    {
        /// <summary>
        /// Construct the attribute, specifying a combining strategy and source of parameter data.
        /// </summary>
        public TheoryAttribute(bool searchInDeclaringTypes = false) : base(
            new CombinatorialStrategy(),
            new ParameterDataProvider(new DatapointProvider(searchInDeclaringTypes), new ParameterDataSourceProvider()))
        {
        }
    }
}
