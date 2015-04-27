using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface ICmdLineToolTest : ITest
    {
        TestResultDto Run([NotNull] CmdLineToolDto cmdLineToolDto, [NotNull] CaseDto caseDto);
    }
}