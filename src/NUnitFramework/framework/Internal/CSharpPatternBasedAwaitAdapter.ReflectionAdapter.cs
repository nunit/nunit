// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal
{
    internal static partial class CSharpPatternBasedAwaitAdapter
    {
        private sealed class ReflectionAdapter : DefaultBlockingAwaitAdapter
        {
            private readonly object _awaiter;
            private readonly Func<bool> _awaiterIsCompleted;
            private readonly Action<Action> _awaiterOnCompleted;
            private readonly MethodInfo _getResultMethod;

            public ReflectionAdapter(object awaiter, MethodInfo isCompletedGetter, MethodInfo onCompletedMethod, MethodInfo getResultMethod)
            {
                _awaiter = awaiter;
                _awaiterIsCompleted = (Func<bool>)isCompletedGetter.CreateDelegate(typeof(Func<bool>), awaiter);
                _awaiterOnCompleted = (Action<Action>)onCompletedMethod.CreateDelegate(typeof(Action<Action>), awaiter);
                _getResultMethod = getResultMethod;
            }

            public override bool IsCompleted => _awaiterIsCompleted.Invoke();

            public override void OnCompleted(Action action) => _awaiterOnCompleted.Invoke(action);

            public override object GetResult() => _getResultMethod.InvokeWithTransparentExceptions(_awaiter);
        }
    }
}
