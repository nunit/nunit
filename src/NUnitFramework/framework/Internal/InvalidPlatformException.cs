// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

#if PLATFORM_DETECTION
using System;
using System.Runtime.Serialization;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InvalidPlatformException is thrown when the platform name supplied
    /// to a test is not recognized.
    /// </summary>
    [Serializable]
    class InvalidPlatformException : ArgumentException
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class.
        /// </summary>
        public InvalidPlatformException() : base() { }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidPlatformException(string message) : base(message) { }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidPlatformException(string message, Exception inner) : base(message, inner) { }

#if !NETSTANDARD1_6
        /// <summary>
        /// Serialization constructor for the <see cref="InvalidPlatformException"/> class
        /// </summary>
        protected InvalidPlatformException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
#endif
