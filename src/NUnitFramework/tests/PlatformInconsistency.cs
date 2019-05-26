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
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    internal sealed class PlatformInconsistency
    {
        private readonly RuntimeType _runtimeType;
        private readonly Version _minVersion;
        private readonly Version _maxVersion;

        private PlatformInconsistency(RuntimeType runtimeType, Version minVersion, Version maxVersion)
        {
            _runtimeType = runtimeType;
            _minVersion = minVersion;
            _maxVersion = maxVersion;
        }

#if PLATFORM_DETECTION
        private static Version GetCurrentVersion()
        {
            if (RuntimeFramework.CurrentFramework.Runtime == RuntimeType.Mono)
            {
                var displayName = RuntimeFramework.CurrentFramework.DisplayName;

                return new Version(displayName.Substring(0, displayName.IndexOf(' ')));
            }

            return RuntimeFramework.CurrentFramework.FrameworkVersion;
        }
#endif

        private bool CurrentPlatformIsInconsistent
        {
            get
            {
#if PLATFORM_DETECTION
                if (RuntimeFramework.CurrentFramework.Runtime != _runtimeType) return false;

                var version = GetCurrentVersion();
                if (_minVersion != null && version < _minVersion) return false;
                if (_maxVersion != null && version > _maxVersion) return false;

                return true;
#else
                return false;
#endif
            }
        }

        public void SkipOnAffectedPlatform(Action action)
        {
            Guard.ArgumentNotNull(action, nameof(action));

            if (!CurrentPlatformIsInconsistent) action.Invoke();
        }

        /// <summary>
        /// Mono's MethodInfo.Invoke loses the stack trace beginning in 5.20.
        /// </summary>
        public static PlatformInconsistency MonoMethodInfoInvokeLosesStackTrace { get; } = new PlatformInconsistency(
            RuntimeType.Mono,
            minVersion: new Version(5, 20),
            maxVersion: null);
    }
}
