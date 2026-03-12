// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies the handling behavior for unhandled exceptions in the assembly, class, or method.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to assemblies, classes, or methods to define how unhandled
    /// exceptions should be managed. It allows for centralized exception handling strategies across different
    /// components of an application.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class UnhandledExceptionHandlingAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionHandlingAttribute"/> class with the specified handling behavior.
        /// </summary>
        /// <param name="handling">The flag indicating how exceptions not handled by the user should be handled.</param>
        public UnhandledExceptionHandlingAttribute(UnhandledExceptionHandling handling)
        {
            Properties.Add(PropertyNames.UnhandledExceptionHandling,
                           new UnhandledExceptionConfiguration(handling, null));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionHandlingAttribute"/> class with the specified handling behavior.
        /// </summary>
        /// <param name="handling">The flag indicating how exceptions not handled by the user should be handled.</param>
        /// <param name="exceptions">The exceptions types to be handled liek this.</param>
        public UnhandledExceptionHandlingAttribute(UnhandledExceptionHandling handling, params Type[] exceptions)
        {
            Properties.Add(PropertyNames.UnhandledExceptionHandling, new UnhandledExceptionConfiguration(handling, exceptions));
        }
    }

    /// <summary>
    /// Flag to change how NUnit deals with exceptions unhandled by the test code.
    /// </summary>
    /// <remarks>
    /// Note this only deals with exception raised on other threads than the main test thread.
    /// Exceptions on the main test thread will always cause the test to fail, even if this attribute is set to Ignore.
    /// </remarks>
    public enum UnhandledExceptionHandling
    {
        /// <summary>
        /// Unhandled exceptions will be treated as test errors.
        /// This is the default behavior matching what NUnit does if an exception is thrown in the test thread.
        /// </summary>
        Error,
        /// <summary>
        /// Unhandled exceptions will be ignored and not cause the test to fail.
        /// This is not recommended, as it may hide issues in the test code or the system under test.
        /// </summary>
        Ignore,
        /// <summary>
        /// This value is provided for clarity and to explicitly indicate the default handling behavior.
        /// </summary>
        Default = Error,
    }

    internal sealed record UnhandledExceptionConfiguration(UnhandledExceptionHandling Handling, Type[]? Exceptions);
}
