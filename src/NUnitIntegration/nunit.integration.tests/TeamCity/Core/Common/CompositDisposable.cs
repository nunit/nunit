using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Integration.Tests.TeamCity.Core.Common
{
    public class CompositDisposable : IDisposable, IEnumerable<IDisposable>
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