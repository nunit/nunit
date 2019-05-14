// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Provides idiomatic callback-based assertions.
    /// </summary>
    /// <example>
    /// <code>
    /// var watcher = new CallbackWatcher();
    ///
    /// systemUnderTest.PropertyChanged += (sender, e) => watcher.OnCallback();
    ///
    /// using (watcher.ExpectCallback())
    /// {
    ///     systemUnderTest.SomeProperty = 42;
    /// } // Fails if PropertyChanged did not fire
    ///
    /// systemUnderTest.SomeProperty = 42; // Fails if PropertyChanged fires
    /// </code>
    /// </example>
    public sealed class CallbackWatcher
    {
        private int _expectedCount;
        private int _actualCount;

        /// <summary>
        /// Begins expecting callbacks. When the returned scope is disposed, stops expecting callbacks and throws <see cref="AssertionException"/>
        /// if <see cref="OnCallback"/> was not called the expected number of times between beginning and ending.
        /// </summary>
        /// <param name="count">
        /// The number of times that <see cref="OnCallback"/> is expected to be called before the returned scope is disposed.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is less than 1.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the returned scope from the previous call has not been disposed.</exception>
        /// <exception cref="AssertionException">Thrown when <see cref="OnCallback"/> was not called the expected number of times between beginning and ending.</exception>
        public IDisposable ExpectCallback(int count = 1)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), _expectedCount, "Expected callback count must be greater than or equal to zero.");

            if (_expectedCount != 0)
                throw new InvalidOperationException($"The previous {nameof(ExpectCallback)} scope must be disposed before calling again.");

            _expectedCount = count;
            _actualCount = 0;

            return On.Dispose(() =>
            {
                try
                {
                    if (_actualCount < _expectedCount)
                        Assert.Fail($"Expected {(_expectedCount == 1 ? "a single call" : _expectedCount + " calls")}, but there {(_actualCount == 1 ? "was" : "were")} {_actualCount}.");
                }
                finally
                {
                    _expectedCount = 0;
                }
            });
        }

        /// <summary>
        /// Call this from the callback being tested.
        /// Throws <see cref="AssertionException"/> if this watcher is not currently expecting a callback
        /// (see <see cref="ExpectCallback"/>) or if the expected number of callbacks has been exceeded.
        /// </summary>
        /// <exception cref="AssertionException">
        /// Thrown when this watcher is not currently expecting a callback (see <see cref="ExpectCallback"/>)
        /// or if the expected number of callbacks has been exceeded.
        /// </exception>
        public void OnCallback()
        {
            _actualCount++;
            if (_actualCount > _expectedCount)
            {
                Assert.Fail(_expectedCount == 0
                    ? "Expected no callback."
                    : $"Expected {(_expectedCount == 1 ? "a single call" : _expectedCount + " calls")}, but there were more.");
            }
        }
    }
}
