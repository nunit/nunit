using System.Reflection;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// Validates method to execute.
    /// </summary>
    public interface IMethodValidator
    {
        /// <summary>
        /// Determines whether a method is allowed to execute and throws an exception otherwise.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        void Validate(MethodInfo method);
    }
}