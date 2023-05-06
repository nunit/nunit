// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private readonly T? _value;
        private readonly string? _errorMessage;

        private Result(T? value, string? errorMessage)
        {
            _value = value;
            _errorMessage = errorMessage;
        }

        public static Result<T> Success(T? value)
        {
            return new Result<T>(value, errorMessage: null);
        }

        public static Result<T> Error(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Error message must be specified.", nameof(message));

            return new Result<T>(default(T), message);
        }

        public bool IsSuccess(out T? value)
        {
            value = _value;
            return _errorMessage is null;
        }

        public bool IsError(out string? message)
        {
            message = _errorMessage;
            return _errorMessage is not null;
        }

        public T? Value
        {
            get
            {
                if (_errorMessage is not null) throw new InvalidOperationException("The result is not success.");
                return _value;
            }
        }

        public static implicit operator Result<T>(T value) => Success(value);

        public static implicit operator Result<T>(string value) => Error(value);
    }
}
