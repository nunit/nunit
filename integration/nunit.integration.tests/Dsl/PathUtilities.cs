namespace nunit.integration.tests.Dsl
{
    using System;

    using Microsoft.Build.Utilities;

    internal static class PathUtilities
    {
        public static string GetNUnitAssembliesPath(TargetDotNetFrameworkVersion frameworkVersion)
        {
            switch (frameworkVersion)
            {
                case TargetDotNetFrameworkVersion.Version45:
                    return "net-4.5";                    

                case TargetDotNetFrameworkVersion.Version40:
                    return "net-4.0";                    

                case TargetDotNetFrameworkVersion.Version20:
                    return "net-2.0";                    

                default:
                    throw new NotSupportedException(frameworkVersion.ToString());
            }            
        }
    }
}
