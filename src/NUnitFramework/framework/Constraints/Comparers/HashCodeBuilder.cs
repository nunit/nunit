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

using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Helper class that supports the implementation of <see cref="object.GetHashCode"/>
    /// method overrides.
    /// </summary>
    internal struct HashCodeBuilder
    {
        #region Constants

        private const uint InitialHash = 2166136261;
        private const uint Multiplier = 16777619;

        #endregion

        #region Instance fields

        private readonly NUnitEqualityComparer _equalityComparer;
        private uint _hash;

        #endregion

        #region Constructor(s)

        public HashCodeBuilder(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
            _hash = InitialHash;
        }

        #endregion

        #region Public methods

        public HashCodeBuilder Append(int data)
        {
            Hash(ref _hash, (uint)data);
            return this;
        }

        public HashCodeBuilder Append(uint data)
        {
            Hash(ref _hash, data);
            return this;
        }

        public HashCodeBuilder AppendAll(IEnumerable data)
        {
            if (data != null)
            {
                foreach (object item in data)
                {
                    Hash(ref _hash, (uint)_equalityComparer.GetHashCode(item, topLevelComparison: false));
                }
            }
            return this;
        }

        public HashCodeBuilder AppendAll(IDictionary data)
        {
            if (data != null)
            {
                foreach (DictionaryEntry item in data)
                {
                    Hash(ref _hash, (uint)_equalityComparer.GetHashCode(item.Key, topLevelComparison: false));
                    Hash(ref _hash, (uint)_equalityComparer.GetHashCode(item.Value, topLevelComparison: false));
                }
            }
            return this;
        }

        public HashCodeBuilder AppendAllUnordered(IEnumerable data)
        {
            if (data != null)
            {
                unchecked
                {
                    uint sum = 0;
                    uint count = 0;
                    foreach (object item in data)
                    {
                        sum += (uint)_equalityComparer.GetHashCode(item, topLevelComparison: false);
                        count++;
                    }
                    Hash(ref _hash, sum);
                    Hash(ref _hash, count);
                }
            }
            return this;
        }

        public HashCodeBuilder AppendAllUnordered(IDictionary data)
        {
            if (data != null)
            {
                unchecked
                {
                    uint keySum = 0;
                    uint valueSum = 0;
                    uint count = 0;
                    foreach (DictionaryEntry item in data)
                    {
                        keySum += (uint)_equalityComparer.GetHashCode(item.Key, topLevelComparison: false);
                        valueSum += (uint)_equalityComparer.GetHashCode(item.Value, topLevelComparison: false);
                        count++;
                    }
                    Hash(ref _hash, keySum);
                    Hash(ref _hash, valueSum);
                    Hash(ref _hash, count);
                }
            }
            return this;
        }

        public HashCodeBuilder Append(object data)
        {
            Hash(ref _hash, (uint)_equalityComparer.GetHashCode(data, topLevelComparison: false));
            return this;
        }

        public bool Equals(HashCodeBuilder other)
        {
            return _hash == other._hash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HashCodeBuilder)) return false;
            return Equals((HashCodeBuilder)obj);
        }

        public override int GetHashCode()
        {
            var hash = _hash;
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return (int)hash;
        }

        #endregion

        #region Private methods

        private static void Hash(ref uint hash, uint data)
        {
            hash = (hash ^ (data & 0xFF)) * Multiplier;
            hash = (hash ^ ((data >> 8) & 0xFF)) * Multiplier;
            hash = (hash ^ ((data >> 16) & 0xFF)) * Multiplier;
            hash = (hash ^ ((data >> 24) & 0xFF)) * Multiplier;
        }

        #endregion
    }
}
