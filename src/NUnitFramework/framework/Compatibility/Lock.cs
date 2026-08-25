// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
namespace System.Threading;

#if !NET9_0_OR_GREATER

using System.Runtime.CompilerServices;

internal sealed class Lock
{
    private readonly object _lock = new();

    public bool IsHeldByCurrentThread => Monitor.IsEntered(_lock);

    public void Enter() => Monitor.Enter(_lock);
    public bool TryEnter() => Monitor.TryEnter(_lock);
    public bool TryEnter(System.TimeSpan timeout) => Monitor.TryEnter(_lock, timeout);
    public bool TryEnter(int millisecondsTimeout) => Monitor.TryEnter(_lock, millisecondsTimeout);
    public void Exit() => Monitor.Exit(_lock);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scope EnterScope()
    {
        Enter();
        return new Scope(this);
    }

    public ref struct Scope(Lock l)
    {
        public readonly void Dispose() => l.Exit();
    }
}
#endif
