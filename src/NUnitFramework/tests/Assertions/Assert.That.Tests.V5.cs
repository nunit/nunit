using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public sealed class AssertThatTestsV5
    {
        [Test]
        [Explicit]
        public async Task AssertThat()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(3 + 4, Is.EqualTo(4 + 4));

                Assert.That(3 + 4).Is.EqualTo(4 + 4).Run();    // Explicit Run

                await Assert.That(3 + 4).Is.EqualTo(4 + 4);    // Explicit Awaited

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Assert.That(3 + 4).Is.EqualTo(4 + 4);          // Implicit Run by Interceptor
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
    }
}

#pragma warning disable CS9270 // FileName, Line, Column needs to be replaced by some magic hash

namespace NUnit.V5.Intercepts
{
    internal static partial class InvokeFinalBuilder
    {
        [InterceptsLocation(@"D:\Development\3rd\NUnit\nunit\src\NUnitFramework\tests\Assertions\Assert.That.Tests.V5.cs", 24, 39)]
        public static ConstraintBuilder<T, EqualConstraint<T>> EqualToAndRun<T>(
            this ActualValueBuilder<T> builder,
            T expected,
            string expectedExpression)
        {
            var invokeable = ConstraintExtensions.EqualTo(builder, expected, expectedExpression);
            invokeable.Run();
            return invokeable;
        }
    }
}
