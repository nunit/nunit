using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Common
{
    using System;
    using System.Diagnostics.Contracts;

    public static class Disposable
    {
        [NotNull]
        public static IDisposable Create([NotNull] Action dispose)
        {
            return new DisposableAction(dispose);
        }

        private sealed class DisposableAction : IDisposable
        {
            private Action _dispose;

            public DisposableAction([NotNull] Action dispose)
            {
                Contract.Requires<ArgumentNullException>(dispose != null);

                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose();
                _dispose = () => { };
            }
        }
    }
}
