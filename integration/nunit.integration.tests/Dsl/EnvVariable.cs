namespace nunit.integration.tests.Dsl
{
    using System;

    internal class EnvVariable
    {
        private readonly DataType _envVariableType;

        public EnvVariable(DataType envVariableType)
        {
            _envVariableType = envVariableType;
        }

        public string GetName(NUnitVersion version)
        {
            return GetData(version).Item1;
        }

        public string GetValue(NUnitVersion version)
        {
            return GetData(version).Item2;
        }

        private Tuple<string, string> GetData(NUnitVersion version)
        {
            switch (version)
            {
                case NUnitVersion.NUnit3:
                    switch (_envVariableType)
                    {
                        case DataType.TeamCity:
                            return Tuple.Create("TEAMCITY_PROJECT_NAME", "Test");

                        default:
                            throw new NotSupportedException(this._envVariableType.ToString());
                    }

                default:
                    throw new NotSupportedException(version.ToString());
            }
        }
    }
}
