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

using System;
using System.Reflection;
using NUnit.Compatibility;

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
