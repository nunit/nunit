using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using NUnitLite;

namespace uap10_0
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await Task.Run(() =>
            {
                var applicationArgs = new CommandLineArgsReader(e.Arguments, containsOnlyArgs: true);
                var exitCodeFile = applicationArgs.ReadNext();

                var assemblyPath = Path.Combine(Package.Current.InstalledLocation.Path, "uap10.0.dll");
                var runnerArgs = new List<string> { assemblyPath };
                runnerArgs.AddRange(applicationArgs.ReadToEnd());

                var exitCode = new TextRunner(testAssembly: null).Execute(runnerArgs.ToArray());
                File.WriteAllText(exitCodeFile, exitCode.ToString());
            });

            Exit();
        }
    }
}
