// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Interface for constraints which support asynchrnious ApplyAsync.
    /// </summary>
    internal interface IAsyncConstraint : IConstraint
    {
        /// <summary>
        /// Applies the constraint to a delegate that returns the task.
        /// The default implementation simply evaluates the delegate and awaits the task
        /// but derived classes may override it to provide for delayed processing.
        /// </summary>
        /// <typeparam name="TActual"></typeparam>
        /// <param name="delTask"></param>
        /// <returns></returns>
        Task<ConstraintResult> ApplyToAsync<TActual>(Func<Task<TActual>> delTask);
    }
}
