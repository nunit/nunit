// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

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
