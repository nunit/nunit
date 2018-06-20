using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace AppxRunner
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            var args = CommandLineArgsReader.ReadCurrent();
            
            return await Run(
                appxRecipePath: args.ReadNext(),
                applicationId: args.ReadNext(),
                applicationArgs: args.ReadToEnd()).ConfigureAwait(false);
        }

        public static async Task<int> Run(string appxRecipePath, string applicationId, string applicationArgs)
        {
            var recipe = AppxRecipe.Load(appxRecipePath);
            recipe.InitializeLayout();

            var manager = new PackageManager();

            var package = await Install(manager, recipe).ConfigureAwait(false);
            try
            {
                using (var process = Process.GetProcessById(ApplicationUtils.Launch(package.Id.FamilyName, applicationId, applicationArgs)))
                {
                    return await process.WaitForExitAsync().ConfigureAwait(false);
                }
            }
            finally
            {
               await manager.RemovePackageAsync(package.Id.FullName);
            }
        }

        private static async Task<Package> Install(PackageManager manager, AppxRecipe recipe)
        {
            var existingPackage = FindPackage(manager, recipe);
            if (existingPackage != null)
            {
                await manager.RemovePackageAsync(existingPackage.Id.FullName);
            }

            await manager.RegisterPackageAsync(
                new Uri(Path.Combine(recipe.LayoutDir, "AppxManifest.xml")),
                dependencyPackageUris: null,
                DeploymentOptions.DevelopmentMode | DeploymentOptions.ForceApplicationShutdown | DeploymentOptions.ForceTargetApplicationShutdown);

            return FindPackage(manager, recipe) ?? throw new Exception("Registered package but failed to find it.");
        }

        private static Package FindPackage(PackageManager manager, AppxRecipe recipe)
        {
            return manager
                .FindPackagesForUserWithPackageTypes(
                    userSecurityId: string.Empty,
                    recipe.PackageIdentityName,
                    recipe.PackageIdentityPublisher,
                    PackageTypes.Main | PackageTypes.Framework | PackageTypes.Resource | PackageTypes.Xap | PackageTypes.Optional)
                .SingleOrDefault();
        }
    }
}
