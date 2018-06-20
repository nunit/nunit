using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.TestData;

namespace NUnit.Framework.Api
{
    [TestFixture] // [TestFixture] is needed so that ClassesCanBeImpliedToBeFixtures is effective.
    public static class DefaultTestAssemblyBuilderTests
    {
        [Test]
        public static void ClassesCanBeImpliedToBeFixtures()
        {
            var assemblyTest = new DefaultTestAssemblyBuilder().Build(
                typeof(ImpliedFixture).GetTypeInfo().Assembly, 
                options: new Dictionary<string, object>());

            Assert.That(
                new[] { assemblyTest }.SelectManyRecursive(test => test.Tests),
                Has.Some.With.Property("Type").EqualTo(typeof(ImpliedFixture)));
        }
    }
}
