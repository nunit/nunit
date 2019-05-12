#if NET35

// ReSharper disable CheckNamespace

namespace System.Runtime.CompilerServices
{
    internal interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }

    internal interface ICriticalNotifyCompletion : INotifyCompletion
    {
        void UnsafeOnCompleted(Action continuation);
    }
}

#endif
