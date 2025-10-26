using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NUnit.Framework
{
    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    public abstract partial class Assert
    {
        public static ActualValueBuilder<TActual> That<TActual>(
            TActual value,
            [CallerArgumentExpression(nameof(value))] string actualExpression = "")
        {
            return new ActualValueBuilder<TActual>(value, $"{nameof(Assert)}.{nameof(That)}({actualExpression})");
        }

        // These are needed to resolve compile time resolution conflicts with the pre-existing methods.
        public static void That(bool value) => Assert.That(value, Is.True);
        public static void That(bool value, string? message) => Assert.That(value, Is.True, message);
        public static void That(bool value, FormattableString message) => Assert.That(value, Is.True, message);
        public static void That(bool value, Func<string> getMessage) => Assert.That(value, Is.True, getMessage);
    }

    public interface IBuilder<T>
    {
        string ExpressionString { get; }
    }

    public interface IRunableBuilder<T> : IBuilder<T>
    {
        T Run();
    }

    public static class RunnableBuilderExtensions
    {
#if NETFRAMEWORK
        public static TaskAwaiter<TActual> GetAwaiter<TActual>(this IRunableBuilder<TActual> runnable) => Task.FromResult(runnable.Run()).GetAwaiter();
#else
        public static ValueTaskAwaiter<TActual> GetAwaiter<TActual>(this IRunableBuilder<TActual> runnable) => ValueTask.FromResult(runnable.Run()).GetAwaiter();
#endif
    }

    public readonly struct ActualValueBuilder<TActual> : IBuilder<TActual>
    {
        private readonly FormattableString _actualExpression;

        public ActualValueBuilder(TActual value, FormattableString actualExpression)
        {
            Actual = value;
            _actualExpression = actualExpression;
        }

        public TActual Actual { get; }

        public string ExpressionString => _actualExpression.ToString();

        public ActualValueBuilder<TActual> Is => new(Actual, $"{_actualExpression}.{nameof(Is)}");
    }

    public readonly struct ConstraintBuilder<TActual, TConstraint> : IRunableBuilder<TActual>
        where TConstraint : IConstraint
    {
        private readonly ActualValueBuilder<TActual> _actualValueBuilder;
        private readonly TConstraint _constraint;
        private readonly FormattableString _constraintExpression;

        public ConstraintBuilder(ActualValueBuilder<TActual> actualValueBuilder, TConstraint constraint, FormattableString constraintExpression)
        {
            _actualValueBuilder = actualValueBuilder;
            _constraint = constraint;
            _constraintExpression = constraintExpression;
        }

        public string ExpressionString => $"{_actualValueBuilder.ExpressionString}.{_constraintExpression}";

        public TActual Run()
        {
            Assert.IncrementAssertCount();
            var result = _constraint.ApplyTo(_actualValueBuilder.Actual);
            if (!result.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter(ExpressionString);
                result.WriteMessageTo(writer);

                Assert.ReportFailure(writer.ToString());
            }

            return _actualValueBuilder.Actual;
        }
    }

    public static class ConstraintExtensions
    {
        public static ConstraintBuilder<T, EqualConstraint<T>> EqualTo<T>(
            this ActualValueBuilder<T> builder,
            T expected,
            [CallerArgumentExpression(nameof(expected))] string expectedExpression = "")
        {
            return new ConstraintBuilder<T, EqualConstraint<T>>(builder, new EqualConstraint<T>(expected), $"{nameof(EqualTo)}({expectedExpression})");
        }
    }
}
