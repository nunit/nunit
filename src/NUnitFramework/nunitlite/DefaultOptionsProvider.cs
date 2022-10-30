// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Common
{
    internal sealed class DefaultOptionsProvider : IDefaultOptionsProvider
    {
        private const string EnvironmentVariableTeamcityProjectName = "TEAMCITY_PROJECT_NAME";

        public bool TeamCity
        {
            get
            {
                return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariableTeamcityProjectName));
            }
        }
    }
}
