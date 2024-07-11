// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeTearDownCommand performs any teardown actions
    /// specified for a suite and calls Dispose on the user
    /// test object, if any.
    /// </summary>
    public class DisposeFixtureCommand : AfterTestCommand
    {
        /// <summary>
        /// Construct a OneTimeTearDownCommand
        /// </summary>
        /// <param name="innerCommand">The command wrapped by this command</param>
        public DisposeFixtureCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            Guard.OperationValid(
                HasDisposableFixture(Test),
                $"DisposeFixtureCommand does not apply neither to {Test.GetType().Name}, nor to {Test.Parent?.GetType().Name ?? "it's parent (null)"}");

            AfterTest = context =>
            {
                try
                {
                    DisposeHelper.EnsureDisposed(context.TestObject);
                }
                catch (Exception ex)
                {
                    context.CurrentResult.RecordTearDownException(ex);
                }
            };
        }

        private static bool HasDisposableFixture(ITest test)
        {
            ITest? current = test;
            do
            {
                if (current is IDisposableFixture)
                    return true;

                current = current.Parent;
            }
            while (current is not null);

            return false;
        }
    }
}
