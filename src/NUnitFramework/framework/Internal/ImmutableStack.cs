// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A minimalistic implementation of an immutable stack. Add members as needed.
    /// </summary>
    internal struct ImmutableStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// Represents an empty stack. <see langword="default"/> may be used instead.
        /// </summary>
        public static ImmutableStack<T> Empty => default(ImmutableStack<T>);

        private readonly Node _head;

        private ImmutableStack(Node head)
        {
            _head = head;
        }

        /// <summary>
        /// Returns a new immutable stack which begins with the specified value and continues with the values in the
        /// current stack.
        /// </summary>
        /// <param name="value">The beginning value of the new stack.</param>
        public ImmutableStack<T> Push(T value)
        {
            return new ImmutableStack<T>(new Node(value, _head));
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var current = _head; current != null; current = current.Next)
                yield return current.Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class Node
        {
            public Node(T value, Node next)
            {
                Value = value;
                Next = next;
            }

            public T Value { get; }
            public Node Next { get; }
        }
    }
}
