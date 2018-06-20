using System.Diagnostics;
using System.Threading.Tasks;

namespace AppxRunner
{
    internal static class Extensions
    {
        public static Task<int> WaitForExitAsync(this Process process)
        {
            if (process.HasExited) return Task.FromResult(process.ExitCode);

            var source = new TaskCompletionSource<int>();

            process.EnableRaisingEvents = true;
            process.Exited += OnProcessExit;

            if (process.HasExited)
            {
                process.Exited -= OnProcessExit;
                source.SetResult(process.ExitCode);
            }

            return source.Task;

            void OnProcessExit(object sender, System.EventArgs e)
            {
                source.SetResult(process.ExitCode);
                process.Exited -= OnProcessExit;
            }
        }
    }
}
