namespace JetBrains.TeamCityCert.Tools
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.TeamCityCert.Tools.Common;
    using JetBrains.TeamCityCert.Tools.Contracts;

    internal sealed class TestsRepository : ITestsRepository, ICheckList
    {
        public IEnumerable<ICmdLineToolTest> GetCmdLineToolTests(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICmdLineToolTest>>() != null);

            return 
                from test in ServiceLocator.Root.GetAllServices<ICmdLineToolTest>()
                where test.CertType == certType
                select test;
        }

        public IEnumerable<ITest> GetCheckList(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ITest>>() != null);

            return GetCmdLineToolTests(certType);
        }
    }
}
