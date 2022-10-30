// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// Checks whether the method to execute is static.
    /// </summary>
    public class StaticMethodValidator : IMethodValidator
    {
        private readonly string _failMessage;

        /// <summary>
        /// Construct a StaticMethodValidator.
        /// </summary>
        /// <param name="failMessage">The error message to output in case the validation fails.</param>
        public StaticMethodValidator(string failMessage)
        {
            Guard.ArgumentNotNullOrEmpty(failMessage, nameof(failMessage));

            _failMessage = failMessage;
        }

        /// <summary>
        /// Determines whether a method to execute is static and throws an InvalidOperationException otherwise.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        public void Validate(MethodInfo method)
        {
            if (!method.IsStatic)
                throw new InvalidOperationException(_failMessage);
        }
    }
}
