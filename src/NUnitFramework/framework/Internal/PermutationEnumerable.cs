using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    internal static class PermutationEnumerable
    {
        public static PermutationEnumerable<T> PermutationsByElementSources<T>(this T[][] elementSources)
        {
            return new PermutationEnumerable<T>(elementSources);
        }
    }

    internal struct PermutationEnumerable<T> : IEnumerable<PermutationEnumerable<T>.Permutation>
    {
        private readonly T[][] _elementSources;

        public PermutationEnumerable(T[][] elementSources)
        {
            Guard.ArgumentNotNull(elementSources, nameof(elementSources));
            _elementSources = elementSources;
        }

        public Enumerator GetEnumerator() => new Enumerator(_elementSources);

        IEnumerator<Permutation> IEnumerable<Permutation>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<Permutation>
        {
            private readonly T[][] _elementSources;
            private T[] _items;
            private int[] _indexes;

            public Enumerator(T[][] elementSources)
            {
                Guard.ArgumentNotNull(elementSources, nameof(elementSources));
                _elementSources = elementSources;
                _items = null;
                _indexes = null;
            }

            public bool MoveNext()
            {
                if (_items == null)
                {
                    if (_elementSources.Length == 0) return false;

                    _items = new T[_elementSources.Length];

                    for (var i = 0; i < _elementSources.Length; i++)
                    {
                        var source = _elementSources[i];
                        if (source.Length == 0)
                        {
                            _items = null;
                            return false;
                        }

                        _items[i] = source[0];
                    }

                    _indexes = new int[_items.Length];
                    return true;
                }

                for (var incrementIndex = 0; incrementIndex < _indexes.Length; incrementIndex++)
                {
                    _indexes[incrementIndex]++;
                    if (_indexes[incrementIndex] < _elementSources[incrementIndex].Length)
                        return true;

                    _indexes[incrementIndex] = 0;
                }

                return false;
            }

            public Permutation Current => new Permutation(_items);

            object IEnumerator.Current => Current;

            public void Reset()
            {
                _items = null;
                _indexes = null;
                _currentIndex = 1;
            }

            public void Dispose()
            {
            }
        }

        internal struct Permutation : IEnumerable<T>
        {
            private readonly T[] _sharedItems;

            public Permutation(T[] sharedItems)
            {
                Guard.ArgumentNotNull(sharedItems, nameof(sharedItems));
                _sharedItems = sharedItems;
            }

            public T[] ToArray() => (T[])_sharedItems.Clone();

            public T this[int index] => _sharedItems[index];

            public int Count => _sharedItems.Length;

            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_sharedItems).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _sharedItems.GetEnumerator();
        }
    }
}
