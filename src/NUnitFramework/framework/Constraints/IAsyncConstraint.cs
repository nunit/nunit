// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if ASYNC
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// All constraints should implement this interface, easily done by inheriting from
    /// <see cref="Constraint"/>.
    /// These methods are only separate from <see cref="IConstraint"/> for backwards compatibility.
    /// </summary>
    public interface IAsyncConstraint : IConstraint
    {
        /// <summary>
        /// Applies the constraint to an actual value, asynchronously returning a <see cref="ConstraintResult"/>.
        /// </summary>
        /// <param name="actual">The value to be tested.</param>
        Task<ConstraintResult> ApplyToAsync(object actual);
    }
}
#endif
