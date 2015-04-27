using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using JetBrains.TeamCityCert.Tools.Contracts;

    internal interface ICmdLineToolTest : ITest
    {
        TestResultDto Run([NotNull] CmdLineToolDto cmdLineToolDto, [NotNull] CaseDto caseDto);
    }
}