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
using System.Collections.Concurrent;

namespace NUnit.Framework.Internal
{
    internal static partial class CSharpPatternBasedAwaitAdapter
    {
        private static readonly ConcurrentDictionary<Type, AwaitShapeInfo> ShapeInfoByType = new ConcurrentDictionary<Type, AwaitShapeInfo>();

        public static AwaitAdapter TryCreate(object awaitable)
        {
            if (awaitable == null) return null;

            return GetShapeInfo(awaitable.GetType())?.CreateAwaitAdapter(awaitable);
        }

        public static bool IsAwaitable(Type awaitableType)
        {
            return GetShapeInfo(awaitableType) != null;
        }

        public static Type GetResultType(Type awaitableType)
        {
            return GetShapeInfo(awaitableType)?.ResultType;
        }

        private static AwaitShapeInfo GetShapeInfo(Type type)
        {
            return ShapeInfoByType.GetOrAdd(type, AwaitShapeInfo.TryCreate);
        }
    }
}
