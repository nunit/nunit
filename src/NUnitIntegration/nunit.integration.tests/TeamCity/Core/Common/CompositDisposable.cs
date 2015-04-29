// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Integration.Tests.TeamCity.Core.Common
{
    public sealed class CompositDisposable : IDisposable, IEnumerable<IDisposable>
    {
        private readonly List<IDisposable> _disposable = new List<IDisposable>();
        private bool _disposed;

        public CompositDisposable(params IDisposable[] disposable)
        {            
            _disposable.AddRange(disposable);
        }

        public void Add(IDisposable disposable)
        {
            if (_disposed)
            {
                disposable.Dispose();
                return;
            }

            _disposable.Add(disposable);
        }

        public void Dispose()
        {
            _disposed = true;
            var dispList = _disposable.ToList();
            dispList.Reverse();
            foreach (var disposable in dispList)
            {
                disposable.Dispose();
                _disposable.Remove(disposable);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposable.GetEnumerator();
        }
    }
}