namespace nunit.integration.tests.Dsl
{
    using System;

    internal class CmdArg
    {
        private readonly DataType _argType;

        private readonly string _value;

        public CmdArg(DataType argType, string value = "")
        {
            _argType = argType;
            _value = value;
        }

        public string ConvertToString(NUnitVersion version)
        {
            switch (version)
            {
                case NUnitVersion.NUnit3:
                    switch (_argType)
                    {
                        case DataType.TeamCity:
                            return "--teamcity";

                        case DataType.Where:
                            return $"--where \"{_value}\"";

                        case DataType.Workers:
                            return $"--workers={_value}";

                        case DataType.Agents:
                            return $"--agents={_value}";

                        case DataType.WorkingDirectory:
                            return $"--work={_value}";

                        case DataType.Process:
                            return $"--process={_value}";

                        case DataType.Explore:
                            return $"--explore={_value}";

                        case DataType.TestList:
                            return $"--testlist={_value}";

                        default:
                            throw new NotSupportedException(_argType.ToString());
                    }

                default:
                    throw new NotSupportedException(version.ToString());
            }
        }
    }
}
