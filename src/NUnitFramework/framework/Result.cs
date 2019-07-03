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

using System;

namespace NUnit.Framework
{
    internal static class Result
    {
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);

        public static Result<T> Error<T>(string message) => Result<T>.Error(message);
    }

    internal struct Result<T>
    {
        private readonly T _value;
        private readonly string _errorMessage;

        private Result(T value, string errorMessage)
        {
            _value = value;
            _errorMessage = errorMessage;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, errorMessage: null);
        }

        public static Result<T> Error(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Error message must be specified.", nameof(message));

            return new Result<T>(default(T), message);
        }

        public bool IsSuccess(out T value)
        {
            value = _value;
            return _errorMessage is null;
        }

        public bool IsError(out string message)
        {
            message = _errorMessage;
            return _errorMessage != null;
        }

        public T Value
        {
            get
            {
                if (_errorMessage != null) throw new InvalidOperationException("The result is not success.");
                return _value;
            }
        }

        public static implicit operator Result<T>(T value) => Success(value);

        public static implicit operator Result<T>(string value) => Error(value);
    }
}
