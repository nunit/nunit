using System;

namespace NUnit.TestData
{
    public struct AsyncWorkload
    {
        private readonly Action _beforeReturningAwaitable;
        private readonly Action _beforeReturningAwaiter;
        private readonly Func<bool> _isCompleted;
        private readonly Action<Action> _onCompleted;
        private readonly Func<object> _getResult;

        public AsyncWorkload(bool isCompleted, Action<Action> onCompleted, Func<object> getResult)
            : this(null, null, () => isCompleted, onCompleted, getResult)
        {
        }

        public AsyncWorkload(Func<bool> isCompleted, Action<Action> onCompleted, Func<object> getResult)
            : this(null, null, isCompleted, onCompleted, getResult)
        {
        }

        public AsyncWorkload(
            Action beforeReturningAwaitable,
            Action beforeReturningAwaiter,
            Func<bool> isCompleted,
            Action<Action> onCompleted,
            Func<object> getResult)
        {
            _beforeReturningAwaitable = beforeReturningAwaitable;
            _beforeReturningAwaiter = beforeReturningAwaiter;
            _isCompleted = isCompleted;
            _onCompleted = onCompleted;
            _getResult = getResult;
        }

        public void BeforeReturningAwaitable() => _beforeReturningAwaitable?.Invoke();

        public void BeforeReturningAwaiter() => _beforeReturningAwaiter?.Invoke();

        public bool IsCompleted => _isCompleted.Invoke();

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation.Invoke();
            else
                _onCompleted.Invoke(continuation);
        }

        public object GetResult()
        {
            return _getResult.Invoke();
        }
    }
}
