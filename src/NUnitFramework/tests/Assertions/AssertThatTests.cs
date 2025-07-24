// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertThatTests
    {
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
        [Test]
        public void AssertionPasses_Boolean()
        {
            Assert.That(2 + 2 == 4);
        }

        [Test]
        public void AssertionPasses_BooleanWithMessage()
        {
            Assert.That(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssertionPasses_BooleanWithNullMessage()
        {
            Assert.That(2 + 2 == 4, default(string));
        }

        [Test]
        public void AssertionPasses_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(2 + 2 == 4, GetExceptionMessage);
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void AssertionPasses_ActualAndConstraint()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessage()
        {
            Assert.That(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithNullMessage()
        {
            Assert.That(2 + 2, Is.EqualTo(4), default(string));
        }

        [Test]
        public void AssertionPasses_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraint()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessage()
        {
            Assert.That(() => 2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssertionPasses_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => $"Not Equal to {4}";
            Assert.That(() => 2 + 2, Is.EqualTo(4), GetExceptionMessage);
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraint()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4));
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessage()
        {
            Assert.That(ReturnsFour, Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssertionPasses_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            Assert.That(ReturnsFour, Is.EqualTo(4), GetExceptionMessage);
        }

        private int ReturnsFour() => 4;

        [Test]
        public void TestEquatableWithConvertible()
        {
            var actual = new Number(42);
            var expected = new Number(42.0);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private readonly struct Number : IEquatable<Number>, IConvertible
        {
            private readonly double _value;

            public Number(int value) => _value = value;
            public Number(double value) => _value = value;

            public bool Equals(Number other)
            {
                return _value == other._value;
            }

            TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
            byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotImplementedException();
            sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotImplementedException();
            ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();
            short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotImplementedException();
            uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();
            int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotImplementedException();
            ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();
            long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotImplementedException();
            string IConvertible.ToString(IFormatProvider? provider) => throw new NotImplementedException();
            bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();
            char IConvertible.ToChar(IFormatProvider? provider) => throw new NotImplementedException();
            DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();
            decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();
            double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotImplementedException();
            float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotImplementedException();
            object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => throw new NotImplementedException();
        }

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void FailureThrowsAssertionException_Boolean()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, "message"));
            Assert.That(ex?.Message, Does.Contain("message"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2 == 5, Is.True)"));
        }

        [Test]
        public void FailureThrowsAssertionException_BooleanWithMessageStringFunc()
        {
            string GetExceptionMessage() => "Not Equal to 4";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2 == 5, GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("Not Equal to 4"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2 == 5, Is.True)"));
        }
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5)));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_ActualLambdaAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => 2 + 2, Is.EqualTo(5), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => 2 + 2, Is.EqualTo(5))"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraint()
        {
            Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4)));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessage()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(ReturnsFive, Is.EqualTo(4))"));
        }

        [Test]
        public void FailureThrowsAssertionException_DelegateAndConstraintWithMessageStringFunc()
        {
            string GetExceptionMessage() => "error";
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ReturnsFive, Is.EqualTo(4), GetExceptionMessage));
            Assert.That(ex?.Message, Does.Contain("error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(ReturnsFive, Is.EqualTo(4))"));
        }

        [Test]
        public void AssertionsAreCountedCorrectly()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(AssertCountFixture));

            int totalCount = 0;
            foreach (var childResult in result.Children)
            {
                int expectedCount = childResult.Name == "ThreeAsserts" ? 3 : 1;
                Assert.That(childResult.AssertCount, Is.EqualTo(expectedCount), $"Bad count for {childResult.Name}");
                totalCount += expectedCount;
            }

            Assert.That(result.AssertCount, Is.EqualTo(totalCount), "Fixture count is not correct");
        }

        [Test]
        public void PassingAssertion_DoesNotCallExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
#pragma warning disable NUnit2045 // Use Assert.Multiple
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(0 + 1 == 1, GetExceptionMessage);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
#pragma warning restore NUnit2045 // Use Assert.Multiple

            // Assert
            Assert.That(!funcWasCalled, "The getExceptionMessage function was called when it should not have been.");
        }

        [Test]
        public void FailingAssertion_CallsExceptionStringFunc()
        {
            // Arrange
            var funcWasCalled = false;

            string GetExceptionMessage()
            {
                funcWasCalled = true;
                return "Func was called";
            }

            // Act
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            var ex = Assert.Throws<AssertionException>(() => Assert.That(1 + 1 == 1, GetExceptionMessage));
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

            // Assert
            Assert.That(ex?.Message, Does.Contain("Func was called"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(1 + 1 == 1, Is.True)"));
            Assert.That(funcWasCalled, "The getExceptionMessage function was not called when it should have been.");
        }

        [Test]
        public void OnlyFailingAssertion_FormatsString()
        {
            const string text = "String was formatted";
            var formatCounter = new FormatCounter();

            Assert.That(1 + 1, Is.EqualTo(2), $"{text} {formatCounter}");
            Assert.That(formatCounter.NumberOfToStringCalls, Is.EqualTo(0), "The interpolated string should not have been evaluated");

            Assert.That(() => Assert.That(1 + 1, Is.Not.EqualTo(2), $"{text} {formatCounter}"),
                Throws.InstanceOf<AssertionException>()
                    .With.Message.Contains(text).
                    And
                    .With.Message.Contains("Assert.That(1 + 1, Is.Not.EqualTo(2)"));

            Assert.That(formatCounter.NumberOfToStringCalls, Is.EqualTo(1), "The interpolated string should have been evaluated once");
        }

        private sealed class FormatCounter
        {
            public int NumberOfToStringCalls { get; private set; }

            public override string ToString()
            {
                NumberOfToStringCalls++;
                return string.Empty;
            }
        }

        private int ReturnsFive()
        {
            return 5;
        }

        [Test]
        public void AssertThatSuccess()
        {
            Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(1));
        }

        [Test]
        public void AssertThatFailure()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(async () => await AsyncReturnOne(), Is.EqualTo(2)));
        }

        [Test, Platform(Exclude = "Linux", Reason = "Intermittent failures on Linux")]
        public void AssertThatErrorTask()
        {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionTask(), Is.EqualTo(1)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionTask"));
        }

        [Test]
        public void AssertThatErrorGenericTask()
        {
            var exception =
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await ThrowInvalidOperationExceptionGenericTask(), Is.EqualTo(1)));

            Assert.That(exception?.StackTrace, Does.Contain("ThrowInvalidOperationExceptionGenericTask"));
        }

        private static Task<int> AsyncReturnOne()
        {
            return Task.Run(() => 1);
        }

        private static async Task<int> ThrowInvalidOperationExceptionGenericTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

        private static async Task ThrowInvalidOperationExceptionTask()
        {
            await AsyncReturnOne();
            throw new InvalidOperationException();
        }

        [Test]
        public void AssertThatWithLambda()
        {
            Assert.That(() => true);
        }

        [Test]
        public void AssertThatWithFalseLambda()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(() => false, "Error"));
            Assert.That(ex?.Message, Does.Contain("Error"));
            Assert.That(ex?.Message, Does.Contain("Assert.That(() => false, Is.True)"));
        }

        [TestCase(default(string), default(string))]
        [TestCase("", "")]
        public void AssertWithStrings(string? actual, string? expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AssertWithExpectedTypeImplicitConvertibleToString()
        {
            const string value = "Implicit Cast";
            var instance = new TypeWithImplicitCastToString(value);

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(value, Is.Not.EqualTo(instance), "EqualConstaint");
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
        }

        [Test]
        public void AssertWithActualTypeImplicitConvertibleToString()
        {
            const string value = "Implicit Cast";
            var instance = new TypeWithImplicitCastToString(value);

            Assert.Multiple(() =>
            {
                Assert.That(instance.Value, Is.EqualTo(value), "Value");
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
                Assert.That(instance, Is.Not.EqualTo(value), "ImplicitOperatorNotConsidered");
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            });
        }

        private sealed class TypeWithImplicitCastToString
        {
            public string Value { get; }

            public TypeWithImplicitCastToString(string value)
            {
                Value = value;
            }

            public static implicit operator string(TypeWithImplicitCastToString instance) => instance.Value;
        }

        [Test]
        public void AssertWithTypeWhichImplementsIEquatableString()
        {
            const string value = "Equatable<string>";
            var instance = new TypeWhichImplementsIEquatableString(value);

            Assert.Multiple(() =>
            {
                Assert.That(instance.Value, Is.EqualTo(value), "Value");
                Assert.That(instance, Is.EqualTo(value), "EqualStringConstaint");
                Assert.That(() => Assert.That(instance, Is.EqualTo(value.ToLowerInvariant()).IgnoreCase),
                            Throws.InvalidOperationException);
            });
        }

        private sealed class TypeWhichImplementsIEquatableString : IEquatable<string>
        {
            public string Value { get; }

            public TypeWhichImplementsIEquatableString(string value)
            {
                Value = value;
            }

            public bool Equals(string? other)
            {
                return Value.Equals(other);
            }
        }

        [TestCase("Hello", "World")]
        [TestCase('A', 'B')]
        [TestCase(false, true)]
        [TestCase(SomeEnum.One, SomeEnum.Two)]
        public void AssertThatWithTypesNotSupportingTolerance(object? x, object? y)
        {
            Assert.That(() => Assert.That(x, Is.EqualTo(y).Within(0.1)),
                        Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
        }

        [TestCase(40, 42)]
        public void AssertThatWithInvalidTolerance(object actual, object expected)
        {
            Assert.That(() => Assert.That(actual, Is.EqualTo(expected).Within(1).Seconds),
                        Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
        }

        [Test]
        public async Task AssertThatWithInvalidTolerance()
        {
            DateTime expected = DateTime.UtcNow;

            await Task.Delay(500);

            DateTime actual = DateTime.UtcNow;

            Assert.That(actual, Is.EqualTo(expected).Within(1).Seconds);
            Assert.That(() => Assert.That(actual, Is.EqualTo((object)expected).Within(1)),
                        Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
        }

        [Test]
        public async Task AssertPropertiesComparerOnlyUsesToleranceWhereAppropriate()
        {
            var expected = new RecordWithDifferentToleranceAwareMembers(1, "Name", 1.80, DateTimeOffset.UtcNow);
            await Task.Delay(500);
            var actual = new RecordWithDifferentToleranceAwareMembers(1, "Name", 1.80, DateTimeOffset.UtcNow);

#pragma warning disable NUnit2047 // Incompatible types for Within constraint
            Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer().Within(1.0).Seconds);
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
        }

        [Test]
        public async Task AssertPropertiesComparerOnlyUsesToleranceWhereSpecified()
        {
            var expected = new RecordWithDifferentToleranceAwareMembers(1, "Name", 1.80, DateTimeOffset.UtcNow);
            await Task.Delay(500);
            var actual = new RecordWithDifferentToleranceAwareMembers(2, "Name", 1.81, DateTimeOffset.UtcNow);

            // Fails because of Id and Height field
            Assert.That(actual, Is.Not.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(TimeSpan.FromSeconds(1))));

            // Fails because of Start field
            Assert.That(actual, Is.Not.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1)));

            // Fails because of Height field
            Assert.That(actual, Is.Not.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1).Within(TimeSpan.FromSeconds(1))));
            // Fails because of Id field
            Assert.That(actual, Is.Not.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1.0).Within(TimeSpan.FromSeconds(1))));

            // Succeeds, uses 1 tolerance for Id and 0.1 tolerance for Height
            Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1).Within(0.1).Within(TimeSpan.FromSeconds(1))));

            // Succeeds, used 1 tolerance for Id and 1% tolerance for Height
            Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1).Within(new Tolerance(1.0).Percent).Within(TimeSpan.FromSeconds(1))));

            // Fails because Height tolerance is too small
            Assert.That(actual, Is.Not.EqualTo(expected).UsingPropertiesComparer(
                c => c.Within(1).Within(0.001).Within(TimeSpan.FromSeconds(1))));
        }

        private record RecordWithDifferentToleranceAwareMembers(int Id, string Name, double Height, DateTimeOffset Start);

        [Test]
        public void AssertThatEqualsWithClassWithSomeToleranceAwareMembers()
        {
            var zero = new ClassWithSomeToleranceAwareMembers(0, 0.0, string.Empty, null);
            var instance = new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero);

            Assert.Multiple(() =>
            {
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer());

                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero),
                            Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(nameof(ClassWithSomeToleranceAwareMembers.ValueB))));
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero),
                            Is.EqualTo(instance).Within(0.1).UsingPropertiesComparer());

                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", null),
                            Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", null),
                            Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(nameof(ClassWithSomeToleranceAwareMembers.Chained))));

                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "2.2", zero),
                            Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.1, "2.2", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(nameof(ClassWithSomeToleranceAwareMembers.ValueC))));

                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(nameof(ClassWithSomeToleranceAwareMembers.ValueA))));

                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 2.2, "2.2", zero),
                            Is.Not.EqualTo(instance).UsingPropertiesComparer());

                // Exclude all but one property.
                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(
                                nameof(ClassWithSomeToleranceAwareMembers.ValueA),
                                nameof(ClassWithSomeToleranceAwareMembers.ValueB),
                                nameof(ClassWithSomeToleranceAwareMembers.ValueC))));

                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.EqualTo(instance)
                              .UsingPropertiesComparer<ClassWithSomeToleranceAwareMembers>(c => c.Excluding(
                                x => x.ValueA,
                                x => x.ValueB,
                                x => x.ValueC)));

                // Only test for the one property.
                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.EqualTo(instance).UsingPropertiesComparer(
                                c => c.Using(nameof(ClassWithSomeToleranceAwareMembers.Chained))));

                Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                            Is.EqualTo(instance)
                              .UsingPropertiesComparer<ClassWithSomeToleranceAwareMembers>(c => c.Using(x => x.Chained)));

                // Property names work on nested classes!
                var alsmostZero = new ClassWithSomeToleranceAwareMembers(1, 0.0, string.Empty, null);
                Assert.That(new ClassWithSomeToleranceAwareMembers(1, 2.2, "2.2", alsmostZero),
                            Is.EqualTo(instance).UsingPropertiesComparer(
                                c => c.Using(nameof(ClassWithSomeToleranceAwareMembers.ValueA))));

                // We can't test properties that don't exist
                Assert.That(() =>
                            Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                                        Is.EqualTo(instance).UsingPropertiesComparer(c => c.Using("ValueD"))),
                            Throws.InstanceOf<ArgumentException>().With.Message.Contains("must all exist"));

                // We can't exclude properties that don't exist
                Assert.That(() =>
                            Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", zero),
                                        Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding("ValueD"))),
                            Throws.InstanceOf<ArgumentException>().With.Message.Contains("must all exist"));

                // We don't allow excluding all properties.
                Assert.That(() =>
                            Assert.That(new ClassWithSomeToleranceAwareMembers(2, 1.1, "1.1", null),
                                        Is.EqualTo(instance).UsingPropertiesComparer(c => c.Excluding(
                                            nameof(ClassWithSomeToleranceAwareMembers.ValueA),
                                            nameof(ClassWithSomeToleranceAwareMembers.ValueB),
                                            nameof(ClassWithSomeToleranceAwareMembers.ValueC),
                                            nameof(ClassWithSomeToleranceAwareMembers.Chained)))),
                            Throws.InstanceOf<NotSupportedException>().With.Message.Contains("No comparer found"));
            });
        }

        [Test]
        [DefaultFloatingPointTolerance(0.1)]
        public void AssertThatEqualsWithClassWithSomeToleranceAwareMembersUsesDefaultFloatingPointTolerance()
        {
            var zero = new ClassWithSomeToleranceAwareMembers(0, 0.0, string.Empty, null);
            var instance = new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", zero);

            Assert.That(new ClassWithSomeToleranceAwareMembers(1, 1.2, "1.1", zero), Is.EqualTo(instance).UsingPropertiesComparer());
        }

        [Test]
        public void AssertThatDifferentTypesCanBeCompared()
        {
            var instanceC = new ClassWithSomeToleranceAwareMembers(1, 1.1, "1.1", null);
            var instanceS = new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.Two);

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(() =>
                Assert.That(instanceS, Is.Not.EqualTo(instanceC).UsingPropertiesComparer()),
                Throws.InstanceOf<NotSupportedException>());
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint

            Assert.That(instanceS, Is.EqualTo(instanceC).UsingPropertiesComparer(
                c => c.CompareOnlyCommonProperties()));
        }

        private sealed class ClassWithSomeToleranceAwareMembers
        {
            public ClassWithSomeToleranceAwareMembers(int valueA, double valueB, string valueC, ClassWithSomeToleranceAwareMembers? chained)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                Chained = chained;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public ClassWithSomeToleranceAwareMembers? Chained { get; }

            public override string ToString()
            {
                return $"{ValueA} {ValueB} '{ValueC}' [{Chained}]";
            }
        }

        [Test]
        public void AssertThatEqualsWithStructWithSomeToleranceAwareMembers()
        {
            var instance = new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One);

            Assert.Multiple(() =>
            {
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.2, "1.1", SomeEnum.One), Is.EqualTo(instance).Within(0.1).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 1.1, "1.1", SomeEnum.Two), Is.Not.EqualTo(instance).Within(0.1).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 2.2, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 1.1, "1.1", SomeEnum.One), Is.Not.EqualTo(instance).UsingPropertiesComparer());
            });
        }

        [Test]
        public void AssertThatEqualsWithStructMemberDifferences()
        {
            var instance = new StructWithSomeToleranceAwareMembers(1, 0.123, "1.1", SomeEnum.One);

            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 0.123, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                    Throws.InstanceOf<AssertionException>().With
                        .Message.Contains("Expected: StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("But was:  StructWithSomeToleranceAwareMembers { ValueA = 2, ValueB = 0.123d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueA").And
                        .Message.Contains("Expected: 1").And
                        .Message.Contains("But was:  2"));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.246, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                    Throws.InstanceOf<AssertionException>().With
                        .Message.Contains("Expected: StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("But was:  StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.246d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueB").And
                        .Message.Contains("Expected: 0.123d").And
                        .Message.Contains("But was:  0.246d"));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.123, "2.2", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer()),
                    Throws.InstanceOf<AssertionException>().With
                        .Message.Contains("Expected: StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("But was:  StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"2.2\", ValueD = One }").And
                        .Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueC").And
                        .Message.Contains("Expected: \"1.1\"").And
                        .Message.Contains("But was:  \"2.2\""));
            Assert.That(() =>
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.123, "1.1", SomeEnum.Two), Is.EqualTo(instance).UsingPropertiesComparer()),
                    Throws.InstanceOf<AssertionException>().With
                        .Message.Contains("Expected: StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"1.1\", ValueD = One }").And
                        .Message.Contains("But was:  StructWithSomeToleranceAwareMembers { ValueA = 1, ValueB = 0.123d, ValueC = \"1.1\", ValueD = Two }").And
                        .Message.Contains("at property StructWithSomeToleranceAwareMembers.ValueD").And
                        .Message.Contains("Expected: One").And
                        .Message.Contains("But was:  Two"));

            /*
             * Uncomment this block to see the actual exception messages. Test will fail.
             *
            Assert.Multiple(() =>
            {
                Assert.That(new StructWithSomeToleranceAwareMembers(2, 0.123, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.246, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.123, "1.1", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.123, "1.2", SomeEnum.One), Is.EqualTo(instance).UsingPropertiesComparer());
                Assert.That(new StructWithSomeToleranceAwareMembers(1, 0.123, "1.1", SomeEnum.Two), Is.EqualTo(instance).UsingPropertiesComparer());
            });
             */
        }

        private enum SomeEnum
        {
            One = 1,
            Two = 2,
        }

        private readonly struct StructWithSomeToleranceAwareMembers
        {
            public StructWithSomeToleranceAwareMembers(int valueA, double valueB, string valueC, SomeEnum valueD)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                ValueD = valueD;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public SomeEnum ValueD { get; }
        }

        [Test]
        public void AssertThatEqualsWithStructWithNoToleranceAwareMembers()
        {
            var instance = new StructWithNoToleranceAwareMembers("1.1", SomeEnum.One);

            Assert.Multiple(() =>
            {
                Assert.That(new StructWithNoToleranceAwareMembers("1.1", SomeEnum.One), Is.EqualTo(instance));
                Assert.That(new StructWithNoToleranceAwareMembers("1.2", SomeEnum.One), Is.Not.EqualTo(instance));
                Assert.That(new StructWithNoToleranceAwareMembers("1.1", SomeEnum.Two), Is.Not.EqualTo(instance));
                Assert.That(() =>
                    Assert.That(new StructWithNoToleranceAwareMembers("1.2", SomeEnum.One),
                                Is.EqualTo(instance).Within(0.1)),
                    Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
            });
        }

        private readonly struct StructWithNoToleranceAwareMembers
        {
            public StructWithNoToleranceAwareMembers(string valueA, SomeEnum valueB)
            {
                ValueA = valueA;
                ValueB = valueB;
            }

            public string ValueA { get; }
            public SomeEnum ValueB { get; }

            public override string ToString()
            {
                return $"'{ValueA}' {ValueB}";
            }
        }

        [Test]
        public void AssertThatEqualsWithRecord()
        {
            var zero = new SomeRecord(0, 0.0, string.Empty, null);
            var instance = new SomeRecord(1, 1.1, "1.1", zero);

            Assert.Multiple(() =>
            {
                Assert.That(new SomeRecord(1, 1.1, "1.1", zero), Is.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.2, "1.1", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.1, "1.1", null), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 1.1, "2.2", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(1, 2.2, "1.1", zero), Is.Not.EqualTo(instance));
                Assert.That(new SomeRecord(2, 1.1, "1.1", zero), Is.Not.EqualTo(instance));
#pragma warning disable NUnit2047 // Incompatible types for Within constraint
                Assert.That(() =>
                    Assert.That(new SomeRecord(1, 1.2, "1.1", zero),
                                Is.EqualTo(instance).Within(0.1)),
                    Throws.InstanceOf<NotSupportedException>().With.Message.Contains("Tolerance"));
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
            });
        }

        private sealed record SomeRecord
        {
            public SomeRecord(int valueA, double valueB, string valueC, SomeRecord? chained)
            {
                ValueA = valueA;
                ValueB = valueB;
                ValueC = valueC;
                Chained = chained;
            }

            public int ValueA { get; }
            public double ValueB { get; }
            public string ValueC { get; }
            public SomeRecord? Chained { get; }

            public override string ToString()
            {
                return $"{ValueA} {ValueB} '{ValueC}' [{Chained}]";
            }
        }

        [Test]
        public void AssertWithRecursiveClass()
        {
            LinkedList list1 = new(1, new(2, new(3)));
            LinkedList list2 = new(1, new(2, new(3)));

            Assert.That(list1, Is.Not.EqualTo(list2));
            Assert.That(list1, Is.EqualTo(list2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertWithCyclicRecursiveClass()
        {
            LinkedList list1 = new(1);
            LinkedList list2 = new(1);

            list1.Next = list1;
            list2.Next = list2;

            Assert.That(list1, Is.Not.EqualTo(list2)); // Reference comparison
            Assert.That(list1, Is.EqualTo(list2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertRecordsComparingProperties()
        {
            var record1 = new Record("Name", [1, 2, 3]);
            var record2 = new Record("Name", [1, 2, 3]);

            Assert.That(record1, Is.Not.EqualTo(record2)); // Record's generated method does not handle collections
            Assert.That(record1, Is.EqualTo(record2).UsingPropertiesComparer());
        }

        [Test]
        public void AssertRecordsComparingProperties_WhenRecordHasUserDefinedEqualsMethod()
        {
            var record1 = new ParentRecord(new RecordWithOverriddenEquals("Name"), [1, 2, 3]);
            var record2 = new ParentRecord(new RecordWithOverriddenEquals("NAME"), [1, 2, 3]);

            Assert.That(record1, Is.Not.EqualTo(record2)); // ParentRecord's generated method does not handle collections
            Assert.That(record1, Is.EqualTo(record2).UsingPropertiesComparer());
        }

        private sealed class LinkedList
        {
            public LinkedList(int value, LinkedList? next = null)
            {
                Value = value;
                Next = next;
            }

            public int Value { get; }

            public LinkedList? Next { get; set; }
        }

        [Test]
        public void EqualMemberWithIndexer()
        {
            var members = new Members("Hello", "World", "NUnit");
            var copy = new Members("Hello", "World", "NUnit");

            Assert.That(members[1], Is.EqualTo("World"));
            Assert.That(copy, Is.Not.EqualTo(members));
            Assert.That(() => Assert.That(copy, Is.EqualTo(members).UsingPropertiesComparer()), Throws.InstanceOf<NotSupportedException>());
        }

        private sealed class Members
        {
            private readonly string[] _members;

            public Members(params string[] members)
            {
                _members = members;
            }

            public string this[int index] => _members[index];
        }

        [Test]
        public void PropertiesComparerDoesNotSupportPropertiesOfDifferentTypes()
        {
            var actual = new TypeWithObjectProperty { Value = new ClassA(123) };
            var expected = new TypeWithObjectProperty { Value = new ClassB("Not A Number") };
            Assert.That(() => Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer()), Throws.InstanceOf<AssertionException>());
        }

        private class TypeWithObjectProperty
        {
            public object? Value { get; set; }
        }

        private record ClassA(int Number);
        private record ClassB(string String);

        [Test]
        public void TestPropertyFailureSecondLevel()
        {
            var one = new ParentClass(new ChildClass(new GrandChildClass(1)), new ChildClass(new GrandChildClass(2), new GrandChildClass(3)));
            var two = new ParentClass(new ChildClass(new GrandChildClass(1)), new ChildClass(new GrandChildClass(2), new GrandChildClass(4)));

            Assert.That(() => Assert.That(two, Is.EqualTo(one).UsingPropertiesComparer()),
                        Throws.InstanceOf<AssertionException>().With.Message.Contains("at property ParentClass.Two")
                                                               .And.Message.Contains("at property ChildClass.Values")
                                                               .And.Message.Contains("at index [1]")
                                                               .And.Message.Contains("at property GrandChildClass.Value")
                                                               .And.Message.Contains("Expected: 3"));

            /*
             * Uncomment this block to see the actual exception messages. Test will fail.
             *
            Assert.That(two, Is.EqualTo(one).UsingPropertiesComparer());
             */
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_EqualTo()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.EqualTo([1, 2, 3]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_EquivalentTo()
        {
            var actual = new[] { 3, 2, 1 };
            Assert.That(actual, Is.EquivalentTo([1, 2, 3]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_SubsetOf()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.SubsetOf([1, 2, 3, 4, 5]));
        }

        [Test]
        public void UseAssertThatWithCollectionExpression_SupersetOf()
        {
            var actual = new[] { 1, 2, 3 };
            Assert.That(actual, Is.SupersetOf([1, 2]));
        }

        private record Record(string Name, int[] Collection);

        private record ParentRecord(RecordWithOverriddenEquals Child, int[] Collection);

        private record RecordWithOverriddenEquals(string Name)
        {
            public virtual bool Equals(RecordWithOverriddenEquals? other)
            {
                return string.Equals(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return Name.ToUpperInvariant().GetHashCode();
            }
        }

        private sealed class ParentClass
        {
            public ParentClass(ChildClass one, ChildClass two)
            {
                One = one;
                Two = two;
            }

            public ChildClass One { get; }

            public ChildClass Two { get; }
        }

        private sealed class ChildClass
        {
            public ChildClass(params GrandChildClass[] values) => Values = values;

            public GrandChildClass[] Values { get; }
        }

        private sealed class GrandChildClass
        {
            public GrandChildClass(int value) => Value = value;

            public int Value { get; }
        }

        // Different type with different property name comparison
        private sealed record Address(string House, string Street, string City, string AreaCode, string Country);
        private sealed record Person(string Name, Address Address);

        private sealed record USAddress(string House, string Street, string City, string ZipCode);
        private sealed record USPerson(string Name, USAddress USAddress);

        [Test]
        public void CompareMatchingDifferentTypes()
        {
            var person = new Person("John Doe", new Address("10", "CSI", "Las Vegas", "89030", "U.S.A."));
            var usPerson = new USPerson("John Doe", new USAddress("10", "CSI", "Las Vegas", "89030"));

            // We can supply a Value for the missing property 'Country'
            Assert.That(usPerson, Is.EqualTo(person).UsingPropertiesComparer<Person>(
                c => c.Map<USPerson>(x => x.Address, y => y.USAddress)
                      .Map<Address, USAddress>(x => x.AreaCode, y => y.ZipCode)
                      .Map<Address>(x => x.Country, "U.S.A.")));

            // Or we can exclude the 'Country' property.
            // However this would also pass when the source country is not U.S.A.
            Assert.That(usPerson, Is.EqualTo(person).UsingPropertiesComparer<Person>(
                c => c.Map<USPerson>(x => x.Address, y => y.USAddress)
                      .Map<Address, USAddress>(x => x.AreaCode, y => y.ZipCode)
                      .Excluding<Address>(x => x.Country)));
        }

        [Test]
        public void CompareMismatchedDifferentTypes()
        {
            var person = new Person("John Doe", new Address("10", "CSI", "Las Vegas", "89030", "U.S.A."));
            var usPerson = new USPerson("John Doe", new USAddress("10", "CSI", "Las Vegas", "89031"));

            Assert.That(() =>
                Assert.That(usPerson, Is.EqualTo(person).UsingPropertiesComparer<Person>(
                    c => c.Map<USPerson>(x => x.Address, y => y.USAddress)
                          .Map<Address, USAddress>(x => x.AreaCode, y => y.ZipCode)
                          .Map<Address>(x => x.Country, "U.S.A."))),
                    Throws.InstanceOf<AssertionException>()
                          .With.Message.Contains("at property Person.Address => USPerson.USAddress")
                          .And.Message.Contains("at property Address.AreaCode => USAddress.ZipCode")
                          .And.Message.Contains("Expected: \"89030\"")
                          .And.Message.Contains("But was:  \"89031\""));
        }

        [Test]
        public void CompareWrongTypes()
        {
            var person1 = new Person("John Doe", new Address("10", "CSI", "Las Vegas", "89030", "U.S.A."));
            var person2 = new Person("John Doe", new Address("10", "CSI", "Las Vegas", "89030", "U.S.A."));

            Assert.That(() =>
                Assert.That(person1, Is.EqualTo(person2).UsingPropertiesComparer<USPerson>(
                    c => c.Excluding(x => x.USAddress))),
                    Throws.ArgumentException.With.Message.Contains(
                    "The type parameter USPerson does not match the type parameter Person of this constraint."));
        }

        [Test]
        public void TestToVerifyThatWhenSteppingThroughWithDebuggerThisDoesNotAlterBehaviour()
        {
            EqualStringConstraint expression = Is.Not.EqualTo("abc");
            Assert.That(expression.Builder, Is.Not.Null, "Builder should not be null");
            IConstraint constraint = expression.Builder.Resolve();
            Assert.That(() => Assert.That("abc", constraint), Throws.InstanceOf<AssertionException>()
                .With.Message.Contains("Expected: not equal to \"abc\"")
                .And.Message.Contains("But was:  \"abc\""));
        }
    }
}
