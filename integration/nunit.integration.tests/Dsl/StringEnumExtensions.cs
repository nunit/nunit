namespace nunit.integration.tests.Dsl
{
    using System;

    using Microsoft.Build.Utilities;
    using Microsoft.CodeAnalysis;

    internal static class StringEnumExtensions
    {
        public static ConfigurationType ConvertToConfigurationType(this string configurationTypeStr)
        {
            return ConvertToEnumType<ConfigurationType>(configurationTypeStr);
        }

        public static TargetDotNetFrameworkVersion ConvertToFrameworkVersion(this string frameworkVersionStr)
        {
            return ConvertToEnumType<TargetDotNetFrameworkVersion>(frameworkVersionStr);
        }

        public static DataType ConvertToNUnitArg(this string nunitArgStr)
        {
            return ConvertToEnumType<DataType>(nunitArgStr);
        }

        public static NUnitVersion ConvertToNUnitVersion(this string nuitVersionStr)
        {
            return ConvertToEnumType<NUnitVersion>(nuitVersionStr);
        }

        public static TeamCityIntegration ConvertToTeamCityIntegration(this string teamCityIntegrationStr)
        {
            return ConvertToEnumType<TeamCityIntegration>(teamCityIntegrationStr);
        }

        public static Platform ConvertToPlatform(this string platformStr)
        {
            return ConvertToEnumType<Platform>(platformStr);
        }        

        private static T ConvertToEnumType<T>(this string strValue)
        {
            return (T)Enum.Parse(typeof(T), strValue, true);
        }
    }
}
