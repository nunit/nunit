// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    using Interfaces;
    using Internal.Builders;

    /// <summary>
    /// Adding this attribute to a method within a <seealso cref="TestFixtureAttribute"/> 
    /// class makes the method callable from the NUnit test runner. There is a property 
    /// called Description which is optional which you can provide a more detailed test
    /// description. This class cannot be inherited.
    /// </summary>
    /// 
    /// <example>
    /// [TestFixture]
    /// public class Fixture
    /// {
    ///   [Test]
    ///   public void MethodToTest()
    ///   {}
    ///   
    ///   [Test(Description = "more detailed description")]
    ///   public void TestDescriptionMethod()
    ///   {}
    /// }
    /// </example>
    /// 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited=true)]
    public class TheoryAttribute : CombiningStrategyAttribute, ITestBuilder, IImplyFixture
    {
        /// <summary>
        /// Construct the attribute, specifying a combining strategy and source of parameter data.
        /// </summary>
        public TheoryAttribute() : base(
            new CombinatorialStrategy(),
            new ParameterDataProvider(new DatapointProvider(), new ParameterDataSourceProvider()))
        {
        }
    }
}
