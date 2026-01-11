// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    internal sealed class PlatformInconsistency
    {
        private readonly string _message;
        private readonly RuntimeType _runtimeType;
        private readonly Version? _minVersion;
        private readonly Version? _maxVersion;

        private PlatformInconsistency(string message, RuntimeType runtimeType, Version? minVersion, Version? maxVersion)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Descriptive message must be specified.", nameof(message));

            _message = message;
            _runtimeType = runtimeType;
            _minVersion = minVersion;
            _maxVersion = maxVersion;
        }

        private static Version GetCurrentVersion()
        {
            if (RuntimeFramework.CurrentFramework.Runtime == RuntimeType.Mono)
            {
                var displayName = RuntimeFramework.CurrentFramework.DisplayName;

                return new Version(displayName.Substring(0, displayName.IndexOf(' ')));
            }

            return RuntimeFramework.CurrentFramework.FrameworkVersion;
        }

        private bool CurrentPlatformIsInconsistent
        {
            get
            {
                if (RuntimeFramework.CurrentFramework.Runtime != _runtimeType)
                    return false;

                var version = GetCurrentVersion();
                if (_minVersion is not null && version < _minVersion)
                    return false;
                if (_maxVersion is not null && version > _maxVersion)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Does nothing if the current platform is affected; otherwise, executes the specified action.
        /// </summary>
        public void SkipOnAffectedPlatform(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (!CurrentPlatformIsInconsistent)
                action.Invoke();
        }

        /// <summary>
        /// Reports the current test as ignored if the current platform is affected; otherwise, executes the specified action.
        /// </summary>
        public void IgnoreOnAffectedPlatform(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (CurrentPlatformIsInconsistent)
                Assert.Ignore(_message);
            else
                action.Invoke();
        }

        /// <summary>
        /// Mono's MethodInfo.Invoke loses the stack trace beginning in 5.20.
        /// </summary>
        public static PlatformInconsistency MonoMethodInfoInvokeLosesStackTrace { get; } = new PlatformInconsistency(
            "Mono's MethodInfo.Invoke loses the stack trace beginning in 5.20.",
            RuntimeType.Mono,
            minVersion: new Version(5, 20),
            maxVersion: null);
    }
}
